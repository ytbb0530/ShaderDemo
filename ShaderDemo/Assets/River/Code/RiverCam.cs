using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class RiverCam : MonoBehaviour 
{
	public Camera mirroCam;

	public MeshRenderer river;

	public Cubemap cubemap;


	private Camera cam{get{return GetComponent<Camera> (); }}


	void OnEnable()
	{
		setRendererTexture ();
	}

	void setRendererTexture()
	{
		float w, h;

		if (Screen.width == 0 || Screen.height == 0)
		{
			return;	
		}else if (Screen.width > Screen.height) {
			w = 512;
			h = 512 * Screen.height / Screen.width;
		} else {
			h = 512;
			w = 512 * Screen.width / Screen.height;
		}

		mirroCam.targetTexture = new RenderTexture ((int)w, (int)h, 16);
		
		river.sharedMaterial.SetTexture ("_MirroTex", mirroCam.targetTexture);
	}

	void Update()
	{
		mirroCam.fieldOfView = cam.fieldOfView;

		mirroCam.transform.position = new Vector3(transform.position.x, river.transform.position.y - transform.position.y + river.transform.position.y, transform.position.z);

		Vector3 shadowPoint = new Vector3 (transform.position.x, river.transform.position.y, transform.position.z);

		float lenthY = Vector3.Distance (transform.position, shadowPoint);

		float angle = Vector3.Angle (transform.forward, new Vector3(0, -1, 0));

		float lenth = lenthY / Mathf.Cos (angle * Mathf.Deg2Rad);

		Vector3 tagetPosition = transform.position + transform.forward * lenth;

		mirroCam.transform.forward = tagetPosition - mirroCam.transform.position;
	}

}