using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SnakeVector
{
	public static SnakeVector zero{get{ return new SnakeVector (0, 0);}}

	public int x;
	public int y;

	public SnakeVector(int _x, int _y)
	{
		x = _x;
		y = _y;
	}

	public SnakeVector(Vector3 _pos)
	{
		x = (int)_pos.x;
		y = (int)_pos.y;
	}

	public bool Equals(SnakeVector other)
	{
		return other.x == x && other.y == y;
	}

	/// <summary>
	/// Sames the line.
	/// Return -1 for differentxy.
	/// Return 0 for same point.
	/// Return 1 for same x. Return 2 for same y.
	/// </summary>
	public int SameLine(SnakeVector other)
	{
		if (Equals (other))
			return 0;
		if (x == other.x)
			return 1;
		if (y == other.y)
			return 2;
		return -1;
	}

	public static bool operator ==(SnakeVector a, SnakeVector b)
	{
		return a.Equals (b);
	}

	public static bool operator !=(SnakeVector a, SnakeVector b)
	{
		return !(a == b);
	}

	public static SnakeVector operator +(SnakeVector a, SnakeVector b)
	{
		return new SnakeVector (a.x + b.x, a.y + b.y);
	}

	public static SnakeVector operator -(SnakeVector a, SnakeVector b)
	{
		return new SnakeVector (a.x - b.x, a.y - b.y);
	}

}

/// <summary>
/// 贪吃蛇状态描述，链表结构
/// </summary>
public class SnakeStatus
{
	public SnakeStatus previous;
	public SnakeStatus next;

	public List<SnakeVector> snakePosition;
	public SnakeVector foodPosition;
	public bool[] used;
	public int step;
	public int score;

	public SnakeStatus(SnakeStatus _pre)
	{
		previous = _pre;
		if (previous != null) {
			previous.next = this;
		}

		SnakeStatus p = previous;
		for (int i = 20; i >= 0; i--) {
			if (p == null) break;

			if (i == 0) {
				p.previous = null;
				break;
			}
			p = p.previous;
		}

		snakePosition = new List<SnakeVector> ();
		used = new bool[]{false, false, false, false};
	}
}

/// <summary>
/// 贪吃蛇环境描述：拐点
/// </summary>
public class SnakeCornerDescription
{
	public List<SnakeVector> corner;
	public float[] score;

	public SnakeCornerDescription()
	{
		corner = new List<SnakeVector> ();
		score = new float[]{0, 0, 0, 0};
	}

	public string GetData()
	{
		string data = "";
		for (int i = 0; i < corner.Count; i++) {
			if (i > 0) data += "|";
			data += corner [i].x + "," + corner [i].y;
		}
		data += "_";
		for (int i = 0; i < score.Length; i++) {
			if (i > 0) data += ",";
			data += score [i];
		}
		return data;
	}

	public static bool operator ==(SnakeCornerDescription a, SnakeCornerDescription b)
	{
		if (a.corner.Count != b.corner.Count) return false;
		for (int i = 0; i < a.corner.Count; i++) {
			if (a.corner [i] != b.corner [i]) {
				return false;
			}
		}
		return true;
	}

	public static bool operator !=(SnakeCornerDescription a, SnakeCornerDescription b)
	{
		return !(a == b);
	}

}

/// <summary>
/// 贪吃蛇环境描述：食物位置
/// </summary>
public class SnakeFoodDescription
{
	public SnakeVector food;
	public float[] score;

	public SnakeFoodDescription(SnakeVector foodPos)
	{
		food = foodPos;
		score = new float[]{0, 0, 0, 0};
	}

	public string GetData()
	{
		string data = food.x + "," + food.y + "_";
		for (int i = 0; i < score.Length; i++) {
			if (i > 0) data += ",";
			data += score [i];
		}
		return data;
	}

	public static bool operator ==(SnakeFoodDescription a, SnakeFoodDescription b)
	{
		return a.food.Equals (b.food);
	}

	public static bool operator !=(SnakeFoodDescription a, SnakeFoodDescription b)
	{
		return !(a == b);
	}
}

/// <summary>
/// 贪吃蛇环境描述：蛇头位置
/// </summary>
public class SnakeHeadDescription
{
	public SnakeVector head;
	public float[] score;

	public SnakeHeadDescription(SnakeVector headPos)
	{
		if (headPos.x <= 0) {
			headPos.x = 0;
		} else if (headPos.x >= SnakeMap.instance.mapWidth - 1) {
			headPos.x = SnakeMap.instance.mapWidth - 1;
		} else {
			headPos.x = 1;
		}

		if (headPos.y <= 0) {
			headPos.y = 0;
		} else if (headPos.y >= SnakeMap.instance.mapHeight - 1) {
			headPos.y = SnakeMap.instance.mapHeight - 1;
		} else {
			headPos.y = 1;
		}

		head = headPos;
		score = new float[]{0, 0, 0, 0};
	}

	public string GetData()
	{
		string data = head.x + "," + head.y + "_";
		for (int i = 0; i < score.Length; i++) {
			if (i > 0) data += ",";
			data += score [i];
		}
		return data;
	}

	public static bool operator ==(SnakeHeadDescription a, SnakeHeadDescription b)
	{
		return a.head.Equals (b.head);
	}

	public static bool operator !=(SnakeHeadDescription a, SnakeHeadDescription b)
	{
		return !(a == b);
	}
}

/// <summary>
/// 贪吃蛇环境描述
/// </summary>
public class SnakeDescription 
{
	public static void GetCurDescription(SnakeAgent agent, SnakeMap map, 
		out SnakeHeadDescription headDesc,
		out SnakeCornerDescription cornerDesc, 
		out SnakeFoodDescription foodDesc )
	{
		SnakeVector[] positions = agent.snakePositions;

		cornerDesc = new SnakeCornerDescription ();
		int same = positions [1].SameLine (positions[0]);
		for(int i = 2; i < positions.Length; i++){
			int s = positions [i].SameLine (positions[i-1]);
			if (s != same || i == positions.Length - 1) {
				cornerDesc.corner.Add (positions[i-1] - positions[0]);
			}
		}

		headDesc = new SnakeHeadDescription (positions [0]);
		foodDesc = new SnakeFoodDescription (map.foodPos - positions [0]);
	}

	public static float[] getSocre(float[] headScore, float[] cornerScore, float[] foodScore)
	{
		float[] score = new float[headScore.Length];
		for(int i = 0; i < headScore.Length; i++){
			score[i] = headScore [i] * 5 + cornerScore [i] * 3 + foodScore [i];
		}
		return score;
	}

}