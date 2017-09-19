using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SnakeMap : MonoBehaviour 
{
	public static SnakeMap instance{ private set; get;}
	public static bool training = true;

	public SnakeAgent snake;
	public GameObject food;

	public SnakeVector foodPos{get{return new SnakeVector (food.transform.localPosition);}}

	public int mapWidth{ private set; get;}
	public int mapHeight{ private set; get;}

	void Awake()
	{
		instance = this;
	}

	public void Init () 
	{
		mapWidth = (int)transform.localScale.x * 10;
		mapHeight = (int)transform.localScale.y * 10;

		Texture2D tex = new Texture2D (mapWidth * 10 + 1, mapHeight * 10 + 1, TextureFormat.RGBA32, false);
		for (int w = 0; w < tex.width; w++) {
			for (int h = 0; h < tex.height; h++) {
				if (w % 10 == 0 || h % 10 == 0) {
					tex.SetPixel (w, h, Color.black);
				} else {
					tex.SetPixel (w, h, Color.gray);
				}
			}
		}
		tex.Apply ();
		GetComponent<MeshRenderer> ().material.SetTexture ("_MainTex", tex);

		snake.Init ();
	}
	
	void Update () 
	{

	}

	public void createFood(SnakeStatus status = null)
	{
		SnakeVector pos = status != null ? status.foodPosition : new SnakeVector (-1, -1);
//		pos = new SnakeVector (15, 7);

		while(pos.x == -1 && pos.y == -1) {
			pos = new SnakeVector (Random.Range(0, mapWidth), Random.Range(0, mapHeight));
			SnakeVector[] snakePos = snake.snakePositions;
			foreach(SnakeVector snakeP in snakePos) {
				if (pos.Equals(snakeP)) {
					pos = new SnakeVector (-1, -1);
					break;
				}
			}
		}

		food.transform.localPosition = new Vector3 (pos.x, pos.y, 0);
	}

}