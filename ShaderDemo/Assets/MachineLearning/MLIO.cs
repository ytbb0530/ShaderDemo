using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class MLIO : MonoBehaviour 
{
	private MLEnviroment enviroment;
	private MLAgent agent;

	void Start () 
	{
		enviroment = GetComponent<MLEnviroment> ();
		agent = enviroment.player;
		LoadData ();

//		StartCoroutine (SaveClock());
	}

	private IEnumerator SaveClock()
	{
		while (true) {
			yield return new WaitForSeconds (3600f);
			SaveData ();
		}
	}

	public void SaveData ()
	{
		enviroment.pause();
		List<string> pairs = agent.getData ();
		write (pairs);
		Debug.Log ("Save Data Completed " + System.DateTime.Now);
		enviroment.resume();
	}

	public void LoadData()
	{
		enviroment.pause();
		List<string> pairs = read ();
		setData (pairs);
		enviroment.resume();

		StartCoroutine (SaveClock());
	}

	private void write(List<string> pairs)
	{
		StreamWriter sw = File.CreateText(Application.dataPath + "/MachineLearning/data.txt");//打开现有 UTF-8 编码文本文件以进行读取
		foreach (string str in pairs) {
			sw.WriteLine(str);
		}
		sw.Close ();  
		sw.Dispose ();//文件流释放 
	}

	private List<string> read()
	{
		List<string> pairs = new List<string> ();
		StreamReader sr = File.OpenText (Application.dataPath + "/MachineLearning/data.txt");

		string str;
		while((str = sr.ReadLine() ) != null){
			pairs.Add(str);
		}

		sr.Close ();
		sr.Dispose ();
		return pairs;
	}

	private void setData(List<string> pairs)
	{
		Dictionary<int, Vector2> dictAngle = new Dictionary<int, Vector2> ();

		Dictionary<int, int> dictFeature = new Dictionary<int, int> ();
		Dictionary<int, Vector2> dictPlayer = new Dictionary<int, Vector2> ();
		Dictionary<int, Dictionary<int, Vector2>> dictN = new Dictionary<int, Dictionary<int, Vector2>> ();
		Dictionary<int, Dictionary<int, Vector2>> dictF = new Dictionary<int, Dictionary<int, Vector2>> ();

		for(int i = 0; i < pairs.Count; i++){
			string[] pair = pairs[i].Split (':');
			string key = pair [0];
			string value = pair [1];

			string[] keySplit = key.Split ('_');
			string keyType = keySplit [0];
			int count = int.Parse(keySplit [1]);

			if (keyType.Equals ("angle")) {
				string[] angleStrs = value.Split (',');
				Vector2 ss = new Vector2(float.Parse (angleStrs [0]), float.Parse (angleStrs [1]));
				dictAngle.Add (count, ss);
			} else if (keyType.Equals ("feature")) {
				dictFeature.Add (count, int.Parse (value));
			} else if (keyType.Equals ("p")) {
				string[] strP = value.Split (',');
				dictPlayer.Add (count, new Vector2(float.Parse(strP[0]), float.Parse(strP[1])));
			} else if (keyType.Equals ("n")) {
				Dictionary<int, Vector2> dict;
				if (dictN.ContainsKey (count)) {
					dict = dictN [count];
				} else {
					dict = new Dictionary<int, Vector2> ();
					dictN.Add (count, dict);
				}

				int h = int.Parse (keySplit [2]);
				string[] strN = value.Split (',');
				dict.Add (h, new Vector2(float.Parse(strN[0]), float.Parse(strN[1])));
			} else if (keyType.Equals ("f")) {
				Dictionary<int, Vector2> dict;
				if (dictF.ContainsKey (count)) {
					dict = dictF [count];
				} else {
					dict = new Dictionary<int, Vector2> ();
					dictF.Add (count, dict);
				}

				int h = int.Parse (keySplit [2]);
				string[] strF = value.Split (',');
				dict.Add (h, new Vector2(float.Parse(strF[0]), float.Parse(strF[1])));
			}
		}

		List<List<MLDescription>> descriptions = new List<List<MLDescription>> ();
		descriptions.Add (new List<MLDescription>());
		descriptions.Add (new List<MLDescription>());
		descriptions.Add (new List<MLDescription>());

		List<List<Vector2>> angles = new List<List<Vector2>>();
		angles.Add (new List<Vector2>());
		angles.Add (new List<Vector2>());
		angles.Add (new List<Vector2>());

		foreach (int key in dictFeature.Keys) {
			int feature = dictFeature[key];
			MLDescription desc = null;

			if (feature == 0) {
				Vector2 player = dictPlayer [key];
				desc = new MLDescription (feature, _playerPosition: player);
			} else {
				
				Dictionary<int, Vector2> n = dictN.ContainsKey(key) ? dictN [key] : new Dictionary<int, Vector2>();
				Dictionary<int, Vector2> f = dictF.ContainsKey(key) ? dictF [key] : new Dictionary<int, Vector2>();

				List<Vector2> ns = new List<Vector2> ();
				List<Vector2> fs = new List<Vector2> ();

				foreach (int nkey in n.Keys) {
					ns.Add (n [nkey]);
					fs.Add (f [nkey]);
				}

				desc = new MLDescription (feature, _nearNodes:ns, _nearForwards:fs);
			}

			descriptions [feature].Add (desc);
			angles [feature].Add (dictAngle[key]);
		}

		agent.setData (descriptions, angles);
	}

}
