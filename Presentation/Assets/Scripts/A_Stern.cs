using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
	public bool microSearch = false;
	public EditMode editState = EditMode.Obstical;

	public Gradient Start_Color;
	public Gradient End_Color;
	public Gradient Default_Color;
	public Gradient Open_Color;
	public Gradient Closed_Color;
	public Gradient Obstical_Color;
	public Gradient Path_Color;

	public DistanceVis disVis;
	public Fieldinfo currentFieldInfo;
	public Fieldinfo researchFieldInfo;

	public Marker StartMarker;
	public Marker EndMarker;
	public Marker CurrentFieldMarker;
	public Marker ResearchFieldMarker;

	public UILabel OpenListOut;
	public Character WalkChar;
	public int CharctersAmmount = 10;

	private List<Character> Charcters;
	private List<GridField> CurrentWay;
	private float spawnTime = 1.0f;

	private List<GridField> Field;
	private GridField[,] FieldArray;
	private float tileWidth;
	private float tileHight;

	private GridField start;
	private GridField end;

	private bool searching = false;

	public float WaitTime = 1.0f;
	private float timeStamp = 0.0f;


	private StringBuilder openListOut; 
	// Use this for initialization
	void Start () {
		if (!FieldTile.GetComponent<GridField> ())
						return;

		StartMarker.gameObject.SetActive(false);
		EndMarker.gameObject.SetActive(false);
		CurrentFieldMarker.gameObject.SetActive(false);
		ResearchFieldMarker.gameObject.SetActive(false);
		openListOut = new StringBuilder ();

		CurrentWay = new List<GridField>();
		Charcters = new List<Character> ();
		for (int charCount = 0; charCount < CharctersAmmount; charCount++) {
			GameObject go = (GameObject)GameObject.Instantiate(WalkChar.gameObject);
			go.SetActive(false);
			Charcters.Add(go.GetComponent<Character>());
		}

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
				GridField selectedField = go.GetComponent<GridField>();
				selectedField.evaValue = Random.value;
				go.renderer.material.color = Default_Color.Evaluate(selectedField.evaValue);
				selectedField.X = x;
				selectedField.Y = y;
				Field.Add(selectedField);
				FieldArray[x,y] = selectedField;
			}
		}

		StartMarker.gameObject.SetActive(true);
		start = FieldArray [Mathf.RoundToInt(Random.Range(0,X_Size)), Mathf.RoundToInt(Random.Range(0,Y_Size))];
		start.setState(GridField.FieldState.Unknown,Start_Color.Evaluate(start.evaValue));
		StartMarker.SetPos (start.transform.position);

		EndMarker.gameObject.SetActive(true);
		do {
						end = FieldArray [Mathf.RoundToInt (Random.Range (0, X_Size)), Mathf.RoundToInt (Random.Range (0, Y_Size))];
				} while(end == start);
		end.setState(GridField.FieldState.Unknown,End_Color.Evaluate(end.evaValue));
		EndMarker.SetPos (end.transform.position);
		
		disVis.gameObject.SetActive (false);
	}

	IEnumerator SpawnChars() {
		foreach (Character c in Charcters) {
			if(searching || !complet)
				break;

						c.gameObject.SetActive(true);
						c.WalkWay(CurrentWay);
						yield return new WaitForSeconds (spawnTime);

		}

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
					CurrentFieldMarker.gameObject.SetActive(false);
					ResearchFieldMarker.gameObject.SetActive(false);
					disVis.gameObject.SetActive (false);
					complet = true;
					switch(editState){
					case EditMode.Default:
						selectedField.setState(GridField.FieldState.Unknown,Default_Color.Evaluate(selectedField.evaValue));
						if(selectedField == end){
							end = null;
						}
						if( selectedField == start){
							start = null;
						}
						break;
					case EditMode.Start:
						StartMarker.gameObject.SetActive(true);
							if(start){
							start.setState(GridField.FieldState.Unknown,Default_Color.Evaluate(selectedField.evaValue));
							}
						selectedField.setState(GridField.FieldState.Unknown,Start_Color.Evaluate(selectedField.evaValue));
						start = selectedField;
						StartMarker.SetPos (start.transform.position);
						break;
					case EditMode.End:
						EndMarker.gameObject.SetActive(true);
						if(end){
							end.setState(GridField.FieldState.Unknown,Default_Color.Evaluate(selectedField.evaValue));
						}
						selectedField.setState(GridField.FieldState.Unknown,End_Color.Evaluate(selectedField.evaValue));
						end = selectedField;
						EndMarker.SetPos (end.transform.position);
						break;
					case EditMode.Obstical:
						selectedField.setState(GridField.FieldState.Obstical,Obstical_Color.Evaluate(selectedField.evaValue));
						if(selectedField == end){
							end = null;
							EndMarker.gameObject.SetActive(false);
						}
						if( selectedField == start){
							start = null;
							StartMarker.gameObject.SetActive(false);
						}
						break;
					}

					//Debug.Log("X: "+selectedField.X+" Y:"+selectedField.Y);
				}
			}
		}
		if(searching && Time.time > timeStamp){
			//search = false;
			//Clear();
			ASternStep();
			timeStamp = Time.time+WaitTime;
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

	public void Activate(){
		gameObject.SetActive (true);
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
		OpenListOut.text = "OpenList:";
		OpenList.Clear ();
		ClosedList.Clear ();
		neigbours.Clear ();
		neigbourCount = 0;
		disVis.gameObject.SetActive (false);
		CurrentFieldMarker.gameObject.SetActive(false);
		ResearchFieldMarker.gameObject.SetActive(false);


		foreach (Character c in Charcters)
				c.gameObject.SetActive(false);

		foreach (GridField gField in Field) {
			gField.PrevLink = null;
			gField.SetWay(false);
						if (gField.State == GridField.FieldState.Open || gField.State == GridField.FieldState.Closed) {
							if(gField == start){
					gField.setState(GridField.FieldState.Unknown,Start_Color.Evaluate(gField.evaValue));
							}else if(gField == end){
					gField.setState(GridField.FieldState.Unknown,End_Color.Evaluate(gField.evaValue));	
							}else{
					gField.setState(GridField.FieldState.Unknown,Default_Color.Evaluate(gField.evaValue));
							}
						}
				}

		complet = true;
	}


	private float currentDistanceKey(GridField gField){
			if (useDijkstra)
							return gField.accumulatedDistance;
					else
							return gField.guessedTargetDistance;
		}

	private SortedList<float,List<GridField>> OpenList = new SortedList<float, List<GridField>> ();
	private List<GridField> ClosedList = new List<GridField> ();
	private List<GridField> neigbours = new List<GridField>();
	private GridField currentField;
	private bool complet = true;
	private int neigbourCount = 0;

	public void ASternStep(){
		if (!start || !end)
						return;



		if (complet) {
			Clear();
						CurrentFieldMarker.gameObject.SetActive(true);
						ResearchFieldMarker.gameObject.SetActive(true);
			if(!useDijkstra && !disVis.gameObject.activeSelf)
				disVis.gameObject.SetActive(true);

						OpenList.Add (0, new List<GridField> ());
						OpenList [0].Add (start);
						start.State = GridField.FieldState.Open;
			complet = false;
				}

		int stepCount = 0;

		if (neigbourCount >= neigbours.Count || currentField == null) {
						currentField = OpenList.Values [0] [0];
						OpenList.Values [0].RemoveAt (0);
						if (OpenList.Values [0].Count == 0) {
								OpenList.RemoveAt (0);
						}
					currentFieldInfo.setInfo(currentField.X,currentField.Y,currentField.State,currentField.accumulatedDistance,(useDijkstra?currentField.accumulatedDistance:currentField.guessedTargetDistance));
					CurrentFieldMarker.SetPos (currentField.transform.position);
				}
			researchNeigbours (OpenList, currentField);
			/*Debug.Log("Step "+(stepCount++));
			foreach( KeyValuePair<float, List<GridField>> kvp in OpenList )
			{
				Debug.Log("Key = "+kvp.Key+", Value = "+kvp.Value.Count);
			}*/
			
				if(currentField!=start && currentField!=end)
			currentField.setState(GridField.FieldState.Closed,Closed_Color.Evaluate(currentField.evaValue));
				else
					currentField.State = GridField.FieldState.Closed;

			ClosedList.Add(currentField);

		if (end.State != GridField.FieldState.Closed && OpenList.Count == 0 && neigbourCount >= neigbours.Count) {
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
		if (neigbourCount >= neigbours.Count) {
						int x = currentField.X, y = currentField.Y;
						neigbours.Clear ();
						if (y > 0)
								neigbours.Add (FieldArray [x, y - 1]);
						if (x < X_Size - 1)
								neigbours.Add (FieldArray [x + 1, y]);
						if (y < Y_Size - 1)
								neigbours.Add (FieldArray [x, y + 1]);
						if (x > 0)
								neigbours.Add (FieldArray [x - 1, y]);
						if (withDiagonals) {
								if (y > 0 && x > 0)
										neigbours.Add (FieldArray [x - 1, y - 1]);
								if (y < Y_Size - 1 && x < X_Size - 1)
										neigbours.Add (FieldArray [x + 1, y + 1]);
								if (y > 0 && x < X_Size - 1)
										neigbours.Add (FieldArray [x + 1, y - 1]);
								if (y < Y_Size - 1 && x > 0)
										neigbours.Add (FieldArray [x - 1, y + 1]);
						}
						neigbourCount = 0;
				}
		
		if (microSearch) {
			nextNeighbour (neigbours [neigbourCount], currentField);
			if(reseached > 1){
				reseached = 0;
				neigbourCount++;
			}		
					
				} else {
						for(;neigbourCount < neigbours.Count;neigbourCount++)
							nextNeighbour (neigbours [neigbourCount], currentField);
				}

	}

	int reseached = 0;

	void nextNeighbour (GridField gField, GridField currentField)
	{
		if (microSearch && reseached == 0) {
			reseached++;
				} else {
			reseached++;
						if (gField.State == GridField.FieldState.Unknown) {
								if (gField != start && gField != end)
										gField.setState (GridField.FieldState.Open, Open_Color.Evaluate (gField.evaValue));
								else
										gField.State = GridField.FieldState.Open;
								gField.accumulatedDistance = currentField.accumulatedDistance + Vector3.Distance (currentField.transform.position, gField.transform.position);
								if (!useDijkstra)
										disVis.VisDistance (gField.transform.position + Vector3.up, end.transform.position + Vector3.up);
								gField.guessedTargetDistance = gField.accumulatedDistance + (useDijkstra ? 0 : Vector3.Distance (gField.transform.position, end.transform.position));
								gField.PrevLink = currentField;
								if (!OpenList.ContainsKey (gField.guessedTargetDistance)) {
										OpenList.Add (gField.guessedTargetDistance, new List<GridField> ());
								}
								OpenList [gField.guessedTargetDistance].Add (gField);
						} else if (gField.State == GridField.FieldState.Open) {
								float accDistance = currentField.accumulatedDistance + Vector3.Distance (currentField.transform.position, gField.transform.position);// + (useDijkstra ? 0 : Vector3.Distance (gField.transform.position, end.transform.position));
								if (gField.accumulatedDistance > accDistance) {
										OpenList [gField.guessedTargetDistance].Remove (gField);
										if (OpenList [gField.guessedTargetDistance].Count == 0) {
												OpenList.Remove (gField.guessedTargetDistance);
										}
										gField.accumulatedDistance = accDistance;
										if (!useDijkstra)
												disVis.VisDistance (gField.transform.position + Vector3.up, end.transform.position + Vector3.up);
										gField.guessedTargetDistance = gField.accumulatedDistance + (useDijkstra ? 0 : Vector3.Distance (gField.transform.position, end.transform.position));
										gField.PrevLink = currentField;
										if (!OpenList.ContainsKey (gField.guessedTargetDistance)) {
												OpenList.Add (gField.guessedTargetDistance, new List<GridField> ());
										}
										OpenList [gField.guessedTargetDistance].Add (gField);
								}
						}
						
				}

		researchFieldInfo.setInfo (gField.X, gField.Y, gField.State, gField.accumulatedDistance, (useDijkstra ? gField.accumulatedDistance : gField.guessedTargetDistance));
		ResearchFieldMarker.SetPos (gField.transform.position);

		openListOut.Length = 0;
		openListOut.AppendLine ("OpenList:");
		foreach(KeyValuePair<float,List<GridField>> kp in OpenList){
			openListOut.Append(kp.Key);
			openListOut.Append(" : ");
			foreach(GridField gf in kp.Value){
				openListOut.Append("[");
				openListOut.Append(gf.X);
				openListOut.Append(",");
				openListOut.Append(gf.Y);
				openListOut.Append("]");
				openListOut.Append(" ");
			}
			openListOut.AppendLine();
		}

		OpenListOut.text = openListOut.ToString ();
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
		CurrentWay.Clear ();
		CurrentWay.Insert(0,end);
			while (gField.PrevLink != start && toMuch <= 10000) {
				gField = gField.PrevLink;
				gField.SetWay (true);
			gField.setState (GridField.FieldState.Closed, Path_Color.Evaluate(gField.evaValue));
			CurrentWay.Insert(0,gField);
				toMuch++;
				
			}
		CurrentWay.Insert(0,start);
			if (toMuch >= 10000) {
				Debug.Log ("WoW " + toMuch + " iterations oO");
			}

		spawnTime = CurrentWay.Count/10f;

		StartCoroutine (SpawnChars());
	}

	public void setUseDijkstra(bool value) {
			useDijkstra = value;
	}

	public void setWithDiagonals(bool value) {
		withDiagonals = value;
	}
	public void setMicroSteps(bool value) {
		microSearch = value;
	}
	public void setWaitTime(string value) {
		WaitTime = float.Parse(value);
	}
}
