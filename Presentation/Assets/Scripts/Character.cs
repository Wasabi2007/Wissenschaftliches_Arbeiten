using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

	public float HeightOffset = 1f;
	public float speed = 10f;
	public Animator anim;

	private List<GridField> Way;
	private int index = 0;
	private float timer = 0;

	// Use this for initialization
	void Start () {
		if(Way == null)
			Way = new List<GridField> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("Way.Count: "+Way.Count);
		if (Way.Count > 1) {
			//Debug.Log("Way.Count: "+Way.Count);
						if (timer > 1) {
								timer=0;
								index++;
								if (index >= Way.Count-1) {
										index = 0;
								}
						}

						transform.position = Vector3.Lerp (Way [index].transform.position + Vector3.up * HeightOffset, Way [index + 1].transform.position + Vector3.up * HeightOffset, timer);
			Quaternion rot = Quaternion.LookRotation(Way [index + 1].transform.position-Way [index].transform.position,Vector3.up);
			rot *= Quaternion.Euler(0, -90, 0);
			transform.rotation = rot;
						timer += Time.deltaTime * (speed / 10f);
				}
	}

	public void WalkWay(List<GridField> way){
		index = 0;
		timer=0;
		this.Way = way;
		transform.position = Way [index].transform.position + Vector3.up * HeightOffset;
		anim.SetBool ("Walking", true);
		anim.SetBool ("Waveing", false);
		//Debug.Log("Way.Count: "+Way.Count);
		//Debug.Log ("Hi "+ Way.Count);
	}
}
