using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxAnim : MonoBehaviour {

	public GameObject body1, body2, body3, body4, body5;

	public GameObject thing1, thing2, thing3, thing4, thing5, thing6;

	public float bodyAnimSpeed, bodyAnimAmp;
	public float thingAnimSpeed, thingAnimAmp;



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



		thing1.transform.localEulerAngles = new Vector3(thing1.transform.localEulerAngles.x, 
			thing1.transform.localEulerAngles.y, (Mathf.Sin(Time.time/(thingAnimSpeed * -0.85f))*thingAnimAmp));
		thing2.transform.localEulerAngles = new Vector3(thing2.transform.localEulerAngles.x, 
			thing2.transform.localEulerAngles.y, (Mathf.Sin(Time.time/(thingAnimSpeed * 1.15f))*thingAnimAmp));
		thing3.transform.localEulerAngles = new Vector3(thing3.transform.localEulerAngles.x, 
			thing3.transform.localEulerAngles.y, (Mathf.Sin(Time.time/(thingAnimSpeed * -0.9f))*thingAnimAmp));
		thing4.transform.localEulerAngles = new Vector3(thing4.transform.localEulerAngles.x, 
			thing4.transform.localEulerAngles.y, (Mathf.Sin(Time.time/(thingAnimSpeed * 1.05f))*thingAnimAmp));
		thing5.transform.localEulerAngles = new Vector3(thing5.transform.localEulerAngles.x, 
			thing5.transform.localEulerAngles.y, (Mathf.Sin(Time.time/(thingAnimSpeed * -0.8f))*thingAnimAmp));
		thing6.transform.localEulerAngles = new Vector3(thing6.transform.localEulerAngles.x, 
			thing6.transform.localEulerAngles.y, (Mathf.Sin(Time.time/(thingAnimSpeed * 0.95f))*thingAnimAmp));

		//thing1.transform.Rotate(new Vector3(0f,0f,0.5f * Time.deltaTime));


	}
}
