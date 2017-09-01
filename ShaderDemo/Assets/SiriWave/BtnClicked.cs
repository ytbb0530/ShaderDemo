using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class BtnClicked : MonoBehaviour 
{
	public WaveShowsTest show;


	private string text;


	public void setText(string _text)
	{
		text = _text;
		Text te = GetComponentInChildren<Text> ();
		te.text = text;
	}

	public void onClick()
	{
		show.setMidi(text);
	}

}
