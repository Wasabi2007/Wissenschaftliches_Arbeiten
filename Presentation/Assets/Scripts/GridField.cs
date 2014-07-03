using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridField : MonoBehaviour {
	public enum FieldState{
		Unknown,
		Open,
		Closed,
		Obstical,
	}
	private FieldState state = FieldState.Open;
	public FieldState State { get{ return state; } set{ state = value;} }//0=not Visited; 1=Visited; 2=Closed;
	private GridField prevLink;
	[HideInInspector]
	public GridField PrevLink{ get{ return prevLink; } set{ prevLink = value; setupLinerender ();} }
	[HideInInspector]
	public float accumulatedDistance = 0;
	[HideInInspector]
	public float guessedTargetDistance = 0;

	public Color WayColor;
	public Color Default;

	public int X;
	public int Y;

	public LineRenderer activeLineRenderer;
	private List<GameObject> lineRender;

	// Use this for initialization
	void Start () {
		activeLineRenderer.enabled = false;
		activeLineRenderer.SetColors (Default, Default);
		lineRender = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void setupLinerender (){
		if (prevLink == null) {
			if (lineRender.Count != 0){
				activeLineRenderer = lineRender[0].GetComponent<LineRenderer>();
				lineRender.RemoveAt(0);
			}

			activeLineRenderer.enabled = false;


			foreach(GameObject go in lineRender){
				GameObject.Destroy(go);
			}
			lineRender.Clear();
			return;
		}

		if (lineRender.Count != 0) {
			GameObject go = (GameObject)GameObject.Instantiate(activeLineRenderer.gameObject);
			go.transform.parent = this.transform;
			activeLineRenderer = go.GetComponent<LineRenderer>();
		}

		activeLineRenderer.SetVertexCount (2);
		activeLineRenderer.SetPosition (0, prevLink.transform.position + Vector3.up * 0.1f);
		activeLineRenderer.SetPosition (1, transform.position + Vector3.up * 0.1f);
		activeLineRenderer.enabled = true;

		lineRender.Add (activeLineRenderer.gameObject);


	}

	public void SetWay (bool isWay){
		if(isWay)
			activeLineRenderer.SetColors (WayColor, WayColor);
		else
			activeLineRenderer.SetColors (Default, Default);
	}

	public void setState(FieldState State,Color col){
		this.State = State;
		this.renderer.material.color = col;
	}
}
