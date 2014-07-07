using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour {

	public UILabel infoOut;
	public float lineWidth = 0.1f;
	public Material lineMat;
	public Color lineCol;
	public float Height;
	public float LableOffset;
	private LineRenderer markerLine;

	// Use this for initialization
	void Start () {
		GameObject go = new GameObject ();
		go.transform.parent = transform;
		markerLine = go.AddComponent<LineRenderer> ();
		markerLine.SetWidth (lineWidth,lineWidth);
		markerLine.material = lineMat;
		markerLine.SetColors (lineCol, lineCol);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetPos(Vector3 pos){
		markerLine.SetPosition (0, pos);
		markerLine.SetPosition (1, pos+(Vector3.up*Height));

		infoOut.transform.position =  pos+(Vector3.up*Height)+(Vector3.up*LableOffset);
		
		
	}
}
