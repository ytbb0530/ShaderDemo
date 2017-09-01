using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WetController : MonoBehaviour 
{
	public MeshRenderer mashCloth;
	public MeshRenderer mashTop;
	public MeshRenderer mashPants;

	public Slider slider;

	private Texture2D wetTex;
	private float wetHeight = 0;
	private const float wetTrans = .2f;
	private float transTime;
	private const float timeTrans_Stay = 1f;
	private const float timeTrans_Change = 2f;

	void Start () 
	{
		wetTex = new Texture2D (8, 1024, TextureFormat.RGBA32, false);

		for(int x = 0; x < wetTex.width; x++){
			for(int y = 0; y < wetTex.height; y++){

				wetTex.SetPixel (x, y, Color.black);
			}
		}
		wetTex.Apply();

		mashCloth.material.SetTexture ("_WetTex", wetTex);
		mashTop.material.SetTexture ("_WetTex", wetTex);
		mashPants.material.SetTexture ("_WetTex", wetTex);
	}
	
	void Update () 
	{
		float timeRate = 0;
		if (Time.realtimeSinceStartup < transTime + timeTrans_Stay) {
			timeRate = 1;
		} else if (Time.realtimeSinceStartup > transTime + timeTrans_Stay + timeTrans_Change) {
			timeRate = 0;
		} else {
			timeRate = 1 - (Time.realtimeSinceStartup - transTime - timeTrans_Stay) * (1 / timeTrans_Change);
		}

		for(int x = 0; x < wetTex.width; x++){
			for(int y = 0; y < wetTex.height; y++){

				float rate = y * 1f / wetTex.height;

				if (rate < wetHeight) {
					wetTex.SetPixel (x, y, new Color(timeRate, 0, 0));
				}else if(rate > wetHeight + wetTrans){
					wetTex.SetPixel (x, y, Color.black);
				} else {
					float tr = 1 - (rate - wetHeight) * (1 / wetTrans);
					wetTex.SetPixel (x, y, new Color(tr * timeRate, 0, 0));
				}
			}
		}
		wetTex.Apply();
	}

	public void sliderValueChange()
	{
		wetHeight = slider.value;

		transTime = Time.realtimeSinceStartup;
	}

}
