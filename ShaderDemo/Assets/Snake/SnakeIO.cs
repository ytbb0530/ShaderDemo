using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class SnakeIO : MonoBehaviour 
{
	private SnakeAgent agent;
	private int state;

	public void Init () 
	{
		SnakeMap map = GetComponent<SnakeMap> ();
		agent = map.snake;
		agent.gameObject.SetActive (true);
		state = 0;

		map.Init ();
		Load ();

		StartCoroutine (SaveClock ());
	}

	void Update ()
	{
		if(Mathf.Abs(state) == 1) agent.Pause ();
		if(Mathf.Abs(state) == 3) agent.Resume ();

		if (state == 2) SaveData ();
		if (state == -2) LoadData ();

		if (state != 0) {
			state += state > 0 ? 1 : -1;
			state = Mathf.Abs (state) > 3 ? 0 : state;
		}
	}

	private IEnumerator SaveClock()
	{
		while (SnakeMap.training) {
			Debug.Log ("Waiting for time to Save ...");
			yield return new WaitForSeconds (3600f);
			Save ();
		}
	}

	public void Save()
	{
		#if UNITY_EDITOR
		state = 1;
		#endif
	}

	public void Load()
	{
		state = -1;
	}

	private void SaveData()
	{
		List<string> pairs = agent.GetData ();
		Write (pairs);
		Debug.Log ("Save Data Completed " + System.DateTime.Now);
	}

	private void Write(List<string> pairs)
	{
		string path = "";
		#if UNITY_EDITOR
		path = Application.dataPath + "/StreamingAssets";
		#elif UNITY_IPHONE
		path = Application.streamingAssetsPath;
		#endif
		StreamWriter sw = File.CreateText(path + "/Snake/data.txt");//打开现有 UTF-8 编码文本文件以进行读取
		sw.WriteLine("Time:" + System.DateTime.Now);
		foreach (string str in pairs) {
			sw.WriteLine(str);
		}
		sw.Close ();  
		sw.Dispose ();//文件流释放 
	}

	private void LoadData()
	{
		List<string> pairs = read ();
		string timeStr = agent.SetData (pairs);
		if (timeStr.Length > 0) {
			Debug.Log ("Load Data From " + timeStr);
		} else {
			Debug.Log ("No Data To Load, Start A New Game.");
		}
	}

	private List<string> read()
	{
		List<string> pairs = new List<string> ();

		string path = "";
		#if UNITY_EDITOR
		path = Application.dataPath + "/StreamingAssets";
		#elif UNITY_IPHONE
		path = Application.streamingAssetsPath;
		#endif
		StreamReader sr = File.OpenText (path + "/Snake/data.txt");

		string str;
		while((str = sr.ReadLine() ) != null){
			pairs.Add(str);
		}

		sr.Close ();
		sr.Dispose ();
		return pairs;
	}

}