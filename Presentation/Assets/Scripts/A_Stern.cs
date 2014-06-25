using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class A_Stern : MonoBehaviour {

	public GameObject FieldTile;
	public int X_Size;
	public int Y_Size;

	public bool useDijkstra = false;

	public Color Start_Color;
	public Color End_Color;
	public Color Default_Color;
	public Color Obstical_Color;

	private List<GameObject> Field;
	private GameObject[,] FieldArray;
	private float tileWidth;
	private float tileHight;


	// Use this for initialization
	void Start () {
		Field = new List<GameObject> ();
		FieldArray = new GameObject[X_Size,Y_Size];
		tileWidth = FieldTile.renderer.bounds.size.x;
		tileHight = FieldTile.renderer.bounds.size.z;

		Vector3 origin = transform.position;

		for (int x = 0; x<X_Size; x++) {
			for (int y = 0; y<Y_Size; y++) {
				Vector3 offsetPosition = new Vector3(origin.x + x*tileWidth,origin.y ,origin.z + y*tileHight);
				GameObject go = (GameObject)GameObject.Instantiate(FieldTile);
				go.transform.position = offsetPosition;	
				go.transform.parent = transform;
				go.renderer.material.color = Default_Color;
				Field.Add(go);
				FieldArray[x,y] = go;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 mPos = Input.mousePosition;
			int x = (int)(mPos.x / tileWidth);
			int y = (int)(mPos.y / tileHight);
			Debug.Log("X: "+x+" Y:"+y);
			if(x >= 0 && x < X_Size && y >= 0 && y < Y_Size){
				FieldArray[x,y].renderer.material.color = Obstical_Color;
			}
		}
	}
}
