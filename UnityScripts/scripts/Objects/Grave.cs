﻿using UnityEngine;
using System.Collections;

public class Grave : object_base {
	///ID of the grave to lookup
	public int GraveID;

	protected override void Start ()
	{
		base.Start ();
		GraveID=  objInt().objectloaderinfo.DeathWatched;//seriously?????? Need to make this better. Look at BuildObjectList
		CreateGrave(this.gameObject,objInt());
	}


	/// <summary>
	/// Plays cutscene that displays the gravestone.
	/// </summary>
	/// <returns>The <see cref="System.Boolean"/>.</returns>
	public override bool LookAt ()
	{
		//CheckReferences();
		UWHUD.instance.CutScenesSmall.SetAnimation= "cs401_n01_00" + (GraveID-1).ToString ("D2");
		UWHUD.instance.MessageScroll.Add (StringController.instance.GetString (8, objInt().link-512));
		return true;
	}

	public override bool use ()
	{
		if (GameWorldController.instance.playerUW.playerInventory.ObjectInHand=="")
		{
			return LookAt ();
		}
		else
		{
			//TODO: if garamons bones activate something special.
			return ActivateByObject(GameWorldController.instance.playerUW.playerInventory.GetGameObjectInHand());
			//return GameWorldController.instance.playerUW.playerInventory.GetGameObjectInHand().GetComponent<ObjectInteraction>().FailMessage();
		}
	}

		/// <summary>
		/// Activation of this object by another. EG key on door
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="ObjectUsed">Object used.</param>
		/// Special case here for Garamon's grave. Activates a hard coded trigger
	public override bool ActivateByObject (GameObject ObjectUsed)
	{
		ObjectInteraction objIntUsed = ObjectUsed.GetComponent<ObjectInteraction>();
		if (GraveID==6)
			{//Garamon's grave
			//Activates a trigger a_move_trigger_54_52_04_0495 (selected by unknown means)
			if (objIntUsed.item_id==198)//Bones
				{
					if (objIntUsed.quality==63)
					{//Garamons bones
					//Arise Garamon.
					//000~001~134~You thoughtfully give the bones a final resting place.
					UWHUD.instance.MessageScroll.Add(StringController.instance.GetString (1,134));
					GameObject trig = GameObject.Find ("a_move_trigger_54_52_04_0495");
						if (trig!=null)
						{					
							objInt().link++;//Update the grave description
							objIntUsed.consumeObject ();
							trig.GetComponent<ObjectInteraction>().GetComponent<trigger_base>().Activate();
						}
						UWHUD.instance.CursorIcon= UWHUD.instance.CursorIconDefault;
						GameWorldController.instance.playerUW.playerInventory.ObjectInHand="";						
						return true;
					}
					else
					{//Regular bones
						//000~001~259~The bones do not seem at rest in the grave, and you take them back.
						UWHUD.instance.MessageScroll.Add(StringController.instance.GetString (1,259));
						UWHUD.instance.CursorIcon= UWHUD.instance.CursorIconDefault;
						GameWorldController.instance.playerUW.playerInventory.ObjectInHand="";
						return true;
					}
				}
			else
				{
				return ObjectUsed.GetComponent<ObjectInteraction>().FailMessage();
				}
			}
		else
		{
			if ((objIntUsed.item_id==198) && (objIntUsed.quality==63))//Garamons Bones used on the wrong grave
			{
				//000~001~259~The bones do not seem at rest in the grave, and you take them back.
				UWHUD.instance.MessageScroll.Add(StringController.instance.GetString (1,259));
				UWHUD.instance.CursorIcon= UWHUD.instance.CursorIconDefault;
				GameWorldController.instance.playerUW.playerInventory.ObjectInHand="";
				return true;
			}
			else
			{
				UWHUD.instance.CursorIcon= UWHUD.instance.CursorIconDefault;
				GameWorldController.instance.playerUW.playerInventory.ObjectInHand="";
				return ObjectUsed.GetComponent<ObjectInteraction>().FailMessage();
			}
		}
	}


	public override string UseObjectOnVerb_World ()
	{
		ObjectInteraction ObjIntInHand=GameWorldController.instance.playerUW.playerInventory.GetObjIntInHand();
		if (ObjIntInHand!=null)
		{
			switch (ObjIntInHand.item_id)	
			{
			case 198://Bones
				return "bury remains";
			}
		}

		return base.UseObjectOnVerb_Inv();
	}


		public static void CreateGrave(GameObject myObj, ObjectInteraction objInt)
		{//TODO:make this a properly texture model as part of map generation.
			myObj.layer=LayerMask.NameToLayer("MapMesh");

			GameObject SpriteController = GameObject.CreatePrimitive(PrimitiveType.Cube); 
			SpriteController.name = myObj.name + "_cube";
			SpriteController.transform.position = myObj.transform.position;
			SpriteController.transform.rotation=myObj.transform.rotation;
			SpriteController.transform.parent = myObj.transform;
			SpriteController.transform.localScale=new Vector3(0.5f,0.5f,0.1f);
			SpriteController.transform.localPosition=new Vector3(0.0f,0.25f,0.0f);

			MeshRenderer mr = SpriteController.GetComponent<MeshRenderer>();
			mr.material= (Material)Resources.Load (_RES+ "/Materials/tmobj/tmobj_" + objInt.flags+28);
			mr.material.mainTexture= GameWorldController.instance.TmObjArt.LoadImageAt(objInt.flags+28);
			BoxCollider bx = myObj.GetComponent<BoxCollider>();
			bx.center= new Vector3(0.0f,0.25f,0.0f);
			bx.size=new Vector3(0.5f,0.5f,0.1f);
			bx.isTrigger=false;

			bx=SpriteController.GetComponent<BoxCollider>();
			bx.enabled=false;
			Component.DestroyImmediate (bx);
		}
}