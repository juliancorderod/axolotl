using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// ambient movement/animation
			// axolotl bob
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y+(Mathf.Sin(Time.time)/400), transform.localPosition.z);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y+(Mathf.Sin(Time.time)/400), transform.localPosition.z);
		
	}
}
