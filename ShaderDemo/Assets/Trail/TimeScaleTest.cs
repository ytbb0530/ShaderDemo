using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleTest : MonoBehaviour 
{
	public Text timeScaleText;

	private int logState = 0;

	private float updateTime;
	private float fixedTime;

	// Use this for initialization
	void Start () 
	{
		updateTime = Time.realtimeSinceStartup;
		fixedTime = Time.realtimeSinceStartup;
	}
	
	void Update () 
	{
		if (logState == 1) {
			float now = Time.realtimeSinceStartup;
			float temp = now - updateTime;
			updateTime = now;
			Debug.Log ("Update: " + temp);
		}
	}

	void FixedUpdate()
	{
		if (logState == 2) {
			float now = Time.realtimeSinceStartup;
			float temp = now - fixedTime;
			fixedTime = now;
			Debug.Log ("FixedUpdate: " + temp);
		}
	}

	public void setLogState()
	{
		if (logState == 1)
			logState = 2;
		else
			logState = 1;
	}

	public void setTimeScale()
	{
		if (Time.timeScale > 0.5f) {
			Time.timeScale = 0.1f;
		} else {
			Time.timeScale = 1;
		}
		timeScaleText.text = Time.timeScale + "";
	}

}
