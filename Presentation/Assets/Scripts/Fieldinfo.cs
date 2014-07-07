using UnityEngine;
using System.Collections;

public class Fieldinfo : MonoBehaviour {
	public string Titel;

	public UILabel TitelLabel;
	public UILabel PositionLabel;
	public UILabel StateLabel;
	public UILabel AccCostLabel;
	public UILabel TotalCostLabel;

	// Use this for initialization
	void Start () {
		TitelLabel.text = Titel;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setInfo(int X,int Y,GridField.FieldState state, float accCost, float totalCost){
		PositionLabel.text = "[" + X + "," + Y + "]";
		StateLabel.text = state.ToString ();
		AccCostLabel.text = accCost.ToString();
		TotalCostLabel.text = totalCost.ToString();
	}
}
