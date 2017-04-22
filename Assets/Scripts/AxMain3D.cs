using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;




public class AxMain3D : MonoBehaviour {
	public GameObject MainCamera;
	public GameObject TankCamera;
	public GameObject FaceGroup;
	public GameObject AllDays;
	
	public Material TankMat;
	public RenderTexture rTexture;
	public GameObject TankQuad;

    static List<string> templateDB = new List<string>();
    static List<string> phraseDB = new List<string>();
    List<string> todaysTemplate; 
    List<string> todaysPhrases;
    List<string> completedText;
    List<string> currentEntries;
    List<string>[] dayText = new List<string>[10];
    Day[] allDays = new Day[10];
    GameObject[] dayObjs = new GameObject[10];
    int zMove = 0, currentDay = 1, maxDays = 10, textState = 0, currentPhrase = 0, currentIndex = 1; 
	float lastShowDay = 0.0f;
	string currentEntry = " ", currentPhrases = "", currentTemplate = "";
	bool releaseTyping = false, templateComplete = false, phraseMatch = false;

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

    void SendAction(string act) {
        if (act == "stepFoward") 
    		dayObjs[(currentDay-1)].GetComponent<DayHandler>().ActionStepForward();
    	else if (act == "stepBack")
    		dayObjs[(currentDay-1)].GetComponent<DayHandler>().ActionStepBack();
    }

    void UpdateStrings() {
    	// TODO compare for what's been typed and color accordingly, for now, just build 
		currentPhrases = "";
		currentTemplate = "";

		for (int i=0; i<allDays[(currentDay-1)].dayPhrases.Count; i++) {
			currentPhrases += allDays[(currentDay-1)].dayPhrases[i] + "\r\n";
		}

		for (int i=0; i<todaysTemplate.Count; i++) {
			if (i % 2 != 0) {
				todaysTemplate[i] = todaysTemplate[i].ToLower();
				if (i == currentIndex && textState == 1 && !releaseTyping) { // this is what we're editing
				 	currentTemplate += "[<color=#FFFF00>" + todaysTemplate[i] + "</color>]";
				}
				else 
					currentTemplate += todaysTemplate[i];
			}
			else
				currentTemplate += todaysTemplate[i];
		}
		

    }

    void EnterText(KeyCode ltr) {
    	if (ltr >= KeyCode.A && ltr <= KeyCode.Z) { // we only accept alphabetical characters
    		todaysTemplate[currentIndex] += ltr;
    	}
    	else if (ltr == KeyCode.Backspace && todaysTemplate[currentIndex].Length > 0) {
    		todaysTemplate[currentIndex] = todaysTemplate[currentIndex].Substring(0, todaysTemplate[currentIndex].Length - 1);
    	}
    	else if(ltr == KeyCode.Return) {
    		if (todaysTemplate[currentIndex].Length == 0) {
    			releaseTyping = true;
    			todaysTemplate[currentIndex] = "___";
    		}
	   		else { // lock text in
	    		releaseTyping = true;
	    		currentIndex += 2;
	    		if (currentIndex >= todaysTemplate.Count) {
	    			templateComplete = true;
	    		}
    		}
    		
    	}
    	UpdateStrings();
    }

	void NewDayUpdate() {
		todaysTemplate = new List<string>();
    	todaysPhrases = new List<string>();
		currentEntries = new List<string>();
		string curStr = "";
		int numEntries = 0;
		templateComplete = false;
		for (int i=0; i<allDays[(currentDay-1)].dayTemplates.Count; i++) { // count the number of phrases in template
			for (int j=0; j<allDays[(currentDay-1)].dayTemplates[i].Length;j++) {
				if (allDays[(currentDay-1)].dayTemplates[i][j] == '_') {
					todaysTemplate.Add(curStr);
					todaysTemplate.Add("___");
					curStr = "";
					numEntries++;
				}
				else {
					curStr += allDays[(currentDay-1)].dayTemplates[i][j];
				}
				if (j == allDays[(currentDay-1)].dayTemplates[i].Length-1)
					todaysTemplate.Add(curStr);
			}
		}


		// Change alpha of tankQuad
		Color color = TankMat.color;
		color.a = 1.0f-(0.05f*currentDay);
		TankMat.color = color;

		// Change alpha of face
		FaceGroup.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(float)0.3+(0.05f*currentDay));


		// if we're > day 5, swap position of two
		// face starts at -3.5, tank at -1.5
		if (currentDay < 6) {
 			FaceGroup.transform.localPosition = new Vector3(FaceGroup.transform.localPosition.x, FaceGroup.transform.localPosition.y, (float)-3.5);	
 			TankQuad.transform.localPosition = new Vector3(TankQuad.transform.localPosition.x, TankQuad.transform.localPosition.y, (float)-1.5);	
		}
		else {
			FaceGroup.transform.localPosition = new Vector3(FaceGroup.transform.localPosition.x, FaceGroup.transform.localPosition.y, (float)-1.5);	
			TankQuad.transform.localPosition = new Vector3(TankQuad.transform.localPosition.x, TankQuad.transform.localPosition.y, (float)-3.5);	
		}

		// disable all childen
		foreach(Transform child in AllDays.transform)
		{
			child.gameObject.SetActive(false);
		}

		// enable the correct day
		dayObjs[(currentDay-1)].SetActive(true);
		UpdateStrings();

	}

	void Start () {
		// turn all days on so we can add them to the array
		foreach(Transform child in AllDays.transform)
		{
			child.gameObject.SetActive(true);
		}

		// add all days to dayObjs array for quicker access, then turn them off
		for (int i=0;i<10;i++) {
			GameObject tmpObj = GameObject.Find("/ALLDAYS/day_"+(i+1));
			dayObjs[i] = tmpObj;
			tmpObj.SetActive(false);
		}


		// populate the template and phrase  arrays
		ReadFile("template.txt", templateDB);
	    ReadFile("phrase.txt", phraseDB);
		
	    // Set up the text for Days
	    	// manual
	    //			  array of templates    array of phrases
	  	allDays[0] = new Day(new int[] {0},new int[]{1,3});
	  	allDays[1] = new Day(new int[] {1},new int[]{2,4,5});
	  	allDays[2] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[3] = new Day(new int[] {1},new int[]{2,4,5});
	  	allDays[4] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[5] = new Day(new int[] {1},new int[]{2,4,5});
	  	allDays[6] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[7] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[8] = new Day(new int[] {0},new int[]{2,4,5});
	  	allDays[9] = new Day(new int[] {0},new int[]{2,4,5});
	
		NewDayUpdate();
		
	}	

	void OnGUI() {
		if (Time.time - lastShowDay < 2)
			GUI.Label(new Rect(Screen.width-100, Screen.height-20, 100, 20), "Current Day: " + currentDay.ToString());	

		// show current day template
		GUI.skin.label.fontSize = 18;
		GUI.Label(new Rect((Screen.width/3)-currentTemplate.Length, Screen.height-50, 500, 200), currentTemplate);	

		GUI.skin.label.fontSize = 14;
		// show available phrases   currentPhrases
		GUI.Label(new Rect(Screen.width-100, 100, 100, 600), currentPhrases);	

		if (textState == 1) { // we're typing
			Event e = Event.current;
        	if (e.isKey && e.type == EventType.KeyUp) // key up
        		EnterText(e.keyCode);
		}
	}
	
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
		// z-axis camera movement
		if (zMove == 1) { // stepping toward the glass
			
			if (MainCamera.GetComponent<Camera>().orthographicSize > 3) {
				MainCamera.GetComponent<Camera>().orthographicSize = MainCamera.GetComponent<Camera>().orthographicSize - (float)0.05;
			}
			else
				zMove = 2;

		}
		else if (zMove == -1) { // stepping back from the glass
	
		
			if (MainCamera.GetComponent<Camera>().orthographicSize < 4.5) 
				MainCamera.GetComponent<Camera>().orthographicSize = MainCamera.GetComponent<Camera>().orthographicSize + (float)0.05;
			else
				zMove = 0;
				
			
		}
		else if (zMove == 3) { // test rotation to left
			if (MainCamera.transform.localRotation.y > -.7) 
				MainCamera.transform.localRotation = new Quaternion(MainCamera.transform.localRotation.x, MainCamera.transform.localRotation.y-(float)0.05, MainCamera.transform.localRotation.z, MainCamera.transform.localRotation.w);	
			else
				zMove = 0;
		}

		// Keystrokes
		if (Input.GetKeyUp("return") && textState == 0 && !templateComplete) {
			// entering typing mode
			textState = 1;
			todaysTemplate[currentIndex] = "";
			UpdateStrings();
		}

 		if (Input.GetKey("escape"))
    		Application.Quit();

    	if (textState == 0) { // only if we're not typing something
	    	if (Input.GetKey("w") && zMove == 0 && textState == 0) {
	    		SendAction("stepFoward");
	    		zMove = 1;
	    	}
			if (Input.GetKey("s") && zMove == 2 && textState == 0) {
	    		SendAction("stepBack");
	    		zMove = -1;    		
			}
			if (Input.GetKeyUp("a")) { // DEBUG move day backward
	    		currentDay = (currentDay == 1) ? (maxDays) : (currentDay - 1);
	    		lastShowDay = Time.time;
	    		NewDayUpdate();
	    	}
	    	if (Input.GetKeyUp("q")) {
	    		MainCamera.GetComponent<Camera>().orthographic = false;
	    		zMove = 3;
	    	}
			if (Input.GetKeyUp("d")) { // DEBUG move day forward
	    		currentDay = (currentDay == maxDays) ? (1) : (currentDay + 1);    
	    		lastShowDay = Time.time;
	    		NewDayUpdate();
	    	}
		}
		if (releaseTyping) {
			textState = 0;
			releaseTyping = false;
		}

	}
}
