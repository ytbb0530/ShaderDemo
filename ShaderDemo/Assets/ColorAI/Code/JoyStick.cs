using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

	public delegate void StickDelegate (Vector2 offset);


	public GameObject btn;

	public StickDelegate callback;


	private  int curPointerId;

	private RectTransform canvas;

	private RectTransform rectTransform;


	void Start () 
	{
		curPointerId = -100;

		for(;;) {
			Canvas c = transform.parent.gameObject.GetComponent<Canvas> ();
			if (c != null) {
				canvas = c.gameObject.GetComponent<RectTransform> ();
				break;
			}
		}

		rectTransform = gameObject.GetComponent<RectTransform> ();
	}

	public void setCallback(StickDelegate _callback)
	{
		callback = _callback;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if(curPointerId != -100) return;

		curPointerId = eventData.pointerId;
	
		setPosition (eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.pointerId != curPointerId) return;

		btn.transform.localPosition = Vector2.zero;

		curPointerId = -100;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.pointerId != curPointerId) return;
	
		setPosition (eventData);
	}

	private void setPosition(PointerEventData eventData)
	{
		Vector2 pos = new Vector2();
		bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, eventData.position, eventData.enterEventCamera, out pos);
		if (isRect) {
			Vector2 tPos = pos - new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y);
			if(tPos.magnitude > 100) {
				tPos = tPos.normalized * 100;
			}
			btn.transform.localPosition = tPos;
		}
	}

	void FixedUpdate () 
	{
		if(callback!= null) {
			callback (btn.transform.localPosition);
		}
	}

}