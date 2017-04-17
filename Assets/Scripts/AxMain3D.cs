using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AxMain3D : MonoBehaviour {
	public GameObject MainCamera;
	public GameObject HumanBoxA;
	public GameObject HumanBoxB;
	public GameObject AxBoxA;
	public GameObject AxBoxB;
	public GameObject PLXGroup;
	public bool threeD;
	int zMove = 0, currentDay = 1, maxDays = 10; 
	float zCamStart, lastShowDay = 0.0f;
	Vector3 plxgrpStart;
	Color glassClr;


	void TempFadeBounce() {
		
	}
	void NewDayUpdate() {
		TempFadeBounce();
		HumanBoxA.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(float)0.3-(0.1f*currentDay));
		HumanBoxB.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(float)-0.1+(0.1f*currentDay));
		AxBoxA.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(float)1.1-(0.1f*currentDay));
		AxBoxB.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(float)-0.8+(0.1f*currentDay));
		
		//AxBoxA.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(float)(1-(0.085*currentDay)));
		//AxBoxB.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(float)(0+(0.085*currentDay)));
		//GlassA.GetComponent<SpriteRenderer>().color = new Color(glassClr.r,glassClr.g,glassClr.b,(float)(glassClr.a-( (glassClr.a/maxDays) * currentDay)));
		//GlassB.GetComponent<SpriteRenderer>().color = new Color(glassClr.r,glassClr.g,glassClr.b,(float)(0+( (glassClr.a/maxDays) * currentDay)));
		
		//print( (glassClr.a/maxDays) * currentDay);
		//print(AxBoxA.GetComponent<SpriteRenderer>().color);
	}

	void Start () {
		zCamStart = MainCamera.transform.localPosition.z;
		//glassClr = GlassA.GetComponent<SpriteRenderer>().color;
		NewDayUpdate();
		plxgrpStart = PLXGroup.transform.localScale;
		
	}	

	void OnGUI() {
		if (Time.time - lastShowDay < 2)
			GUI.Label(new Rect(Screen.width-100, Screen.height-20, 100, 20), "Current Day: " + currentDay.ToString());	
	}
	
	void Update () {
		// x-axis camera movement
		if (Input.mousePosition.x > 0 && Input.mousePosition.x < (Screen.width-5)) { // only allow mouse movements that are within game window

			// Move front box left and right at same speed of mouse x axis (higher divisor is, the slower it moves)
			if(Input.GetAxis("Mouse X") != 0) {
				MainCamera.transform.localPosition = new Vector3(MainCamera.transform.localPosition.x + Input.GetAxis("Mouse X")/200, MainCamera.transform.localPosition.y, MainCamera.transform.localPosition.z);	
				HumanBoxA.transform.localPosition = new Vector3(HumanBoxA.transform.localPosition.x - Input.GetAxis("Mouse X")/100, HumanBoxA.transform.localPosition.y, HumanBoxA.transform.localPosition.z);	
				HumanBoxB.transform.localPosition = new Vector3(HumanBoxB.transform.localPosition.x - Input.GetAxis("Mouse X")/100, HumanBoxB.transform.localPosition.y, HumanBoxB.transform.localPosition.z);	
				AxBoxA.transform.localPosition = new Vector3(AxBoxA.transform.localPosition.x + Input.GetAxis("Mouse X")/100, AxBoxA.transform.localPosition.y, AxBoxA.transform.localPosition.z);	
				AxBoxB.transform.localPosition = new Vector3(AxBoxB.transform.localPosition.x + Input.GetAxis("Mouse X")/100, AxBoxB.transform.localPosition.y, AxBoxB.transform.localPosition.z);	
			}
	
		}
		// z-axis camera movement
		if (zMove == 1) { // stepping toward the glass
			if (threeD) {
				if (MainCamera.transform.localPosition.z - zCamStart < 2)
					MainCamera.transform.localPosition = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y, MainCamera.transform.localPosition.z+(  (float)0.1   ));	
				else
					zMove = 2;	
			}
			else {
				if (PLXGroup.transform.localScale.x < 1.5) 
					PLXGroup.transform.localScale = new Vector3(PLXGroup.transform.localScale.x+(float)0.025,PLXGroup.transform.localScale.y+(float)0.025,PLXGroup.transform.localScale.z+(float)0.025);
				else
					zMove = 2;

			}
			
		}
		else if (zMove == -1) { // stepping back from the glass
			if (threeD) {
				if (MainCamera.transform.localPosition.z - zCamStart > 0)
					MainCamera.transform.localPosition = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y, (MainCamera.transform.localPosition.z-((float)0.1)));	
				else
					zMove = 0;
			}
			else {
				if (PLXGroup.transform.localScale.x > 1) 
					PLXGroup.transform.localScale = new Vector3(PLXGroup.transform.localScale.x-(float)0.025,PLXGroup.transform.localScale.y-(float)0.025,PLXGroup.transform.localScale.z-(float)0.025);
				else
					zMove = 0;
				
			}
		}

		
		
		// ambient movement/animation
			// axolotl bob
		AxBoxA.transform.localPosition = new Vector3(AxBoxA.transform.localPosition.x, AxBoxA.transform.localPosition.y+(Mathf.Sin(Time.time)/400), AxBoxA.transform.localPosition.z);
		AxBoxB.transform.localPosition = new Vector3(AxBoxB.transform.localPosition.x, AxBoxB.transform.localPosition.y+(Mathf.Sin(Time.time)/400), AxBoxB.transform.localPosition.z);
		


		// Keystrokes
 		if (Input.GetKey("escape"))
    		Application.Quit();

    	if (Input.GetKey("w") && zMove == 0)
    		zMove = 1;
		if (Input.GetKey("s") && zMove == 2)
    		zMove = -1;    		
		if (Input.GetKeyUp("a")) { // DEBUG move day backward
    		currentDay = (currentDay == 1) ? (maxDays) : (currentDay - 1);
    		lastShowDay = Time.time;
    		NewDayUpdate();
    	}
		if (Input.GetKeyUp("d")) { // DEBUG move day forward
    		currentDay = (currentDay == maxDays) ? (1) : (currentDay + 1);    
    		lastShowDay = Time.time;
    		NewDayUpdate();
    	}
    	

   		// misc debug
   		//print(Time.time);
	}
}
