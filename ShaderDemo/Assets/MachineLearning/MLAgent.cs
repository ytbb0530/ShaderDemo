using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class MLAgent : MonoBehaviour
{
	public enum ACTION {
		STAY,
		UP,
		DOWN,
		LEFT,
		RIGHT,
	}

	public MLEnviroment enviroment;
	public Text textFeatureCount_1;
	public Text textFeatureCount_2;
	public Text textFeatureCount_3;
	public Text textLine;
	public Text textCount;

	private List<List<MLDescription>> descriptions;
	private List<List<float[]>> scores;
	private ACTION curAction;
	private int curDescriptionIndex;
	private int curFeatureIndex;
	private float award;

	private const float AWARD_RATE = 20f;
	private float[] HEAVY_RATE = new float[]{.5f, .4f, .1f};

	void Awake()
	{
		descriptions = new List<List<MLDescription>> ();
		scores = new List<List<float[]>> ();
		for (int i = 0; i < 3; i++) {
			descriptions.Add (new List<MLDescription>());
			scores.Add (new List<float[]> ());
		}
	}

	void Update()
	{
		if (MLEnviroment.inPause) return;

		DoAction ();
		Train (award);
		Clean ();
		award = AWARD_RATE;

		string s1 = "" + descriptions [0].Count;
		string s2 = "" + descriptions [1].Count;
		string s3 = "" + descriptions [2].Count;
		string st = "" + (descriptions [0].Count + descriptions [1].Count + descriptions [2].Count);
		string sl = "";

		int diff = st.Length - s3.Length;
		diff = diff < 1 ? 1 : diff;
		string space = "";
		for (int i = 0; i < diff; i++) {
			space += "  ";
		}
		s3 = "+" + space + s3;

		for (int i = 0; i < st.Length; i++) {
			sl += "—";
		}

		textFeatureCount_1.text = s1;
		textFeatureCount_2.text = s2;
		textFeatureCount_3.text = s3;
		textLine.text = sl;
		textCount.text = st;
	}

	private void DoAction()
	{
		float[] defaultScore = new float[]{ 1000, 1000, 1000, 1000, 1000 };
		float[][] curScores = new float[][]{defaultScore, defaultScore, defaultScore};
		bool[] curHasDesc = new bool[]{false, false, false};
		MLDescription[] curDescs = MLDescription.getCurDescription (enviroment);
		curFeatureIndex = -1;
		curDescriptionIndex = -1;

		// 对比之前的经验
		for(int feature = 0; feature < 3; feature++){
			MLDescription curDesc = curDescs [feature];
			List<MLDescription> featureList = descriptions [feature];
			for(int i = 0; i < featureList.Count; i++) {
				MLDescription desc = featureList [i];
				int approximate = curDesc.Approximate (desc);
				if (approximate == feature) {
					curHasDesc [feature] = true;
					curScores[feature] = scores [feature] [i];
					curFeatureIndex = feature;
					curDescriptionIndex = i;
					break;
				}
			}

			if (!curHasDesc [feature]) {
				scores [feature].Add (curScores [feature]);
				featureList.Add (curDesc);
			}
			curDescriptionIndex = curDescriptionIndex < 0 ? (-1 * (featureList.Count)) : curDescriptionIndex;
			curFeatureIndex = curFeatureIndex < 0 ? (-1 * (feature + 1)) : curFeatureIndex;
		}

		curDescriptionIndex = Mathf.Abs(curDescriptionIndex < 0 ? (curDescriptionIndex + 1) : curDescriptionIndex);
		curFeatureIndex = Mathf.Abs (curFeatureIndex < 0 ? (curFeatureIndex + 1) : curFeatureIndex);

		// 综合之前的经验
		float[] curScore = new float[]{ -1, -1, -1, -1, -1 };
		for(int feature = 0; feature < 3; feature++){
			bool has = curHasDesc [feature];
			if (has) {
				for(int i = 0; i < curScore.Length; i++){
					if (curScore [i] == -1) {
						curScore [i] = curScores [feature] [i];
					} else {
						curScore [i] = Mathf.Lerp (curScores [feature] [i], curScore[i], HEAVY_RATE[feature]);
					}
				}
			}
		}

		// 产生随机行为
		float range = 0;
		for(int i = 0; i <= (int)ACTION.RIGHT; i++){
			float total = 0;
			for(int j = 0; j <= (int)ACTION.RIGHT; j++){
				if (i != j) {
					total += curScore [j] < 1 ? 1 : curScore [j];
				}
			}
			if (curScore [i] > total * 10) {
				range = -1;
				curAction = (ACTION)i;
				break;
			}

			range += curScore [i] < 1 ? 1 : curScore [i];
		}

		if (range > 0) {
			float random = Random.Range (0, range);
			range = 0;
			for (int i = 0; i <= (int)ACTION.RIGHT; i++) {
				range += curScore [i] < 1 ? 1 : curScore [i];
				if (random < range) {
					curAction = (ACTION)i;
					break;
				}
			}
		}

		Vector3 speed = Vector3.zero;
		if (curAction == ACTION.STAY) {
		} else if (curAction == ACTION.UP) {
			speed = Vector3.up;
		} else if (curAction == ACTION.DOWN) {
			speed = Vector3.down;
		} else if (curAction == ACTION.LEFT) {
			speed = Vector3.left;
		} else if (curAction == ACTION.RIGHT) {
			speed = Vector3.right;
		}

		Vector3 targetPosition = transform.position + speed * Time.deltaTime * 5;
		if (targetPosition.magnitude < 30) {
			transform.position = targetPosition;
		} else {
			award = AWARD_RATE * -10;
		}
	}

	public void Train(float award)
	{
		scores [curFeatureIndex] [curDescriptionIndex] [(int)curAction] += award;
	}

	private void Clean()
	{
		List<Vector2> list = new List<Vector2> ();
		for(int i = scores.Count - 1; i >= 0; i--){
			List<float[]> ss = scores [i];
			for(int j = ss.Count - 1; j >= 0; j--){
				float[] s = ss [j];
				if (s [0] == 1000 && s [1] == 1000 && s [2] == 1000 && s [3] == 1000 && s [4] == 1000) {
					list.Add (new Vector2(i, j));
				}
			}
		}

		foreach(Vector2 ij in list){
			int i = (int)ij.x;
			int j = (int)ij.y;

			descriptions [i].RemoveAt (j);
			scores [i].RemoveAt (j);
			if (descriptions [i].Count == 0) {
				descriptions.RemoveAt (i);
				scores.RemoveAt (i);
			}
		}
	}

	public void reset()
	{
		transform.position = Vector3.zero;
		transform.eulerAngles = new Vector3 (-90, 0, 0);
	}

	public void OnTriggerEnter(Collider other)
	{
		MLNode node = other.GetComponent<MLNode> ();
		if (node == null) {
			return;
		}

		award = node.type * AWARD_RATE;
		if (node.type == 1) {
			enviroment.addTime ();
		} else {
			enviroment.resetTime ();
		}
		// Train (reward);

		node.Remove ();
	}

	public List<string> getData()
	{
		List<string> pairs = new List<string> ();
		int count = 0;
		for (int i = 0; i < descriptions.Count; i++) {
			List<MLDescription> list = descriptions [i];
			List<float[]> scoreList = scores [i];
			for (int j = 0; j < list.Count; j++) {
				MLDescription desc = list [j];
				float[] score = scoreList [j];

				addPairs (pairs, "score_" + count, score[0] + "," + score[1] + "," + score[2] + "," + score[3] + "," + score[4]);
				addPairs (pairs, "feature_" + count, "" + desc.getFeature ());

				if (desc.getFeature () == 0) {
					Vector2 p = desc.getPlayerPosition ();
					addPairs (pairs, "p_" + count, p.x + "," + p.y);
				} else {
					List<Vector2> listN = desc.getNearNodes ();
					List<Vector2> listF = desc.getNearForwards ();
					for (int h = 0; h < listN.Count; h++) {
						Vector3 pos = listN [h];
						addPairs (pairs, "n_" + count + "_" + h, pos.x + "," + pos.y);
					}
					for (int h = 0; h < listF.Count; h++) {
						Vector3 pos = listF [h];
						addPairs (pairs, "f_" + count + "_" + h, pos.x + "," + pos.y);
					}
				}

				count++;
			}
		}
		return pairs;
	}

	private void addPairs(List<string> pairs, string key, string value)
	{
		pairs.Add (key + ":" + value);
	}

	public void setData(List<List<MLDescription>> _descriptions, List<List<float[]>> _scores)
	{
		descriptions = _descriptions;
		scores = _scores;
	}

}