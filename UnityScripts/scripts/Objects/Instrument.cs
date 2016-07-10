﻿using UnityEngine;
using System.Collections;
/// <summary>
/// Musical instruments
/// </summary>
/// Tracks the notes the player plays to detect the cup of wonder tune.
public class Instrument : object_base {
		/// Is the player currently playing an instrument		
	public static bool PlayingInstrument;
		/// What instrument is currently being played
	static string CurrentInstrument;
		/// Records the last few notes played for a puzzle.
	static string NoteRecord;

	protected override void Start ()
	{
		base.Start ();
	}

	public override bool use ()
	{
		if (objInt.PickedUp==true)
		{
			if (playerUW.playerInventory.ObjectInHand=="")
			{
				if (PlayingInstrument==false)
				{
					PlayInstrument();
				}
				return true;
			}
			else
			{
				return ActivateByObject(playerUW.playerInventory.GetGameObjectInHand());
			}
		}
		else
		{
			return false;
		}

	}

		/// <summary>
		/// Sets the instrucment play UI and state.
		/// </summary>
	public void PlayInstrument()
	{
		WindowDetectUW.WaitingForInput=true;
		playerUW.playerMotor.enabled=false;
		PlayingInstrument=true;
		CurrentInstrument=this.name;
		playerUW.mus.Stop ();
		NoteRecord="";
		//000~001~250~You play the instrument.  (Use 0-9 to play, or ESC to return to game)
		ml.Set (playerUW.StringControl.GetString (1,250));
	}

		/// <summary>
		/// Plays the notes to match the keys pressed
		/// </summary>
	void Update()
	{
		if ((PlayingInstrument == true) && (CurrentInstrument==this.name))
		{

			if (Input.GetKeyDown("1"))
				{PlayNote (1);}
			if (Input.GetKeyDown("2"))
				{PlayNote (2);}
			if (Input.GetKeyDown("3"))
				{PlayNote (3);}
			if (Input.GetKeyDown("4"))
				{PlayNote (4);}
			if (Input.GetKeyDown("5"))
				{PlayNote (5);}
			if (Input.GetKeyDown("6"))
				{PlayNote (6);}
			if (Input.GetKeyDown("7"))
				{PlayNote (7);}
			if (Input.GetKeyDown("8"))
				{PlayNote (8);}
			if (Input.GetKeyDown("9"))
				{PlayNote (9);}
			if (Input.GetKeyDown("0"))
				{PlayNote (10);}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				//000~001~251~You put the instrument down.
				PlayingInstrument=false;
				CurrentInstrument="";
				WindowDetectUW.WaitingForInput=false;
				playerUW.playerMotor.enabled=true;
				ml.Add(playerUW.StringControl.GetString (1,251));
				playerUW.mus.Resume();
				//354237875
				if (NoteRecord=="354237875")
				{
					ml.Add ("Eyesnack would be proud of your playing");
				}
				else
				{
					Debug.Log (NoteRecord);
				}
			}
		}
	}

		/// <summary>
		/// Plays a musical note by adjusting the pitch of a note.
		/// </summary>
		/// <param name="note">Note to play</param>
	void PlayNote(int note)
	{
	//	Debug.Log (noteNo);
		if (note==10)
		{
			NoteRecord=NoteRecord+"0";
		}
		else
		{
			NoteRecord=NoteRecord+note.ToString();
		}

		if (NoteRecord.Length>9)
		{
			NoteRecord=NoteRecord.Remove(0,1);
		}
		//From
		//http://answers.unity3d.com/questions/141771/whats-a-good-way-to-do-dynamically-generated-music.html

		this.GetComponent<AudioSource>().pitch =  Mathf.Pow(2.0f, ((float)note)/12.0f);
		this.GetComponent<AudioSource>().Play();
	}
}