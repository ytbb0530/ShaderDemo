using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Video2Text : MonoBehaviour
{
	public RenderTexture renderTexture;

	private VideoPlayer video;
	private Texture2D tex;
	private string[] letterLuminance = {"。", "丨", "二", "干", "正", "百", "青", "聪"};

	private List<string> frameList;

	void Start () 
	{
		video = GetComponent<VideoPlayer> ();
		video.Prepare();

		tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
		frameList = new List<string> ();
	}

	bool flag = false;

	void Update ()
	{
		if (flag) return;

		video.frame++;
		Debug.Log (gameObject.name + ": " + video.frame + " / " + video.frameCount);
		if (video.frame >= (long)video.frameCount - 5) {
			flag = true;
			Debug.Log ("Video over");
			write ();
		}

		int width = renderTexture.width;
		int height = renderTexture.height;
		RenderTexture.active = renderTexture;
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();

		string frameContent = "";
		for (int y = tex.height - 1; y >= 0; y--) {
			for (int x = 0; x < tex.width; x++) {
				Color col = tex.GetPixel (x, y);
				float lumi = getLuminance(col);
				int index = (int)(lumi * (letterLuminance.Length - 1));

				frameContent += letterLuminance[index];
			}
			frameContent += "\n";
		}

		if (frameList.Count < video.frame + 1) {
			frameList.Add (frameContent);
		}
	}

	private float getLuminance(Color color) {
		return 1f - (0.2125f * color.r + 0.7154f * color.g + 0.0721f * color.b); 
	}

	private void write()
	{
		string path = Application.dataPath + "/StreamingAssets";
		StreamWriter sw = File.CreateText(path + "/VideoPlayer/ppap.txt");//打开现有 UTF-8 编码文本文件以进行读取

		for (int frame = 0; frame < frameList.Count; frame++) {
			sw.WriteLine(frame + "," + frameList[frame]);
		}

		sw.Close ();  
		sw.Dispose ();//文件流释放 
		Debug.Log("Write Over");
	}

}
