using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class MLEnviroment : MonoBehaviour 
{
	public static bool inPause{ private set; get;}

	public MLAgent player;
	public GameObject enemyTemplate;
	public GameObject awardTemplate;
	public Text textTime;
	public Text textMax;
	public Slider slider;
	public Text textTimeScale;
	public List<MLNode> nodes = new List<MLNode>();

	private float tempTime;
	private float maxTime;
	private float timeScale = 20f;

	void Start () 
	{
		tempTime = Time.time;
		maxTime = 0;
		nodes.Clear ();
		StartCoroutine (createEnviromentNode());

		slider.value = 1;
		setTimeScale ();
	}
	
	void Update () 
	{
		float timeCost = Time.time - tempTime;
		textTime.text = timeCost.ToString ("f2");

		if (timeCost > maxTime) {
			maxTime = timeCost;
			textMax.text = (int)maxTime + "";
		}
	}

	IEnumerator createEnviromentNode()
	{
		yield return new WaitForSeconds (1f);

		for(;;){
			float random = Random.value;
			int type = random > .8f ? 1 : -1;
			GameObject template = type == 1 ? awardTemplate : enemyTemplate;
		
			Vector2 randomPoint = Random.insideUnitCircle;
			randomPoint = randomPoint.normalized * 50;

			GameObject gc = Instantiate(template) as GameObject;
			gc.SetActive (true);
			gc.transform.position = new Vector3 (randomPoint.x, randomPoint.y, 0);

			Vector2 forward2D = Random.insideUnitCircle * 50;
			Vector3 forward = new Vector3(forward2D.x, forward2D.y, 0) - gc.transform.position;
			gc.transform.forward = new Vector3 (forward.x, forward.y, 0);

			MLNode node = gc.AddComponent<MLNode> ();
			node.type = type;
			node.speed = Random.Range (8f, 14f);
			node.enviroment = this;
			nodes.Add (node);

			yield return new WaitForSeconds (.2f);
		}
	}

	public void removeNode(MLNode node)
	{
		DestroyObject (node.gameObject);
		nodes.Remove (node);
	}

	public void resetTime()
	{
		tempTime = Time.time;
	}

	public void addTime()
	{
		tempTime -= 2f;
	}

	public void pause()
	{
		Time.timeScale = 0;
		inPause = true;
	}

	public void resume()
	{
		Time.timeScale = timeScale;
		inPause = false;
	}

	public void setTimeScale()
	{
		timeScale = slider.value * 19f + 1f;
		Time.timeScale = timeScale;
		textTimeScale.text = timeScale.ToString ("f1");
	}

}