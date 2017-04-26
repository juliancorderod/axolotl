using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseParallax : MonoBehaviour {

	public GameObject MainCamera;
	public GameObject TankCamera;
	public GameObject FaceGroup;
	public GameObject AllDays;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// x-axis camera movement
		if (Input.mousePosition.x > 0 && Input.mousePosition.x < (Screen.width-5)) { // only allow mouse movements that are within game window

			// Move front box left and right at same speed of mouse x axis (higher divisor is, the slower it moves)
			if(Input.GetAxis("Mouse X") != 0) {
				MainCamera.transform.localPosition = new Vector3(MainCamera.transform.localPosition.x + Input.GetAxis("Mouse X")/200, MainCamera.transform.localPosition.y, MainCamera.transform.localPosition.z);	
				TankCamera.transform.localPosition = new Vector3(TankCamera.transform.localPosition.x + Input.GetAxis("Mouse X")/200, TankCamera.transform.localPosition.y, TankCamera.transform.localPosition.z);					
				FaceGroup.transform.localPosition = new Vector3(FaceGroup.transform.localPosition.x - Input.GetAxis("Mouse X")/100, FaceGroup.transform.localPosition.y, FaceGroup.transform.localPosition.z);	
				AllDays.transform.localPosition = new Vector3(AllDays.transform.localPosition.x + Input.GetAxis("Mouse X")/50, AllDays.transform.localPosition.y, AllDays.transform.localPosition.z);	
			}
		}	
	}		
}
