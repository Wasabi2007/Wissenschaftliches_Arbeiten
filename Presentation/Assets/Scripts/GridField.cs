using UnityEngine;
using System.Collections;

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

	public LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
		lineRenderer.enabled = false;
		lineRenderer.SetColors (Default, Default);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void setupLinerender (){
		if (prevLink == null) {
						lineRenderer.enabled = false;
			return;
				}

		lineRenderer.SetVertexCount (2);
		lineRenderer.SetPosition (0, prevLink.transform.position + Vector3.up * 0.1f);
		lineRenderer.SetPosition (1, transform.position + Vector3.up * 0.1f);
		lineRenderer.enabled = true;
	}

	public void SetWay (bool isWay){
		if(isWay)
			lineRenderer.SetColors (WayColor, WayColor);
		else
			lineRenderer.SetColors (Default, Default);
	}

	public void setState(FieldState State,Color col){
		this.State = State;
		this.renderer.material.color = col;
	}
}
