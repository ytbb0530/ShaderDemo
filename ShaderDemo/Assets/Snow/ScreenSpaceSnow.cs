using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScreenSpaceSnow : MonoBehaviour
{
	public Material _material;

	public Texture2D SnowTexture;

	public Color SnowColor = Color.white;

	public float SnowTextureScale = 0.1f;

	[Range(0, 1)]
	public float BottomThreshold = .445f;


	private Text text;


	void OnEnable()
	{
		// dynamically create a material that will use our shader
		//_material = new Material(Shader.Find("TKoU/ScreenSpaceSnow"));

		// tell the camera to render depth and normals
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;

		Slider slider = GameObject.Find ("Slider").GetComponent<Slider>();

		slider.value = BottomThreshold;

		text = GameObject.Find ("TextRate").GetComponent<Text>();

		text.text = Round(slider.value, 2);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) 
	{
		// set shader properties
		_material.SetMatrix("_CamToWorld", GetComponent<Camera>().cameraToWorldMatrix);
		_material.SetColor("_SnowColor", SnowColor);
		_material.SetFloat("_BottomThreshold", 1 - BottomThreshold);
		_material.SetFloat("_TopThreshold", 1 - SnowColor.a);
		_material.SetTexture("_SnowTex", SnowTexture);
		_material.SetFloat("_SnowTexScale", SnowTextureScale);

		// execute the shader on input texture (src) and write to output (dest)
		Graphics.Blit(src, dest, _material);
	}

	public void setThreshold(float rate)
	{
		BottomThreshold = rate;

		text.text = Round(rate, 2);
	}

	private static string Round(float num, int acc)
	{
		string result = "";

		string str = num + "";

		if (str.Length == 1)
		{
			result = str + ".00";
		}
		else if(str.Length < acc + 2)
		{
			result = str + "0";
		}
		else
		{
			char[] charArray = str.ToCharArray (0, acc + 2);

			result = new string (charArray);
		}

		return result;
	}

}
