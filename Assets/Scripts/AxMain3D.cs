using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    static List<string> templateDB = new List<string>();
    static List<string> phraseDB = new List<string>();
    List<string> todaysTemplates; 
    List<string> todaysPhrases;
    List<string> completedText;
    List<string>[] dayText = new List<string>[10];
    Day[] allDays = new Day[10];

	public class Day {
		public List<string> dayTemplates;
		public List<string> dayPhrases;

		public Day(int[] temps, int[] phrases) { // constructor
			dayTemplates = new List<string>();
			for (int i=0; i<temps.Length; i++) 
				this.dayTemplates.Add(templateDB[temps[i]]);

			dayPhrases = new List<string>();
			for (int i=0; i<phrases.Length; i++) 
				this.dayPhrases.Add(phraseDB[phrases[i]]);
		}

		public void DebugDay() {
			foreach (var v in this.dayTemplates) {
    			print(v);
			}
			foreach (var v in this.dayPhrases) {
    			print(v);
			}

		}

	}

    void DebugList(List<string> l) {
    	foreach (var v in l) {
    		print(v);
		}
    }

    void DebugAllDays() {
    	for (int i=0;i<10;i++) {
    		allDays[i].DebugDay();
    	}
    }

    void ReadFile(string fn, List<string> outlist) {
    	try {
			StreamReader fr = new StreamReader(fn);
			string line;

			using (fr)
	         {
	             do
	             {
	                 line = fr.ReadLine();
	                 if (line != null)
	               		outlist.Add(line);
	              }
	             while (line != null);
	          
	       	}
	       }
	       catch // we should catch file not found exceptions at some point, probably
         	{
             //return false;
         	}
         
    }

	void TempFadeBounce() {
		
	}
	void NewDayUpdate() {
		todaysTemplates = new List<string>();
    	todaysPhrases = new List<string>();

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
		/*
			
			* day changes dictionary / scenes setup
			* word/phrase implementation
			* sound layers
			* turning to leave
			* make face more transparent when stepping to glass
		*/
	}

	void Start () {
		zCamStart = MainCamera.transform.localPosition.z;
		//glassClr = GlassA.GetComponent<SpriteRenderer>().color;
		NewDayUpdate();
		plxgrpStart = PLXGroup.transform.localScale;

		// populate the template and phrase  arrays
		ReadFile("template.txt", templateDB);
	    ReadFile("phrase.txt", phraseDB);
		
	    // Set up the Days
	    	// manual
	  	allDays[0] = new Day(new int[] {0},new int[]{1,3});
	  	allDays[1] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[2] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[3] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[4] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[5] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[6] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[7] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[8] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[9] = new Day(new int[] {0},new int[]{2,4,5});
	 	DebugAllDays();


		
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
		else if (zMove == 3) { // test rotation to left
			if (MainCamera.transform.localRotation.y < 90) 
				MainCamera.transform.localRotation = new Quaternion(MainCamera.transform.localRotation.x, MainCamera.transform.localRotation.y+(float)0.05, MainCamera.transform.localRotation.z, MainCamera.transform.localRotation.w);	
			else
				zMove = 0;
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
    	if (Input.GetKeyUp("q")) {
    		zMove = 3;
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
