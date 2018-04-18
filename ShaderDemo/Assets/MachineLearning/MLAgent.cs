using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class MLAgent : MonoBehaviour
{
	public MLEnviroment enviroment;
	public Text textFeatureCount_1;
	public Text textFeatureCount_2;
	public Text textFeatureCount_3;
	public Text textLine;
	public Text textCount;

	private List<List<MLDescription>> descriptions;
	private List<List<Vector2>> angles;
	private int curDescriptionIndex;
	private int curFeatureIndex;
	private float curActionAngle;
	private float award;

	private const float AWARD_RATE = .05f;
//	private float[] HEAVY_RATE = new float[]{.5f, .4f, .1f};
	private const bool FORMAL = true;

	void Awake()
	{
		descriptions = new List<List<MLDescription>> ();
		angles = new List<List<Vector2>> ();
		for (int i = 0; i < 3; i++) {
			descriptions.Add (new List<MLDescription>());
			angles.Add (new List<Vector2>());
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
		List<Vector2> curAngle = new List<Vector2> ();
		for (int i = 0; i < 3; i++) {
//			curAngle.Add (new Vector2(Random.Range(0, 360), Random.Range(0, 360)));
			curAngle.Add (new Vector2(0, 180));
		}
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
					curAngle[feature] = angles [feature] [i];

					curFeatureIndex = feature;
					curDescriptionIndex = i;
					break;
				}
			}

			if (!curHasDesc [feature]) {
				angles [feature].Add (curAngle [feature]);
				featureList.Add (curDesc);
			}
			curDescriptionIndex = curDescriptionIndex < 0 ? (-1 * (featureList.Count)) : curDescriptionIndex;
			curFeatureIndex = curFeatureIndex < 0 ? (-1 * (feature + 1)) : curFeatureIndex;
		}

		curDescriptionIndex = Mathf.Abs(curDescriptionIndex < 0 ? (curDescriptionIndex + 1) : curDescriptionIndex);
//		curFeatureIndex = Mathf.Abs (curFeatureIndex < 0 ? (curFeatureIndex + 1) : curFeatureIndex);

		// 产生随机行为
		float random = Random.value;
		if (curFeatureIndex < 0) {
			curActionAngle = random * 360;
			curFeatureIndex = 0;
			curDescriptionIndex = 0;
		} else if (random < .3f || FORMAL) {
			curActionAngle = curAngle [curFeatureIndex].x;
		} else if (random > .9f) {
			curActionAngle = curAngle [curFeatureIndex].y;
		} else if (Mathf.DeltaAngle (curAngle [curFeatureIndex].x, curAngle [curFeatureIndex].y) < 30f) {
			curActionAngle = Random.Range (0f, 360f);
		} else {
			random = random / .6f - .5f;
			bool flag = ((int)(random * 100)) % 2 == 0;
			if (flag) {
				curActionAngle = Mathf.LerpAngle (curAngle [curFeatureIndex].x, curAngle [curFeatureIndex].y, random);
			} else {
				curActionAngle = Mathf.LerpAngle (curAngle [curFeatureIndex].y + 360, curAngle [curFeatureIndex].x, random);
			}
		}

		transform.localEulerAngles = new Vector3 (curActionAngle, -90, 90);
		Vector3 targetPosition = transform.position + transform.forward * Time.deltaTime * 5;
		targetPosition.z = 0;

		if (targetPosition.magnitude < 30) {
			transform.position = targetPosition;
		} else {
			award = AWARD_RATE * -10;
//			curFeatureIndex = 0;
		}
	}

	public void Train(float award)
	{
		Vector2 angle = angles [curFeatureIndex] [curDescriptionIndex];

		if (award > 0) {
			angle.x = Mathf.LerpAngle (angle.x, curActionAngle, award);
		} else if (award < 0) {
			angle.y = Mathf.LerpAngle (angle.y, curActionAngle, Mathf.Abs(award));
		}

		angles [curFeatureIndex] [curDescriptionIndex] = angle;
	}

	private void Clean()
	{
		List<Vector2> list = new List<Vector2> ();
		for(int i = angles.Count - 1; i >= 0; i--){
			List<Vector2> ass = angles [i];
			for(int j = ass.Count - 2; j >= 0; j--){
				Vector2 a = ass [j];
				if (a.x == 0 && a.y == 180) {
					list.Add (new Vector2(i, j));
				}
			}
		}

		foreach(Vector2 ij in list){
			int i = (int)ij.x;
			int j = (int)ij.y;

			descriptions [i].RemoveAt (j);
			angles [i].RemoveAt (j);
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
//			curFeatureIndex = 2;
		} else {
			enviroment.resetTime ();
//			curFeatureIndex = 1;
		}

		node.Remove ();
	}

	public List<string> getData()
	{
		List<string> pairs = new List<string> ();
		int count = 0;
		for (int i = 0; i < descriptions.Count; i++) {
			List<MLDescription> list = descriptions [i];
			List<Vector2> angleList = angles [i];
			for (int j = 0; j < list.Count; j++) {
				MLDescription desc = list [j];
				Vector2 angle = angleList [j];

				addPairs (pairs, "angle_" + count, angle.x + "," + angle.y);
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

	public void setData(List<List<MLDescription>> _descriptions, List<List<Vector2>> _angles)
	{
		descriptions = _descriptions;
		angles = _angles;
	}

}