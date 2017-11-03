using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Text2Video : MonoBehaviour
{
	public Text textTemplate;

	private VideoPlayer video;
	private string[] letterLuminance = {"。", "丨", "二", "干", "正", "百", "青", "聪"};

	private List<string> frameList;
	private int curFrame;

	void Start () 
	{
		curFrame = 0;
		textTemplate.gameObject.SetActive (true);
		read ();

		video = GetComponent<VideoPlayer> ();
		video.Play ();
	}

	void Update ()
	{
		if (curFrame != (int)video.frame) {
			curFrame = (int)video.frame;
			if ((int)video.frame <= frameList.Count - 1) {
				textTemplate.text = frameList [(int)video.frame];
			}
		}
	}

	private float getLuminance(Color color) {
		return 1f - (0.2125f * color.r + 0.7154f * color.g + 0.0721f * color.b); 
	}

	private void read()
	{
		string path = Application.dataPath + "/StreamingAssets";
		StreamReader sr = File.OpenText (path + "/VideoPlayer/ppap.txt");

		frameList = new List<string> ();

		string str;
		int curIndex = 0;
		while((str = sr.ReadLine() ) != null){
			if (str.Contains (",")) {
				string[] ss = str.Split (',');
				curIndex = int.Parse(ss [0]);
				frameList.Add (ss [1]);
			} else {
				frameList[curIndex] += "\n" + str;
			}
		}

		sr.Close ();
		sr.Dispose ();
	}

	public void pauseResume()
	{
		if (video.isPlaying) {
			video.Pause ();
		} else {
			video.Play ();
		}
	}

}
