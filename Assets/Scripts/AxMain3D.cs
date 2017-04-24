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
	public GameObject FadeSquare;
	
	public Material TankMat;
	public RenderTexture rTexture;
	public GameObject TankQuad;

    List<string> todaysTemplate; 
    List<string> todaysPhrases;
    List<string> completedText;
    List<string>[] dayText = new List<string>[10];
    Day[] allDays = new Day[10];
    GameObject[] dayObjs = new GameObject[10];
    int zMove = 0, currentDay = 1, maxDays = 10, textState = -1, currentPhrase = 0, currentIndex = 1; 
	float lastShowDay = 0.0f;
	Color guiColor = Color.grey;
	string currentEntry = " ", currentPhrases = "", currentTemplate = "", gameState = "intro"; // gameState = intro, startDay, active, endDay
	bool releaseTyping = false, templateComplete = false, phraseMismatch = false;
	

	public class Day {
		public List<string> dayTemplates;
		public List<string> dayPhrases;

		public Day(string tmp, string phs) { // constructor
			dayTemplates = new List<string>();
			this.dayTemplates.Add(tmp);
			//for (int i=0; i<temps.Length; i++) 
				//this.dayTemplates.Add(templateDB[temps[i]]);

			dayPhrases = new List<string>();
			string[] psp = phs.Split(',');
			for (int i=0; i<psp.Length; i++) 
				this.dayPhrases.Add(psp[i]);
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

    void ReadFile(string fn, string[] ar) {
    	try {
			StreamReader fr = new StreamReader(fn);
			string line;
			int ln = 0;
			using (fr)
	         {
	             do
	             {
	                 line = fr.ReadLine();
	                 if (line != null) {
	                 	ar[ln] = line;
	                 	ln++;
	                 }
	               		
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
		currentPhrases = "";
		currentTemplate = "";

		for (int i=0; i<allDays[(currentDay-1)].dayPhrases.Count; i++) {
			currentPhrases += allDays[(currentDay-1)].dayPhrases[i] + "\r\n";
		}

		for (int i=0; i<todaysTemplate.Count; i++) {
			if (i % 2 != 0) {
				todaysTemplate[i] = todaysTemplate[i].ToLower();
				if (i == currentIndex && textState == 1 && !releaseTyping) { // this is what we're editing
					if (phraseMismatch) // trying to lock something in that's not an available phrase
				 		currentTemplate += "[<color=#FF0000>" + todaysTemplate[i] + "</color>]";
				 	else
				 		currentTemplate += "[<color=#FFFF00>" + todaysTemplate[i] + "</color>]";
				}
				else 
					currentTemplate += todaysTemplate[i];
			}
			else
				currentTemplate += todaysTemplate[i];
		}
		

    }

    bool CheckPhraseMatch() {
    	//return false;
    	bool foundMatch = false;
    	for (int i=0; i<allDays[(currentDay-1)].dayPhrases.Count; i++) {
			if (allDays[(currentDay-1)].dayPhrases[i] == todaysTemplate[currentIndex]) {
				foundMatch = true;
				break;
			}
		}
		return foundMatch;
    }

    void EnterText(KeyCode ltr) {
    	phraseMismatch = false;
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
	   		else { // attempt to lock text in
	   			if (CheckPhraseMatch()) {
	   				
	    			for (int i=0; i<allDays[(currentDay-1)].dayPhrases.Count; i++) {
						if (allDays[(currentDay-1)].dayPhrases[i] == todaysTemplate[currentIndex]) {
							allDays[(currentDay-1)].dayPhrases.RemoveAt(i);
							break;
						}
					}
					
					releaseTyping = true;
	    			currentIndex += 2;
	    			if (currentIndex >= todaysTemplate.Count) {
	    				templateComplete = true;
	    			}
	   			}
	   			else
	   				phraseMismatch = true;
	    		
    		}
    		
    	}
    	UpdateStrings();
    }

	void NewDayUpdate() {
		todaysTemplate = new List<string>();
    	todaysPhrases = new List<string>();
		
		string curStr = "";
		int numEntries = 0;
		currentIndex = 1;
		templateComplete = false;
		textState = -1;
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

		string[] allTmps = new string[10];
		string[] allPhrases = new string[10];

		// populate the template and phrase  arrays
		ReadFile("Assets/template.txt", allTmps);
	    ReadFile("Assets/phrase.txt", allPhrases);
		
	    // Set up the text for Days
	    for (int i=0;i<10;i++) 
	    	allDays[i] = new Day(allTmps[i],allPhrases[i]);
		NewDayUpdate();
		
	}	

	void OnGUI() {
		GUI.color = guiColor;
		if (gameState == "intro") {
			GUI.skin.label.fontSize = 18;
			GUI.Label(new Rect((Screen.width/4), Screen.height-300, 600, 300), "I don't quite remember when it started, but I began to see this strange thing every day...");	

			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press E to begin)");	
		}
		else if (gameState == "outro") {
			GUI.skin.label.fontSize = 18;
			GUI.Label(new Rect((Screen.width/2), Screen.height-300, 600, 200), "It walked out of the aquarium one final time, and I never saw it again...");	

			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press ESC to exit)");	
		}
		else {

			if (Time.time - lastShowDay < 2 && gameState == "active") {
				GUI.skin.label.fontSize = 14;
				GUI.Label(new Rect(Screen.width-120, 10, 120, 20), "Current Day: " + currentDay.ToString());	
			}

			if (textState > -1) {
				// show current day template
				GUI.skin.label.fontSize = 18;
				GUI.Label(new Rect((Screen.width/3)-currentTemplate.Length, Screen.height-75, 600, 300), currentTemplate);	

				// show available phrases 
				GUI.skin.label.fontSize = 14;
				GUI.Label(new Rect(Screen.width-100, Screen.height-100, 100, 600), currentPhrases);	

				if (textState == 1) { // we're typing
					Event e = Event.current;
		        	if (e.isKey && e.type == EventType.KeyUp) // key up
		        		EnterText(e.keyCode);
				}
				
				GUI.skin.label.fontSize = 15;
				if (templateComplete) 
					GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press E to complete writing)");	
				else if (textState == 1)
					//GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press Enter when finished)");	
				;
				else 
					GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press Enter to write)");	
				

			}
			else if (textState == -1) {
				GUI.skin.label.fontSize = 15;
				GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press E to begin writing)");	
			}
		}
	}
	
	void Update () {
		if (gameState == "startDay") {
			guiColor = Color.Lerp(Color.black, Color.grey, Time.time);
			if (FadeSquare.GetComponent<SpriteRenderer>().color.a > 0) {
				FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a-(float)0.02);
			}
			else
				gameState = "active";
		}
		else if (gameState == "endDay") {
			guiColor = Color.Lerp(Color.grey, Color.black, Time.time);
		}
		else if (gameState == "intro") {
			guiColor = Color.grey;
		}

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
		else if (zMove == 3) { // fading scene out
			if (FadeSquare.GetComponent<SpriteRenderer>().color.a < 1) {
				//MainCamera.transform.localRotation = new Quaternion(MainCamera.transform.localRotation.x, MainCamera.transform.localRotation.y-(float)0.005, MainCamera.transform.localRotation.z, MainCamera.transform.localRotation.w);	
				FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a+(float)0.02);
			}
			else {
				if (gameState != "outro") {
					gameState = "startDay";
					zMove = 0;
					NewDayUpdate();	
				}
				
			}
		}

		// Keystrokes

		if (Input.GetKey("escape"))
    		Application.Quit();

		if (Input.GetKeyUp("return") && textState == 0 && !templateComplete) {
			// entering typing mode
			textState = 1;
			todaysTemplate[currentIndex] = "";
			UpdateStrings();
		}

 		
		
		if (Input.GetKeyUp("e") && textState == -1 && gameState == "active") { 
	    	textState = 0;
	    	UpdateStrings();
    	}
    	else if (Input.GetKeyUp("e") && gameState == "intro") { 
	    	gameState = "startDay";
    	}
	    
    	if (textState != 1) { // only if we're not typing something
	    	if (Input.GetKey("w") && zMove == 0) {
	    		SendAction("stepFoward");
	    		zMove = 1;
	    	}
			if (Input.GetKey("s") && zMove == 2) {
	    		SendAction("stepBack");
	    		zMove = -1;    		
			}
	    	if (Input.GetKeyUp("e")) { 
	    		if (templateComplete) {
		    		zMove = 3;
		    		currentDay = (currentDay == maxDays) ? (99) : (currentDay + 1);    
		    		lastShowDay = Time.time;
		    		if (currentDay == 99)
		    			gameState = "outro";
		    		else
		    			gameState = "endDay";
	    		}
	    	}
		}


	    if (Input.GetKeyUp(",")) { // DEBUG move day backward
			zMove = 3;
			gameState = "endDay";
    		currentDay = (currentDay == 1) ? (maxDays) : (currentDay - 1);
    		lastShowDay = Time.time;
	    }
		if (Input.GetKeyUp(".")) { // DEBUG move day forward
			zMove = 3;
			gameState = "endDay";
    		currentDay = (currentDay == maxDays) ? (1) : (currentDay + 1);    
    		lastShowDay = Time.time;
	    }
	    
		if (releaseTyping) {
			textState = 0;
			releaseTyping = false;
		}

	}
}
