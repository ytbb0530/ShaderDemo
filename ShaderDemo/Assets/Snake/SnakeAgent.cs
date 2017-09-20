using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SnakeAgent : MonoBehaviour 
{
	private enum Action{
		UP,
		DOWN,
		LEFT,
		RIGHT,
	}

	public SnakeMap map;
	public GameObject snakeUnitTemple;
	public Text textCornerCount;
	public Text textHeadCount;
	public Text textFoodCount;
	public Text textStep;
	public Text textScore;

	public SnakeVector[] snakePositions{get{ return positions.ToArray ();}}

	private int length;
	private bool pause;
	private List<SnakeVector> positions;
	private List<GameObject> snakeUnits;
	private SnakeVector headPos{ get{ return positions [0];}}
	private float tempTime;
	private float deltaTime;
	private List<SnakeCornerDescription> cornerDescriptions;
	private List<SnakeHeadDescription> headDescriptions;
	private List<SnakeFoodDescription> foodDescriptions;
	private SnakeHeadDescription curHeadDesc;
	private SnakeCornerDescription curCornerDesc;
	private SnakeFoodDescription curFoodDesc;
	private SnakeStatus curStatus;
	private bool inRecord;
	private int curStep;
	private int maxStep;
	private int curScore;
	private int maxScore;

	void Awake()
	{
		length = 6;
		curStep = 0;
		maxStep = 0;
		curScore = 0;
		maxScore = 0;
		deltaTime = SnakeMap.training ? 0 : .2f;
		pause = false;

		snakeUnits = new List<GameObject> ();
		positions = new List<SnakeVector> ();
		cornerDescriptions = new List<SnakeCornerDescription> ();
		headDescriptions = new List<SnakeHeadDescription> ();
		foodDescriptions = new List<SnakeFoodDescription> ();
	}

	public void Init (SnakeStatus status = null)
	{
		tempTime = Time.time;
		inRecord = status != null;
		curStep = status == null ? 0 : status.step;
		curScore = status == null ? 0 : status.score;

		foreach (GameObject unitGc in snakeUnits) {
			DestroyObject (unitGc);
		}
		snakeUnits.Clear ();
		positions.Clear ();

		if (status != null) {
			foreach (SnakeVector pos in status.snakePosition) {
				positions.Add (pos);
			}
		} else {
			positions.Add (new SnakeVector (10, 10));
			float rand = Random.value;
			for (int i = 1; i < length; i++) {
				if (rand < .25f)
					positions.Add (headPos + new SnakeVector (0, i));
				else if (rand < 5f)
					positions.Add (headPos + new SnakeVector (0, -i));
				else if (rand < .75f)
					positions.Add (headPos + new SnakeVector (i, 0));
				else
					positions.Add (headPos + new SnakeVector (-i, 0));
			}
		}

		for (int i = 0; i < positions.Count; i++) {
			GameObject gc = addSnakeUnitObject (positions [i]);
			if (i == 0) {
				gc.GetComponent<MeshRenderer> ().material.SetColor ("_Color", Color.black);
			}
		}

		map.createFood ();
	}

	private GameObject addSnakeUnitObject(SnakeVector pos)
	{
		GameObject gc = GameObject.Instantiate (snakeUnitTemple);
		gc.SetActive (true);
		gc.transform.parent = transform;
		gc.transform.localScale = snakeUnitTemple.transform.localScale;
		gc.transform.localPosition = new Vector3 (pos.x, pos.y, 0);
		snakeUnits.Add (gc);

		return gc;
	}

	public void Pause()
	{
		pause = true;
		Time.timeScale = 0;
	}

	public void Resume()
	{
		pause = false;
		Time.timeScale = 1;
	}

	void Update ()
	{
		if (pause) return;

		if (Time.time - tempTime < deltaTime) return;
		tempTime = Time.time;

		// record cur step
		if (!inRecord) {
			SnakeStatus status = new SnakeStatus (curStatus);
			foreach (SnakeVector pos in positions) {
				status.snakePosition.Add (pos);
			}
			status.foodPosition = map.foodPos;
			status.step = curStep;
			status.score = curScore;
			curStatus = status;
		}
		inRecord = false;

		// get action and move
		Action curAction = getCurAction ();
		curStatus.used [(int)curAction] = true;
		move (curAction);

		// show imformations
		textCornerCount.text = cornerDescriptions.Count + " C";
		textHeadCount.text = headDescriptions.Count + " H";
		textFoodCount.text = foodDescriptions.Count + " F";
		textStep.text = (SnakeMap.training ? maxStep : curStep) + "";
		textScore.text = (SnakeMap.training ? maxScore : curScore) + "";
	}

	private Action getCurAction()
	{
		SnakeDescription.GetCurDescription (this, map, out curHeadDesc, out curCornerDesc, out curFoodDesc);

		float[] curHeadScore = new float[]{0, 0, 0, 0};
		float[] curCornerScore = new float[]{0, 0, 0, 0};
		float[] curFoodScore = new float[]{0, 0, 0, 0};

		bool hasHeadDesc = false;
		bool hasCornerDesc = false;
		bool hasFoodDesc = false;

		foreach(SnakeHeadDescription headDesc in headDescriptions){
			if (headDesc == curHeadDesc) {
				curHeadDesc = headDesc;
				curHeadScore = headDesc.score;
				hasHeadDesc = true;
				break;
			}
		}
		if (!hasHeadDesc) {
			headDescriptions.Add (curHeadDesc);
		}

		foreach (SnakeCornerDescription cornerDesc in cornerDescriptions) {
			if (cornerDesc == curCornerDesc) {
				curCornerDesc = cornerDesc;
				curCornerScore = cornerDesc.score;
				hasCornerDesc = true;
				break;
			}
		}
		if (!hasCornerDesc) {
			cornerDescriptions.Add (curCornerDesc);
		}

		foreach (SnakeFoodDescription foodDesc in foodDescriptions) {
			if (foodDesc == curFoodDesc) {
				curFoodDesc = foodDesc;
				curFoodScore = foodDesc.score;
				hasFoodDesc = true;
				break;
			}
		}
		if (!hasFoodDesc) {
			foodDescriptions.Add (curFoodDesc);
		}

		float[] curScore = SnakeDescription.getSocre (curHeadScore, curCornerScore, curFoodScore);
		if (map.foodPos.x > headPos.x) {
			curScore [(int)Action.RIGHT] += 10;
		} else if (map.foodPos.x < headPos.x) {
			curScore [(int)Action.LEFT] += 10;
		}
		if (map.foodPos.y > headPos.y) {
			curScore [(int)Action.UP] += 10;
		} else if (map.foodPos.y < headPos.y) {
			curScore [(int)Action.DOWN] += 10;
		}

		Action curAction = Action.UP;
		if (!SnakeMap.training) {
			float max = -9999999;
			for (int i = 0; i < curScore.Length; i++) {
				float cur = curScore [i];
				if (cur > max) {
					max = cur;
					curAction = (Action)i;
				}
			}

			return curAction;
		}

		float min = 9999;
		float total = 0;
		for (int i = 0; i < curScore.Length; i++) {
			if (curStatus.used [i] == true) continue;
			float score = curScore[i];
			if (score < min) min = score;
			total += score;
		}

		if (min < 1) {
			total = 0;
			for (int i = 0; i < curScore.Length; i++) {
				if (curStatus.used [i] == true) continue;
				curScore [i] += 1 + Mathf.Abs(min);
				total += curScore [i];
			}
			min = 1;
		}

		float rand = Random.Range (min, total);
		total = 0;
		for (int i = 0; i < curScore.Length; i++) {
			if (curStatus.used [i] == true) continue;
			total += curScore [i];
			if (rand <= total) {
				curAction = (Action)i;
				break;
			}
		}

		return curAction;
	}

	private void move(Action forward)
	{
		// 移动
		SnakeVector pos = SnakeVector.zero;
		if (forward == Action.DOWN) {
			pos = new SnakeVector (0, -1);
		} else if (forward == Action.UP) {
			pos = new SnakeVector (0, 1);
		} else if (forward == Action.LEFT) {
			pos = new SnakeVector (-1, 0);
		} else if (forward == Action.RIGHT) {
			pos = new SnakeVector (1, 0);
		}

		SnakeVector endPos = positions [positions.Count - 1];
		for(int i = positions.Count - 1; i > 0; i--){
			positions [i] = positions [i - 1];
		}
		positions [0] += pos;

		for(int i = 0; i < positions.Count; i++){
			snakeUnits [i].transform.localPosition = new Vector3 (positions[i].x, positions[i].y, 0);
		}

		// 没死奖励
		float award = 1;

		// 出界惩罚
		if(headPos.x < 0 || headPos.x >= map.mapWidth || headPos.y < 0 || headPos.y >= map.mapHeight){
			award = -1;
			curHeadDesc.score [(int)forward] += award;
		}

		// 碰撞惩罚
		for(int i = positions.Count - 1; i > 0; i--){
			if (headPos == positions [i]) {
				award = -1;
				curCornerDesc.score [(int)forward] += award;
				break;
			}
		}

		// 吃食物奖励
		if (headPos.x == map.foodPos.x && headPos.y == map.foodPos.y) {
			// 限定最长为10
			if (positions.Count < 10) {
				if (SnakeMap.training) {
					// 训练模式中，200个食物长一截
					if (curScore % 200 == 0) {
						positions.Add (endPos);
						addSnakeUnitObject (endPos);
					}
				} else {
					positions.Add (endPos);
					addSnakeUnitObject (endPos);
				}
			}
			curFoodDesc.score [(int)forward] += 10;
			map.createFood ();
			addScore ();
		}

		if (award < 0) {
			if (SnakeMap.training) previousStep ();
			else Init ();
		} else {
			addStep ();
		}
	}

	private void addStep()
	{
		curStep++;
		maxStep = curStep > maxStep ? curStep : maxStep;
	}

	private void addScore()
	{
		curScore++;
		maxScore = curScore > maxScore ? curScore : maxScore;
	}

	private void previousStep()
	{
		SnakeStatus status = curStatus.previous;
		if (status == null) {
			Init ();
			return;
		}

		curStatus = status;
		Init (status);
		map.createFood (status);

		bool allused = true;
		for (int i = 0; i < curStatus.used.Length; i++) {
			if (curStatus.used [i] == false) {
				allused = false;
				break;
			}
		}
		if (allused) {
			previousStep ();
		}
	}

	public List<string> GetData()
	{
		List<string> strList = new List<string> ();

		for(int i = 0; i < cornerDescriptions.Count; i++){
			addString (strList, "corner_" + i, cornerDescriptions[i].GetData());
		}
		for(int i = 0; i < headDescriptions.Count; i++){
			addString (strList, "head_" + i, headDescriptions[i].GetData());
		}
		for(int i = 0; i < foodDescriptions.Count; i++){
			addString (strList, "food_" + i, foodDescriptions[i].GetData());
		}
		return strList;
	}

	private void addString(List<string> list, string key, string value)
	{
		list.Add (key + ":" + value);
	}

	public string SetData(List<string> data)
	{
		string timeStr = "";

		cornerDescriptions.Clear ();
		headDescriptions.Clear ();
		foodDescriptions.Clear ();

		for (int i = 0; i < data.Count; i++) {
			string str = data [i];
			string[] strs = str.Split (':');
			string key = strs [0];

			if (key.Equals ("Time")) {
				for (int j = 1; j < strs.Length; j++) {
					timeStr += strs[j];
					if (j != strs.Length - 1) {
						timeStr += ":";
					}
				}
				continue;
			}

			string value = strs [1];
			strs = value.Split ('_');
			value = strs [0];

			string score = strs [1];
			string[] strScores = score.Split (',');
			float[] scores = new float[strScores.Length];
			for (int j = 0; j < strScores.Length; j++) {
				scores [j] = float.Parse (strScores[j]);
			}

			if (key.StartsWith ("corner_")) {
				SnakeCornerDescription cornerDesc = new SnakeCornerDescription ();
				cornerDesc.score = scores;

				string[] corners = value.Split ('|');
				for(int j = 0; value.Length > 0 && j < corners.Length; j++){
					string strPoint = corners [j];
					string[] ps = strPoint.Split (',');
					cornerDesc.corner.Add (new SnakeVector(int.Parse(ps[0]), int.Parse(ps[1])));
				}
				cornerDescriptions.Add (cornerDesc);
			} else if (key.StartsWith ("head_")) {
				string[] ps = value.Split (',');
				SnakeHeadDescription headDesc = new SnakeHeadDescription (new SnakeVector(int.Parse(ps[0]), int.Parse(ps[1])));
				headDesc.score = scores;
				headDescriptions.Add (headDesc);
			} else if (key.StartsWith ("food_")) {
				string[] ps = value.Split (',');
				SnakeFoodDescription foodDesc = new SnakeFoodDescription (new SnakeVector(int.Parse(ps[0]), int.Parse(ps[1])));
				foodDesc.score = scores;
				foodDescriptions.Add (foodDesc);
			}
		}

		return timeStr;
	}

}