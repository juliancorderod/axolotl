using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smokeSceneAnim : MonoBehaviour {

	public GameObject clouds, branches, frameCity;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		clouds.transform.localPosition += new Vector3(0.07f * Time.deltaTime, 0f,0f);

		branches.transform.localEulerAngles = new Vector3(branches.transform.localEulerAngles.x, 
			branches.transform.localEulerAngles.y, (Mathf.Sin(Time.time/10))*7);
		
	}
}
