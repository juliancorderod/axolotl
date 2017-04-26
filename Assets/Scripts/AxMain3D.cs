using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AxMain3D : MonoBehaviour {
	public GameObject MainCamera;
	public GameObject InterCamera;
	public GameObject FaceGroup;
	public GameObject AllDays;
	public GameObject FadeSquare;
	public GameObject InterFadeSquare;
	
	public Material TankMat;
	public RenderTexture rTexture;
	public GameObject TankQuad;

    List<string> todaysTemplate; 
    List<string> completedText;
    Day[] allDays = new Day[10];
    GameObject[] dayObjs = new GameObject[10];
    int zMove = 0, currentDay = 1, maxDays = 10, textState = -1, currentIndex = 1; 
	float lastShowDay = 0.0f, lastTickCheck = 0.0f, dayStart = 0.0f, fadeTime = 0.01f;
	Color guiColor = Color.grey;
	string currentPhrases = "", currentTemplate = "", gameState = "intro"; // gameState = intro, startDay, active, inter, endDay
	bool releaseTyping = false, templateComplete = false, phraseMismatch = false, closeToGlass = false, transOnce = false;
	
	public class Phrase {
		public string triggerType;
		public string triggerVal;
		public string text;
		public Phrase(string trigg, string val, string txt) { // constructor
			if (trigg == "T") // triggered after time passes
				triggerType = "time";
			else if (trigg == "S") // triggered by a specific scene trigger
				triggerType = "scene";
			else if (trigg == "A") // always available
				triggerType = "always";				
			triggerVal = val;
			text = txt;
		}

		public void DebugPhrase() {
			print(triggerType+","+triggerVal+","+text);
		}

	}


	public class Day {
		public List<string> dayTemplates;
		public List<Phrase> dayPhrases;
		public List<Phrase> dayPhrasesActive;

		public Day(string tmp, string phs) { // constructor
			dayTemplates = new List<string>();
			this.dayTemplates.Add(tmp);
			//for (int i=0; i<temps.Length; i++) 
				//this.dayTemplates.Add(templateDB[temps[i]]);

			dayPhrases = new List<Phrase>();
			dayPhrasesActive = new List<Phrase>();
			string[] psp = phs.Split('|');			
			for (int i=0; i<psp.Length; i++) {
				string[] php = psp[i].Split(','); 
				if (php[0] == "A") // always available, add right to active
					this.dayPhrasesActive.Add(new Phrase(php[0],php[1],php[2]));
				else
					this.dayPhrases.Add(new Phrase(php[0],php[1],php[2]));
	
			}
			
		}
		public void DebugDay() {
			foreach (var v in this.dayTemplates) {
    			print(v);
			}
			foreach (var p in this.dayPhrases) {
    			p.DebugPhrase();
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

		if (!templateComplete) {
			for (int i=0; i<allDays[(currentDay-1)].dayPhrasesActive.Count; i++) {
				currentPhrases += allDays[(currentDay-1)].dayPhrasesActive[i].text + "\r\n";
			}
		}

		for (int i=0; i<todaysTemplate.Count; i++) {
			if (i % 2 != 0) { // odd numbered indices are variable
				todaysTemplate[i] = todaysTemplate[i].ToLower();
				if (i == currentIndex && textState == 1 && !releaseTyping) { // this is what we're editing
					if (phraseMismatch) // trying to lock something in that's not an available phrase
				 		currentTemplate += "[<color=#FF0000>" + todaysTemplate[i] + "</color>]";
				 	else
				 		currentTemplate += "[<color=#FFFF00>" + todaysTemplate[i] + "</color>]";
				}
				else if (todaysTemplate[i] != "___")
					currentTemplate += "<color=#87B7C7>" + todaysTemplate[i] + "</color>";
				else
					currentTemplate += "<color=#ABABAB>" + todaysTemplate[i] + "</color>";
			}
			else
				currentTemplate += todaysTemplate[i];
		}
		

    }

    bool CheckPhraseMatch() {
    	//return false;
    	bool foundMatch = false;
    	for (int i=0; i<allDays[(currentDay-1)].dayPhrasesActive.Count; i++) {
			if (allDays[(currentDay-1)].dayPhrasesActive[i].text == todaysTemplate[currentIndex]) {
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
	   				
	    			for (int i=0; i<allDays[(currentDay-1)].dayPhrasesActive.Count; i++) {
						if (allDays[(currentDay-1)].dayPhrasesActive[i].text == todaysTemplate[currentIndex]) {
							allDays[(currentDay-1)].dayPhrasesActive.RemoveAt(i);
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
		dayStart = 0.0f;
		string curStr = "";
		int numEntries = 0;
		currentIndex = 1;
		templateComplete = false;
		textState = -1;
		MainCamera.GetComponent<Camera>().orthographicSize = 4.5f;
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

		MainCamera.SetActive(true);
		InterCamera.SetActive(false);
		
	}	

	void OnGUI() {
		GUI.color = guiColor;
		if (gameState == "intro") {
			GUI.skin.label.fontSize = 18;
			GUI.Label(new Rect((Screen.width/4), Screen.height-300, 600, 300), "I don't quite remember when it started, but I began to see this strange thing every day...");	

			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "[ E ]");	
		}
		else if (gameState == "outro") {
			GUI.skin.label.fontSize = 18;
			GUI.Label(new Rect((Screen.width/2), Screen.height-300, 600, 200), "It walked out of the aquarium one final time, and I never saw it again...");	

			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press ESC to exit)");	
		}
		else if (gameState == "active" && !closeToGlass) {

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
					GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "[ E ]");	
				else if (textState == 1) {
					//GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press Enter when finished)");	
				}
				
				else 
					GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "[ Enter ]");	
				

			}
			else if (textState == -1) {
				GUI.skin.label.fontSize = 15;
				GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "[ E ]");	
			}
		}
		else if (gameState == "inter" && zMove != 4) {
			guiColor = Color.grey;
			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-35, Screen.height-30, 30, 30), "[ E ]");	
		}
	}
	
	void Update () {
		if (gameState == "startDay") {
			guiColor = Color.Lerp(Color.black, Color.grey, Time.time);
			if (FadeSquare.GetComponent<SpriteRenderer>().color.a > 0) {
				FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a-fadeTime);
			}
			else {
				gameState = "active";
			}
		}
		else if (gameState == "endDay") {
			guiColor = Color.Lerp(Color.grey, Color.black, Time.time);
		}
		else if (gameState == "intro") {
			guiColor = Color.grey;
		}

		

		// z-axis camera movement
		if (zMove == 1) { // stepping toward the glass
			
			if (MainCamera.GetComponent<Camera>().orthographicSize > 3) {
				MainCamera.GetComponent<Camera>().orthographicSize = MainCamera.GetComponent<Camera>().orthographicSize - (float)0.05;
			}
			else {
				//closeToGlass = true;
				zMove = 2;
			}

		}
		else if (zMove == -1) { // stepping back from the glass
	
		
			if (MainCamera.GetComponent<Camera>().orthographicSize < 4.5) 
				MainCamera.GetComponent<Camera>().orthographicSize = MainCamera.GetComponent<Camera>().orthographicSize + (float)0.05;
			else {
				zMove = 0;
				closeToGlass = false;
			}
				
			
		}
		else if (zMove == 3) { // fading scene out to interstitial
			if (FadeSquare.GetComponent<SpriteRenderer>().color.a < 1) {
				FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a+fadeTime);
			}
			else {

				// end game checks here

				MainCamera.SetActive(false);
				InterCamera.SetActive(true);
				//gameState = "inter";

				if (InterFadeSquare.GetComponent<SpriteRenderer>().color.a > 0) {
					InterFadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,InterFadeSquare.GetComponent<SpriteRenderer>().color.a-fadeTime);
				}
				else { // done
					templateComplete = false;
					zMove = 0;
					gameState = "inter";

				}
			}
		}
		else if (zMove == 4) { // fading out from intersitial to next day
			if (InterFadeSquare.GetComponent<SpriteRenderer>().color.a < 1) {
				InterFadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,InterFadeSquare.GetComponent<SpriteRenderer>().color.a+fadeTime);
			}
			else {
				if (!transOnce) {
					InterCamera.SetActive(false);
					MainCamera.SetActive(true);
					NewDayUpdate();
					transOnce = true;		
				}
				
				if (FadeSquare.GetComponent<SpriteRenderer>().color.a > 0) {
					FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a-(float)0.02);
				}
				else { // done 
					gameState = "startDay";
					zMove = 0;
					transOnce = false;

				}
			}
		}

		// Check for time-based word additions
		if (Time.time - lastTickCheck >= 1 && gameState == "active") { // so we're not looping through the list array every frame, just every second
			for (int i=0; i<allDays[(currentDay-1)].dayPhrases.Count; i++) {
				if (dayStart != 0.0 && allDays[(currentDay-1)].dayPhrases[i].triggerType == "time" && float.Parse(allDays[(currentDay-1)].dayPhrases[i].triggerVal) <= (Time.time - dayStart)) {
					allDays[(currentDay-1)].dayPhrasesActive.Add(allDays[(currentDay-1)].dayPhrases[i]);
					allDays[(currentDay-1)].dayPhrases.RemoveAt(i);
					i--;
					UpdateStrings();
				}
				/* debugging timer
				else if (allDays[(currentDay-1)].dayPhrases[i].triggerType == "time") {
					print(allDays[(currentDay-1)].dayPhrases[i].triggerType +","+allDays[(currentDay-1)].dayPhrases[i].triggerVal+","+allDays[(currentDay-1)].dayPhrases[i].text + " | " + (Time.time - dayStart) + ",(" + dayStart + ")");
				}
				*/
			}
			lastTickCheck = Time.time;
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
	    	if (dayStart == 0.0)
	    		dayStart = Time.time;
    	}
    	else if (Input.GetKeyUp("e") && gameState == "intro") { 
	    	gameState = "startDay";
    	}
    	else if (Input.GetKeyUp("e") && gameState == "inter") {
    		zMove = 4; 
    	}
	    
    	if (textState != 1) { // only if we're not typing something
	    	if (Input.GetKey("w") && zMove == 0 && !closeToGlass && gameState == "active") {
	    		SendAction("stepFoward");
	    		zMove = 1;
	    		closeToGlass = true;
	    	}
			if (Input.GetKey("s") && zMove == 2 && closeToGlass && gameState == "active") {
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
