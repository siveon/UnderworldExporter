﻿using UnityEngine;
using System.Collections;

public class a_move_trigger : trigger_base {
/*
A trigger that fires when the player character enters it
*/
	//public bool playerStartedInTrigger=false;
	
	protected Vector3 boxDimensions= new Vector3(1.2f,1.2f,1.2f);
	protected Vector3 boxCenter= Vector3.zero;
	//protected short TriggerReady=5;
	//protected bool startPosTested=false;
	public bool playerStartedInTrigger=false;
	private BoxCollider box;



	protected override void Start ()
	{				
		base.Start ();
		box=this.gameObject.AddComponent<BoxCollider>();
		box.size = boxDimensions;//new Vector3(1.2f, 1.2f, 1.2f);
		box.center=boxCenter;
		box.isTrigger=true;

		CheckPlayerStart();

		if (objInt().flags ==0)		
		{
			Debug.Log(this.name + " Move trigger with flags = 0. Should this trigger fire when moved into?");
		}

		//Check if this trap points to a teleport trap that goes to another level
		if ( (objInt().tileX<TileMap.TileMapSizeX) && (objInt().tileY<TileMap.TileMapSizeY))
		{
			if (objInt().link!=0)
			{
				GameObject triggerObj = ObjectLoader.getGameObjectAt(objInt().link);
				if (triggerObj!=null)
				{
					if (triggerObj.GetComponent<a_teleport_trap>() !=null)
					{
						if (triggerObj.GetComponent<a_teleport_trap>().objInt().zpos!=0)
						{							
							switch(_RES)
							{
								case GAME_UW2:
									GameWorldController.instance.currentAutoMap().MarkTileDisplayType(objInt().tileX, objInt().tileY, AutoMap.DisplayTypeStairUW2);		
									break;
								default:
									GameWorldController.instance.currentAutoMap().MarkTileDisplayType(objInt().tileX, objInt().tileY, AutoMap.DisplayTypeStairUW1);		
									break;
							}	
						}
					}
				}	
			}	
		}
	}

	void CheckPlayerStart()
	{
		Collider[] colliders=Physics.OverlapBox(this.transform.position, box.size/2);
		for (int i=0; i<=colliders.GetUpperBound(0);i++)
		{
			if( (colliders[i].gameObject.GetComponent<UWCharacter>()!=null) 
								||  (colliders[i].gameObject.GetComponent<Feet>()!=null) 
						)
			{
				playerStartedInTrigger=true;
				break;
			}
		}
	}


	protected virtual void OnTriggerEnter(Collider other)
	{
		if (playerStartedInTrigger!=true)
		{
			if (((other.name==UWCharacter.Instance.name) || (other.name=="Feet")) && (!GameWorldController.EditorMode) && (Quest.instance.InDreamWorld==false))
			{	
				Activate (other.gameObject);
			}	
		}	
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		if (((other.name==UWCharacter.Instance.name) || (other.name=="Feet")) && (!GameWorldController.EditorMode) && (Quest.instance.InDreamWorld==false))
		{
			playerStartedInTrigger=false;		
		}
	}

	public override bool Activate (GameObject src)
	{
		if (_RES==GAME_UW2)
		{//Check for moongates in this tile to support qbert in UW2.
			if(GameWorldController.instance.LevelNo==68)
			{
				ObjectLoaderInfo[] objList=GameWorldController.instance.CurrentObjectList().objInfo;
				for (int i = 0; i < 1024;i++)
				{//Make sure triggers, traps and special items are created.
					if (objList[i]!=null)
					{
						if (objList[i].GetItemType() == ObjectInteraction.MOONGATE)
						{
							if ((objList[i].tileX == objInt().tileX) && (objList[i].tileY == objInt().tileY))
							{
								if (objList[i].instance.invis==1)
								{//No teleporting on an invisible moon gate in this level.
										return false;
								}
								else
								{//Okay to teleport
										break;
								}
							}
						}
					}
				}
			}
		}
		return base.Activate (this.gameObject);
	}
}
