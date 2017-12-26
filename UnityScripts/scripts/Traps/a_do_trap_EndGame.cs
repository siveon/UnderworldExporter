﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Ends the game. Plays the ending cutscene.
/// </summary>
public class a_do_trap_EndGame : a_hack_trap {

	public override void ExecuteTrap (object_base src, int triggerX, int triggerY, int State)
	{
		Debug.Log (this.name);
		//base.ExecuteTrap (triggerX, triggerY, State);
		Cutscene_EndGame ce = UWHUD.instance.gameObject.AddComponent<Cutscene_EndGame>();
		UWHUD.instance.CutScenesFull.cs=ce;
		UWHUD.instance.CutScenesFull.Begin();

	}

	public override void PostActivate (object_base src)
	{
		//Stop camera from destroying itself
	}
}
