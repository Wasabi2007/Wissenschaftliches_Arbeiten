using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class A_Stern : MonoBehaviour {
	public enum EditMode{
		Start,
		End,
		Obstical,
		Default
	}


	public Camera SelectCam;

	public GameObject FieldTile;
	public int X_Size;
	public int Y_Size;

	public bool useDijkstra = false;
	public bool withDiagonals = true;
	public bool search = false;
	public EditMode editState = EditMode.Obstical;

	public Gradient Start_Color;
	public Gradient End_Color;
	public Gradient Default_Color;
	public Gradient Open_Color;
	public Gradient Closed_Color;
	public Gradient Obstical_Color;
	public Gradient Path_Color;

	private List<GridField> Field;
	private GridField[,] FieldArray;
	private float tileWidth;
	private float tileHight;

	private GridField start;
	private GridField end;

	private bool searching = false;

	// Use this for initialization
	void Start () {
		if (!FieldTile.GetComponent<GridField> ())
						return;

		Field = new List<GridField> ();
		FieldArray = new GridField[X_Size,Y_Size];
		tileWidth = FieldTile.renderer.bounds.size.x;
		tileHight = FieldTile.renderer.bounds.size.z;

		Vector3 origin = transform.position;

		for (int x = 0; x<X_Size; x++) {
			for (int y = 0; y<Y_Size; y++) {
				Vector3 offsetPosition = new Vector3(origin.x + x*tileWidth,origin.y ,origin.z + y*tileHight);
				GameObject go = (GameObject)GameObject.Instantiate(FieldTile);
				go.transform.position = offsetPosition;	
				go.transform.parent = transform;
				go.renderer.material.color = Default_Color.Evaluate(Random.value);
				GridField selectedField = go.GetComponent<GridField>();
				selectedField.X = x;
				selectedField.Y = y;
				Field.Add(selectedField);
				FieldArray[x,y] = selectedField;
			}
		}

		start = FieldArray [Mathf.RoundToInt(Random.Range(0,X_Size)), Mathf.RoundToInt(Random.Range(0,Y_Size))];
		start.setState(GridField.FieldState.Unknown,Start_Color.Evaluate(Random.value));

		end = FieldArray [Mathf.RoundToInt(Random.Range(0,X_Size)), Mathf.RoundToInt(Random.Range(0,Y_Size))];
		end.setState(GridField.FieldState.Unknown,End_Color.Evaluate(Random.value));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0) && (!UICamera.hoveredObject)) {
			Ray ray = SelectCam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if(Physics.Raycast(ray,out hitInfo)){
				GridField selectedField = hitInfo.collider.GetComponent<GridField>();
				if(selectedField){
					Clear();
					switch(editState){
					case EditMode.Default:
						selectedField.setState(GridField.FieldState.Unknown,Default_Color.Evaluate(Random.value));
						if(selectedField == end){
							end = null;
						}
						if( selectedField == start){
							start = null;
						}
						break;
					case EditMode.Start:
							if(start){
							start.setState(GridField.FieldState.Unknown,Default_Color.Evaluate(Random.value));
							}
						selectedField.setState(GridField.FieldState.Unknown,Start_Color.Evaluate(Random.value));
						start = selectedField;
						break;
					case EditMode.End:
						if(end){
							end.setState(GridField.FieldState.Unknown,Default_Color.Evaluate(Random.value));
						}
						selectedField.setState(GridField.FieldState.Unknown,End_Color.Evaluate(Random.value));
						end = selectedField;

						break;
					case EditMode.Obstical:
						selectedField.setState(GridField.FieldState.Obstical,Obstical_Color.Evaluate(Random.value));
						if(selectedField == end){
							end = null;
						}
						if( selectedField == start){
							start = null;
						}
						break;
					}

					//Debug.Log("X: "+selectedField.X+" Y:"+selectedField.Y);
				}
			}
		}
		if(searching){
			//search = false;
			//Clear();
			ASternStep();
		}

	}

	public void Search(){
		//Clear();
		//ASternStep();
		searching = true;
	}

	public void Step(){
		//Clear();
		ASternStep();
		//searching = true;
	}

	public void Pause(){
		//Clear();
		//ASternStep();
		searching = false;
	}


	public void SetEditState(string input){
		switch (input) {
		case	"Start":
			editState = EditMode.Start;
			break;
		case	"End":
			editState = EditMode.End;
			break;
		case	"Obstical":
			editState = EditMode.Obstical;
			break;
		case	"Default":
			editState = EditMode.Default;
			break;
				}
	}

	public void Clear(){
		foreach (GridField gField in Field) {
			gField.PrevLink = null;
			gField.SetWay(false);
						if (gField.State == GridField.FieldState.Open || gField.State == GridField.FieldState.Closed) {
							if(gField == start){
					gField.setState(GridField.FieldState.Unknown,Start_Color.Evaluate(Random.value));
							}else if(gField == end){
					gField.setState(GridField.FieldState.Unknown,End_Color.Evaluate(Random.value));	
							}else{
					gField.setState(GridField.FieldState.Unknown,Default_Color.Evaluate(Random.value));
							}
						}
				}
	}


	private float currentDistanceKey(GridField gField){
			if (useDijkstra)
							return gField.accumulatedDistance;
					else
							return gField.guessedTargetDistance;
		}

	SortedList<float,List<GridField>> OpenList = new SortedList<float, List<GridField>> ();
	List<GridField> ClosedList = new List<GridField> ();
	List<GridField> neigbours = new List<GridField>();
	bool complet = true;

	public void ASternStep(){
		if (!start || !end)
						return;



		if (complet) {
			Clear();
						OpenList.Clear ();
						ClosedList.Clear ();
						neigbours.Clear ();

						OpenList.Add (0, new List<GridField> ());
						OpenList [0].Add (start);
						start.State = GridField.FieldState.Open;
			complet = false;
				}

		int stepCount = 0;
		
			GridField currentField = OpenList.Values[0][0];
			OpenList.Values[0].RemoveAt(0);
			if(OpenList.Values[0].Count == 0){
				OpenList.RemoveAt(0);
			}
			researchNeigbours (OpenList, currentField);
			/*Debug.Log("Step "+(stepCount++));
			foreach( KeyValuePair<float, List<GridField>> kvp in OpenList )
			{
				Debug.Log("Key = "+kvp.Key+", Value = "+kvp.Value.Count);
			}*/
			
				if(currentField!=start && currentField!=end)
			currentField.setState(GridField.FieldState.Closed,Closed_Color.Evaluate(Random.value));
				else
					currentField.State = GridField.FieldState.Closed;

			ClosedList.Add(currentField);

		if (end.State != GridField.FieldState.Closed && OpenList.Count == 0) {
			complet = true;
			searching = false;
		}

		if(end.State == GridField.FieldState.Closed){
			complet = true;
			searching = false;
			FindWay ();
		}


	}

	void researchNeigbours (SortedList<float, List<GridField>> OpenList, GridField currentField)
	{
		int x=currentField.X,y=currentField.Y;
		neigbours.Clear();
		if(y>0)
			neigbours.Add(FieldArray[x,y-1]);
		if(x<X_Size-1)
			neigbours.Add(FieldArray[x+1,y]);
		if(y<Y_Size-1)
			neigbours.Add(FieldArray[x,y+1]);
		if(x>0)
			neigbours.Add(FieldArray[x-1,y]);
		if(withDiagonals){
			if(y>0 && x>0)
				neigbours.Add(FieldArray[x-1,y-1]);
			if(y<Y_Size-1 && x<X_Size-1)
				neigbours.Add(FieldArray[x+1,y+1]);
			if(y>0 && x<X_Size-1 )
				neigbours.Add(FieldArray[x+1,y-1]);
			if(y<Y_Size-1 && x>0)
				neigbours.Add(FieldArray[x-1,y+1]);
		}

		foreach (GridField gField in neigbours) {
			nextNeighbour (gField, currentField);
		}

	}

	void nextNeighbour (GridField gField, GridField currentField)
	{
		if (gField.State == GridField.FieldState.Unknown) {
			if (gField != start && gField != end)
				gField.setState (GridField.FieldState.Open, Open_Color.Evaluate(Random.value));
			else
				gField.State = GridField.FieldState.Open;
			gField.accumulatedDistance = currentField.accumulatedDistance + Vector3.Distance (currentField.transform.position, gField.transform.position);
			gField.guessedTargetDistance = gField.accumulatedDistance +  (useDijkstra ? gField.accumulatedDistance :Vector3.Distance (gField.transform.position, end.transform.position));
			gField.PrevLink = currentField;
			if (!OpenList.ContainsKey (gField.guessedTargetDistance )) {
				OpenList.Add (gField.guessedTargetDistance , new List<GridField> ());
			}
			OpenList [gField.guessedTargetDistance].Add (gField);
		}
		else if (gField.State == GridField.FieldState.Open) {
			float accDistance = currentField.accumulatedDistance + Vector3.Distance (currentField.transform.position, gField.transform.position);// + (useDijkstra ? 0 : Vector3.Distance (gField.transform.position, end.transform.position));
			if (gField.accumulatedDistance > accDistance) {
				OpenList [gField.guessedTargetDistance ].Remove (gField);
				if (OpenList [gField.guessedTargetDistance ].Count == 0) {
					OpenList.Remove (gField.guessedTargetDistance );
					}
					gField.accumulatedDistance = accDistance;
				gField.guessedTargetDistance = gField.accumulatedDistance +  (useDijkstra ? gField.accumulatedDistance :Vector3.Distance (gField.transform.position, end.transform.position));
				gField.PrevLink = currentField;
				if (!OpenList.ContainsKey (gField.guessedTargetDistance )) {
					OpenList.Add (gField.guessedTargetDistance , new List<GridField> ());
					}
				OpenList [gField.guessedTargetDistance ].Add (gField);
				}
			}
	}
	/*else if(gField.State == GridField.FieldState.Closed){
					float accDistance = currentField.accumulatedDistance + Vector3.Distance(currentField.transform.position, gField.transform.position)+(useDijkstra?0:Vector3.Distance(gField.transform.position, end.transform.position));
					
					if(currentDistanceKey(gField) > accDistance){
						gField.accumulatedDistance = accDistance;
						gField.guessedTargetDistance = gField.accumulatedDistance + Vector3.Distance(gField.transform.position, end.transform.position);
						gField.PrevLink = currentField;
					}
				}*/

	void FindWay ()
	{
			//Debug.Log("Found Path");
			GridField gField = end;
			end.SetWay (true);
			int toMuch = 0;
			while (gField.PrevLink != start && toMuch <= 10000) {
				gField = gField.PrevLink;
				gField.SetWay (true);
			gField.setState (GridField.FieldState.Closed, Path_Color.Evaluate(Random.value));
				toMuch++;
			}
			if (toMuch >= 10000) {
				Debug.Log ("WoW " + toMuch + " iterations oO");
			}
	}

	public void setUseDijkstra(bool value) {
			useDijkstra = value;
	}

	public void setWithDiagonals(bool value) {
		withDiagonals = value;
	}
}
