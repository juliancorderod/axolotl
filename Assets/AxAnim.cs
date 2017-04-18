using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxAnim : MonoBehaviour {

	public GameObject body1, body2, body3, body4, body5;

	public float bodyAnimSpeed, bodyAnimAmp;

	// Use this for initialization
	void Start () {

		//body1.transform.RotateAround(transform.localPosition,Vector3.forward,-f);
		
	}
	
	// Update is called once per frame
	void Update () {


		body1.transform.localEulerAngles = new Vector3(body1.transform.localEulerAngles.x, 
			body1.transform.localEulerAngles.y, (Mathf.Sin(Time.time/bodyAnimSpeed)*bodyAnimAmp));
		body2.transform.localEulerAngles = new Vector3(body2.transform.localEulerAngles.x, 
			body2.transform.localEulerAngles.y, (Mathf.Sin(Time.time/bodyAnimSpeed)*bodyAnimAmp));
		body3.transform.localEulerAngles = new Vector3(body3.transform.localEulerAngles.x, 
			body3.transform.localEulerAngles.y, (Mathf.Sin(Time.time/bodyAnimSpeed)*bodyAnimAmp));
		body4.transform.localEulerAngles = new Vector3(body4.transform.localEulerAngles.x, 
			body4.transform.localEulerAngles.y, (Mathf.Sin(Time.time/bodyAnimSpeed)*bodyAnimAmp * 1.5f));
		body5.transform.localEulerAngles = new Vector3(body5.transform.localEulerAngles.x, 
			body5.transform.localEulerAngles.y, (Mathf.Sin(Time.time/bodyAnimSpeed)*bodyAnimAmp * 2f));


	}
}
