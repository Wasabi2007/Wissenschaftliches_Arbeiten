using UnityEngine;
using System.Collections;

public class DistanceVis : MonoBehaviour {

	public UILabel infoOut;
	public float lineWidth = 0.1f;
	public Material lineMat;
	public Color lineCol;
	private LineRenderer disVis;
	private LineRenderer disVisLeft;
	private LineRenderer disVisRight;

	private float borderHeight = 1;



	// Use this for initialization
	void Start () {
		GameObject go = new GameObject ();
		go.transform.parent = transform;
		disVis = go.AddComponent<LineRenderer> ();
		disVis.SetWidth (lineWidth,lineWidth);
		disVis.material = lineMat;
		disVis.SetColors (lineCol, lineCol);

		go = new GameObject ();
		go.transform.parent = transform;
		disVisLeft = go.AddComponent<LineRenderer> ();
		disVisLeft.SetWidth (lineWidth,lineWidth);
		disVisLeft.material = lineMat;
		disVisLeft.SetColors (lineCol, lineCol);

		go = new GameObject ();
		go.transform.parent = transform;
		disVisRight = go.AddComponent<LineRenderer> ();
		disVisRight.SetWidth (lineWidth,lineWidth);
		disVisRight.material = lineMat;
		disVisRight.SetColors (lineCol, lineCol);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void VisDistance(Vector3 p1, Vector3 p2){
		disVis.SetPosition (0, p1);
		disVis.SetPosition (1, p2);

		disVisLeft.SetPosition (0, p1+(Vector3.up*borderHeight));
		disVisLeft.SetPosition (1, p1-(Vector3.up*borderHeight));

		disVisRight.SetPosition (0, p2+(Vector3.up*borderHeight));
		disVisRight.SetPosition (1, p2-(Vector3.up*borderHeight));

		infoOut.text = Vector3.Distance (p1, p2).ToString();
		infoOut.transform.position = Vector3.Lerp (p1,p2,0.5f);


	}
}
