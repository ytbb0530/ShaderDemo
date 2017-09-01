using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class KnotAnim : MonoBehaviour 
{
	private enum STATE
	{
		NON_START,
		SWELL,
		BOOM,
		GATHER,
	}

	public GameObject particle;
	public float boomWideAdd;

	private int MAX_COUNT = 20;
	private float MAX_TIME = 10;
	private Material mat;
	private List<KnotWave> waves = new List<KnotWave>();
	private float startTime;
	private STATE state;
	private List<float> timePoints;
	private Animation boomAnim;
	private float tempWide;

	void Start () 
	{
		state = STATE.NON_START;
		particle.SetActive (false);
		mat = GetComponent<MeshRenderer> ().material;
		waves.Clear ();
		boomAnim = GetComponent<Animation> ();
		boomAnim.Stop ();

		StartCoroutine (clock());
	}

	void Update ()
	{
		if (state == STATE.SWELL) {
			UpdateSwell ();
		} else if (state == STATE.BOOM) {
			UpdateBoom ();
		}
	}

	IEnumerator clock()
	{
		yield return new WaitForSeconds (2f);

		timePoints = new List<float> ();
		float rate = 1;
		for(int i = 0; i < MAX_COUNT; i++){
			timePoints.Add (1 - rate);
			rate = rate * .75f;
		}

		state = STATE.SWELL;
		startTime = Time.time;
	}

	private void createWave(float rate)
	{
		float startPos = Random.value * .6f + .2f;
		float endPos = startPos + (Random.value-.5f) * .2f;
		float height = (Random.value * .2f + .1f) * rate;
		float length = Random.value * .1f + .05f;
		float time = (Random.value * 1.2f + 1f) * rate;

		KnotWave wave = new KnotWave (waves.Count, startPos, endPos, height, length, time);
		waves.Add (wave);
	}

	void UpdateSwell () 
	{
		float progress = (Time.time - startTime) / MAX_TIME;
		for(int i = timePoints.Count - 1; i >= 0; i--){
			float t = timePoints [i];
			if (progress > t) {
				createWave (1 + progress);
				timePoints.RemoveAt (i);
			}
		}

		List<Vector4> array = new List<Vector4> ();
		float now = Time.time;
		for (int i = waves.Count - 1; i >= 0; i--) {
			KnotWave wave = waves [i];
			if (!wave.timeOver) {
				wave.update (now);
				array.Add (new Vector4(wave.pos, wave.height, wave.length, 0));
			}
		}

		if (array.Count > 0) {
			mat.SetInt ("_Wave_Num", array.Count);
			mat.SetVectorArray ("_Wave", array);
		}

		if (progress > .8f) {
			mat.SetFloat ("_Wide", (progress-0.8f));
		}
		if (progress > 1f) {
			StartCoroutine (particleClock());
		}
	}

	private IEnumerator particleClock()
	{
		state = STATE.BOOM;
		boomWideAdd = 0;
		boomAnim.Play ();
		tempWide = mat.GetFloat ("_Wide");
		yield return new WaitForSeconds (.5f);
		particle.SetActive (true);
	}

	void UpdateBoom ()
	{
		mat.SetFloat ("_Wide", tempWide + boomWideAdd);
	}

}

class KnotWave
{
	public int id{ private set; get;}
	public float pos{ private set; get;}
	public float height{ private set; get;}
	public float length{ private set; get;}
	public bool timeOver{ private set; get;}

	private float startPos;
	private float endPos;
	private float maxHeight;
	private float startTime;
	private float totalTime;
	private float targetLength;
	private float startLength;

	public KnotWave(int _id, float _startPos, float _endPos, float _height, float _length, float _time)
	{
		int random = Random.Range (0, 100);
		if (random % 2 == 0) {
			startLength = _length;
			targetLength = 0;
		} else {
			targetLength = _length;
			startLength = 0;
		}

		id = _id;
		startPos = _startPos;
		endPos = _endPos;
		pos = startPos;
		maxHeight = _height;
		height = 0;
		length = startLength;
		startTime = Time.time;
		totalTime = _time;
		timeOver = false;
	}

	public void update(float now)
	{
		float timeCost = (now - startTime) / totalTime;
		pos = Mathf.Lerp (startPos, endPos, timeCost);
		height = timeCost < 0.5 ? Mathf.Lerp (0, maxHeight, timeCost * 2) : Mathf.Lerp (maxHeight, 0, (timeCost - 0.5f) * 2);
		length = Mathf.Lerp (startLength, targetLength, timeCost);

		if (timeCost >= 1) {
			timeOver = true;
//			Debug.Log (id + " Time Over. " + startTime + " / " + now);
		}
	}

}