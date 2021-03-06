﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MLDescription
{
	private int feature;
	private Vector2 playerPosition;
	private List<Vector2> nearNodes;
	private List<Vector2> nearForwards;

	public MLDescription(int _feature, Vector2 _playerPosition = new Vector2(), List<Vector2> _nearNodes = null, List<Vector2> _nearForwards = null)
	{
		feature = _feature;
		playerPosition = _playerPosition;

		if (feature == 0) {
			nearNodes = new List<Vector2> ();
			nearForwards = new List<Vector2> ();
		} else {
			nearNodes = _nearNodes;
			nearForwards = _nearForwards;
		}
	}

	private MLDescription(int _feature)
	{
		feature = _feature;
		playerPosition = Vector2.zero;
		nearNodes = new List<Vector2> ();
		nearForwards = new List<Vector2> ();
	}

	public static MLDescription[] getCurDescription(MLEnviroment enviroment)
	{
		MLAgent player = enviroment.player;

		MLDescription desc_0 = new MLDescription (0);
		desc_0.playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);

		MLDescription desc_1 = new MLDescription (1);
		MLDescription desc_2 = new MLDescription (2);

		foreach (MLNode node in enviroment.nodes) {
			Vector3 localPosition = node.transform.position - player.transform.position;
			if (localPosition.magnitude < 10) {
				Vector2 localPos = new Vector2 (blurCount (localPosition.x), blurCount (localPosition.y));
				Vector2 forward = new Vector2 ((int)node.transform.forward.x, (int)node.transform.forward.y);
				if (node.type == 1) {
					desc_2.nearNodes.Add (localPos);
					desc_2.nearForwards.Add (forward);
				} else {
					desc_1.nearNodes.Add (localPos);
					desc_1.nearForwards.Add (forward);
				}
			}
		}

		return new MLDescription[]{desc_0, desc_1, desc_2};
	}

	private static int blurCount(float num)
	{
		for (int i = 3; i <= 9; i += 3) {
			if (num < i + 1.5f) return i;
		}

		return 0;
	}

	public int Approximate(MLDescription desc)
	{
		if (feature != desc.feature) return -1;

		if (feature == 0) {
			bool appPlayer = ApproximatePlayer (desc);
			if (appPlayer) return 0;
		}else if (feature == 1) {
			bool appRed = ApproximateNode (desc);
			if (appRed) return 1;
		}else if (feature == 2) {
			bool appGreen = ApproximateNode (desc);
			if (appGreen) return 2;
		}

		return -1;
	}

	private bool ApproximatePlayer(MLDescription desc)
	{
		if (Vector2.Distance (playerPosition, desc.playerPosition) < 1f) {
			return true;
		}
		return false;
	}

	private bool ApproximateNode(MLDescription desc)
	{
		if (nearNodes.Count == 0) return false;

		if (nearNodes.Count == desc.nearNodes.Count) {
			foreach(Vector2 thisPos in nearNodes){
				foreach(Vector2 pos in desc.nearNodes){
					if (Vector2.Distance (thisPos, pos) > 2f) {
						return false;
					}
				}
			}
			foreach(Vector2 thisForward in nearForwards){
				foreach(Vector2 forward in desc.nearForwards){
					if (Vector2.Distance (thisForward, forward) > .5f) {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}

	public int getFeature()
	{
		return feature;
	}

	public Vector2 getPlayerPosition()
	{
		return playerPosition;
	}

	public List<Vector2> getNearNodes()
	{
		List<Vector2> result = new List<Vector2> ();
		foreach(Vector2 vec in nearNodes){
			result.Add (vec);
		}
		return result;
	}

	public List<Vector2> getNearForwards()
	{
		List<Vector2> result = new List<Vector2> ();
		foreach(Vector2 vec in nearForwards){
			result.Add (vec);
		}
		return result;
	}

}