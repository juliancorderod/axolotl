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

	public float fadeOutTime = 0.001f;
	public float fadeInTime = 0.005f;
	public float waitTankToInter = 0.5f;
	public float waitInterToTank = 0.5f;
	public int startDay = 1;

    List<List<string>> todaysTemplate;
    List<int[]> blankMarkers;
    List<int> blanksPerSentence;
    List<string> completedText;
    Day[] allDays = new Day[10];
    GameObject[] dayObjs = new GameObject[10];
	Color guiColor = Color.grey;
	string currentPhrases = "", currentTemplate = "", gameState = "title", introText = "", outroText = ""; // gameState = title, intro, startDay, active, inter, endDay
	float lastShowDay = 0.0f, lastTickCheck = 0.0f, dayStart = 0.0f, stareTime = 0.0f, holdTime = 0.0f, faceResetX = 0.0f, camResetX = 0.0f;
    int zMove = 0, currentDay = 1, maxDays = 10, textState = -1, currentIndex = 0, currentSent = 0, todaysBlanks, currentBlankInSent; 
	bool releaseTyping = false, templateComplete = false, phraseMismatch = false, transOnce = false, needsEntry = false;
	public bool closeToGlass = false;

	public class Phrase {
		public string triggerType;
		public string triggerVal;
		public string text;
		public bool wasTriggered;
		public Phrase(string trigg, string val, string txt) { // constructor
			if (trigg == "T") // triggered after time passes
				triggerType = "time";
			else if (trigg == "S") // triggered by a specific scene trigger
				triggerType = "scene";
			else if (trigg == "A") // always available
				triggerType = "always";
			else if (trigg == "C") // always available
				triggerType = "close";				
			triggerVal = val;
			text = txt;
			wasTriggered = false;
		}
	}


	public class Day {
		public List<string> dayTemplates;
		public List<Phrase> dayPhrases;
		public List<Phrase> dayPhrasesActive;
		public int dayID;
		public Day(string tmp, string phs, int id) { // constructor
			dayTemplates = new List<string>();
			dayID = id;
			string[] tsp = tmp.Split('|');			
			for (int i=0; i<tsp.Length; i++) 
				this.dayTemplates.Add(tsp[i]);
			
			dayPhrases = new List<Phrase>();
			dayPhrasesActive = new List<Phrase>();
			string[] psp = phs.Split('|');			
			for (int i=0; i<psp.Length; i++) {
				string[] php = psp[i].Split(','); 
				if (php[0] == "A") // always available, add directly to active
					this.dayPhrasesActive.Add(new Phrase(php[0],php[1],php[2]));
				else
					this.dayPhrases.Add(new Phrase(php[0],php[1],php[2]));
	
			}	
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
    bool IsBlank(int x, int y) {
    	
		for (int k=0;k<blankMarkers.Count;k++) {
			if (x == blankMarkers[k][0] && y == blankMarkers[k][1]) {
				return true;
			}
		}
		return false;
    }
    void UpdateStrings() {
		currentPhrases = "";
		currentTemplate = "";


		if (!templateComplete) {
			for (int i=0; i<allDays[(currentDay-1)].dayPhrasesActive.Count; i++) 
				currentPhrases += allDays[(currentDay-1)].dayPhrasesActive[i].text + "\r\n";
		}

		for (int i=0; i<todaysTemplate.Count; i++) {
			if (currentSent >= i) {
				for (int j=0; j<todaysTemplate[i].Count; j++) {
					if (blankMarkers[currentIndex][0] == i && blankMarkers[currentIndex][1] == j)
						needsEntry = true;
					if (IsBlank(i,j)) {
						todaysTemplate[i][j] = todaysTemplate[i][j].ToLower();
						if (blankMarkers[currentIndex][0] == i && blankMarkers[currentIndex][1] == j && textState == 1 && !releaseTyping) { // this is what we're editing
							if (phraseMismatch) // trying to lock something in that's not an available phrase
						 		currentTemplate += "[<color=#FF0000>" + todaysTemplate[i][j] + "</color>]";
						 	else
						 		currentTemplate += "[<color=#FFFF00>" + todaysTemplate[i][j] + "</color>]";
						}
						else if (todaysTemplate[i][j] != "___")
							currentTemplate += "<color=#87B7C7>" + todaysTemplate[i][j] + "</color>";
						else
							currentTemplate += "<color=#ABABAB>" + todaysTemplate[i][j] + "</color>";
					}
					else
						currentTemplate += todaysTemplate[i][j];
				}
				currentTemplate += "\n";
			}
		}
    }

    public bool TriggerScenePhrase(string phraseID) {
    	if (gameState == "active") {
	    	for (int i=0; i<allDays[(currentDay-1)].dayPhrases.Count; i++) {
	    		if (allDays[(currentDay-1)].dayPhrases[i].triggerType == "scene" && allDays[(currentDay-1)].dayPhrases[i].triggerVal == phraseID && !allDays[(currentDay-1)].dayPhrases[i].wasTriggered) {
	    			allDays[(currentDay-1)].dayPhrases[i].wasTriggered = true;
	    			allDays[(currentDay-1)].dayPhrasesActive.Add(allDays[(currentDay-1)].dayPhrases[i]);
	    			return true;
	    		}
	    	}	
    	}	
    	return false;
    }

    bool CheckPhraseMatch() {
    	//return false;
    	bool foundMatch = false;
    	for (int i=0; i<allDays[(currentDay-1)].dayPhrasesActive.Count; i++) {
			if (allDays[(currentDay-1)].dayPhrasesActive[i].text == todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]]) {
				foundMatch = true;
				break;
			}
		}
		return foundMatch;
    }

    void EnterText(KeyCode ltr) {
    	phraseMismatch = false;
    	if (ltr >= KeyCode.A && ltr <= KeyCode.Z) { // we only accept alphabetical characters
    		todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] += ltr;
    	}
    	else if (ltr == KeyCode.Backspace && todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]].Length > 0) {
    		todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] = todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]].Substring(0, todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]].Length - 1);
    	}
    	else if(ltr == KeyCode.Return) {
    		if (todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]].Length == 0) {
    			releaseTyping = true;
    			todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] = "___";
    		}
	   		else { // attempt to lock text in
	   			if (CheckPhraseMatch()) {
	   				
	    			for (int i=0; i<allDays[(currentDay-1)].dayPhrasesActive.Count; i++) {
						if (allDays[(currentDay-1)].dayPhrasesActive[i].text == todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]]) {
							allDays[(currentDay-1)].dayPhrasesActive.RemoveAt(i);
							break;
						}
					}
					currentBlankInSent++;
					releaseTyping = true;
	    			currentIndex++;
	    			if (currentBlankInSent >= blanksPerSentence[currentSent]) {
	    				currentSent++;
	    				currentBlankInSent = 0;	
	    			}
	    			
	    			if (currentIndex >= todaysBlanks) {
	    				templateComplete = true;
	    				currentIndex--;
	    			}
	   			}
	   			else
	   				phraseMismatch = true;
	    		
    		}
    		
    	}
    	UpdateStrings();
    }

	void NewDayUpdate() {
		string curStr = "";
		int numEntries = 0;
		int numBlanks = 0;

		todaysTemplate = new List<List<string>>();
		blanksPerSentence = new List<int>();
		blankMarkers = new List<int[]>();
		todaysBlanks = 0;
		dayStart = 0.0f;
		currentIndex = 0;
		currentSent = 0;
		templateComplete = false;
		needsEntry = false;
		textState = -1;
		currentBlankInSent = 0;
		MainCamera.GetComponent<Camera>().orthographicSize = 4.5f;
		FaceGroup.transform.localPosition = new Vector3(faceResetX, FaceGroup.transform.localPosition.y, FaceGroup.transform.localPosition.z);	
		FaceGroup.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
		MainCamera.transform.localPosition = new Vector3(camResetX, MainCamera.transform.localPosition.y, MainCamera.transform.localPosition.z);	
		for (int i=0; i<allDays[(currentDay-1)].dayTemplates.Count; i++) { // count the number of phrases in template
			numEntries = 0;
			numBlanks = 0;
			todaysTemplate.Add(new List<string>());
			for (int j=0; j<allDays[(currentDay-1)].dayTemplates[i].Length;j++) {
				if (allDays[(currentDay-1)].dayTemplates[i][j] == '_') {
					todaysTemplate[i].Add(curStr);
					if (curStr != "")
						numEntries++;
					todaysTemplate[i].Add("___");
					todaysBlanks++;
					curStr = "";
					numEntries++;
					numBlanks++;
					blankMarkers.Add(new int[2]{i,numEntries-1});
				}
				else {
					curStr += allDays[(currentDay-1)].dayTemplates[i][j];

				}
				if (j == allDays[(currentDay-1)].dayTemplates[i].Length-1) {
					todaysTemplate[i].Add(curStr);
					curStr = "";
				}
			}
			blanksPerSentence.Add(numBlanks);
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

		// disable all ALLDAYS children
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
			child.gameObject.SetActive(true);
	

		// add all days to dayObjs array for quicker access, then turn them off
		for (int i=0;i<10;i++) {
			GameObject tmpObj = GameObject.Find("/ALLDAYS/day_"+(i+1));
			dayObjs[i] = tmpObj;
			tmpObj.SetActive(false);
		}

		string[] allTmps = new string[12];
		string[] allPhrases = new string[10];
		currentDay = startDay;
		faceResetX = FaceGroup.transform.localPosition.x;
		camResetX = MainCamera.transform.localPosition.x;
		// populate the template and phrase arrays
		ReadFile("Assets/template.txt", allTmps);
	    ReadFile("Assets/phrase.txt", allPhrases);
		
	    // Set up the text for Days
	    introText = allTmps[0];
	    outroText = allTmps[11];
	    for (int i=0;i<10;i++) 
	    	allDays[i] = new Day(allTmps[i+1],allPhrases[i],i+1);
		NewDayUpdate();

		MainCamera.SetActive(false);
		InterCamera.SetActive(true);
		
	}	

	void OnGUI() {
		var centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.UpperLeft;

		GUI.color = guiColor;
		if (gameState == "title" && zMove == 0) {
			guiColor = Color.white;
			GUI.skin.label.fontSize = 36;
			GUI.Label(new Rect((Screen.width/6), (Screen.height/2), 600, 300), "The Axolotl");	

			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-135, Screen.height-30, 200, 200), "[ Enter to Begin ]");	

		}
		else if (gameState == "intro") {
			GUI.skin.label.fontSize = 18;
			GUI.Label(new Rect((Screen.width/4), Screen.height-300, 600, 300), introText);	

			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width/4, Screen.height-200, 400, 200), "(Move the mouse to look left and right)\n(Press Space to Toggle Zoom)\n(Press Enter to Advance)");	

		}
		else if (gameState == "outro") {
			GUI.skin.label.fontSize = 18;
			GUI.Label(new Rect((Screen.width/5), Screen.height-75, 700, 500), outroText);	

			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-75, Screen.height-30, 200, 200), "[ Escape ]");	
		}
		else if (gameState == "active" && !closeToGlass) {

			if (Time.time - lastShowDay < 2 && gameState == "active") {
				GUI.skin.label.fontSize = 14;
				GUI.Label(new Rect(Screen.width-120, 10, 120, 20), "Current Day: " + currentDay.ToString());	
			}

			if (textState > -1) {

				// show current day template
				GUI.skin.label.fontSize = 18;
				//GUI.skin.GUIText.alignment = TextAlignment.Center;
				GUI.Label(new Rect((Screen.width/5), Screen.height-75, 700, 500), currentTemplate);	

				// show available phrases 
				GUI.skin.label.fontSize = 15;
				GUI.Label(new Rect(Screen.width-100, Screen.height-200, 100, 600), currentPhrases);	

				if (textState == 1) { // we're typing
					Event e = Event.current;
		        	if (e.isKey && e.type == EventType.KeyUp) // key up
		        		EnterText(e.keyCode);
				}
				
				GUI.skin.label.fontSize = 15;
				if (templateComplete) 
					GUI.Label(new Rect(Screen.width-105, Screen.height-30, 200, 200), "[ Enter ] -->");	
				else if (textState == 1) {
					//GUI.Label(new Rect(Screen.width-250, Screen.height-30, 200, 200), "(Press Enter when finished)");	
				}
				
				else {
					if (needsEntry)
						GUI.Label(new Rect(Screen.width-60, Screen.height-30, 200, 200), "[ Enter ]");	
					else
						GUI.Label(new Rect(Screen.width-60, Screen.height-30, 200, 200), "[ Enter ]");	
				}
				

			}
			else if (textState == -1) {
				GUI.skin.label.fontSize = 15;
				GUI.Label(new Rect(Screen.width-60, Screen.height-30, 200, 200), "[ Enter ]");	
			}
		}
		else if (gameState == "inter" && zMove != 4) {
			guiColor = Color.grey;
			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-60, Screen.height-30, 200, 200), "[ Enter ]");	
		}
	}
	
	void Update () {
		if (gameState == "startDay") {
			guiColor = Color.Lerp(Color.black, Color.grey, Time.time);
			if (FadeSquare.GetComponent<SpriteRenderer>().color.a > 0) {
				FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a-fadeInTime);
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
			
			if (MainCamera.GetComponent<Camera>().orthographicSize > 3) 
				MainCamera.GetComponent<Camera>().orthographicSize = MainCamera.GetComponent<Camera>().orthographicSize - (float)0.05;
			
			else 
				zMove = 2;

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

			if (FaceGroup.transform.localPosition.x < 12) { // face walking
				FaceGroup.transform.localPosition = new Vector3(FaceGroup.transform.localPosition.x + (float)0.02, FaceGroup.transform.localPosition.y, FaceGroup.transform.localPosition.z);	
				FaceGroup.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,FaceGroup.GetComponent<SpriteRenderer>().color.a-(float)0.001);
			}

			if (MainCamera.transform.localPosition.x < 12) { 
				MainCamera.transform.localPosition = new Vector3(MainCamera.transform.localPosition.x + (float)0.02, MainCamera.transform.localPosition.y, MainCamera.transform.localPosition.z);	
			}

			if (FadeSquare.GetComponent<SpriteRenderer>().color.a < 1) {
				FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a+fadeOutTime);
			}
			else {
				if (Time.time - holdTime >= waitTankToInter) {
					if (!transOnce) {
						transOnce = true;
						transform.GetComponent<audioManager>().playCig();
						transform.GetComponent<audioManager>().playSmokeAmbient();
						MainCamera.SetActive(false);
						InterCamera.SetActive(true);
					}
					
					if (InterFadeSquare.GetComponent<SpriteRenderer>().color.a > 0) {
						InterFadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,InterFadeSquare.GetComponent<SpriteRenderer>().color.a-fadeInTime);
					}
					else { // done
						templateComplete = false;
						zMove = 0;
						gameState = "inter";
						transOnce = false;
						holdTime = 0.0f;
					}
				}
			}
		}
		else if (zMove == 4) { // fading out from intersitial to next day
			if (InterFadeSquare.GetComponent<SpriteRenderer>().color.a < 1) {
				InterFadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,InterFadeSquare.GetComponent<SpriteRenderer>().color.a+fadeOutTime);
			}
			else {
				if (Time.time - holdTime >= waitInterToTank) {
					if (!transOnce) {
						
						InterCamera.SetActive(false);
						MainCamera.SetActive(true);
						NewDayUpdate();
						transOnce = true;
						transform.GetComponent<audioManager>().playTankBubbles();
    					transform.GetComponent<audioManager>().playTankAmbient();		
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
		}
		else if (zMove == 5) { // face is walking away
			if (FaceGroup.transform.localPosition.x < 12) { 
				FaceGroup.transform.localPosition = new Vector3(FaceGroup.transform.localPosition.x + (float)0.05, FaceGroup.transform.localPosition.y, FaceGroup.transform.localPosition.z);	
				FaceGroup.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,FaceGroup.GetComponent<SpriteRenderer>().color.a-(float)0.01);
			}
			else { // end-game triggers

			}
		}
		else if (zMove == 6) { // fade from title to intro
			if (InterFadeSquare.GetComponent<SpriteRenderer>().color.a < 1) {
				InterFadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,InterFadeSquare.GetComponent<SpriteRenderer>().color.a+fadeInTime);
			}
			else {
				if (!transOnce) {
					InterCamera.SetActive(false);
					MainCamera.SetActive(true);
					transOnce = true;		
				}
					
				gameState = "intro";
				zMove = 0;
				transOnce = false;	
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
				else if (stareTime != 0.0 && allDays[(currentDay-1)].dayPhrases[i].triggerType == "close" && float.Parse(allDays[(currentDay-1)].dayPhrases[i].triggerVal) <= (Time.time - stareTime)) {
					// add sound notifier of new word?
					allDays[(currentDay-1)].dayPhrasesActive.Add(allDays[(currentDay-1)].dayPhrases[i]);
					allDays[(currentDay-1)].dayPhrases.RemoveAt(i);
					i--;
					UpdateStrings();
				}
			}
			lastTickCheck = Time.time;
		}
		
		// Keystrokes

		if (Input.GetKey("escape"))
    		Application.Quit();

		if (Input.GetKeyUp("return") && textState == 0 && !templateComplete && needsEntry) {
			// entering typing mode
			textState = 1;
			todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] = "";
			UpdateStrings();
		}

 		
		
		if (Input.GetKeyUp("return") && textState == -1 && gameState == "active") { 
	    	textState = 0;
	    	UpdateStrings();
	    	if (dayStart == 0.0)
	    		dayStart = Time.time;
    	}
    	else if (Input.GetKeyUp("return") && textState == 0 && gameState == "active" && !needsEntry) { 
    		currentSent++;
    		UpdateStrings();
    	}
    	else if (Input.GetKeyUp("return") && gameState == "intro") { 
    		transform.GetComponent<audioManager>().playTankBubbles();
    		transform.GetComponent<audioManager>().playTankAmbient();
	    	gameState = "startDay";
    	}
    	else if (Input.GetKeyUp("return") && gameState == "inter") {
    		zMove = 4; 
    		holdTime = Time.time;
    		transform.GetComponent<audioManager>().stopCig();
			transform.GetComponent<audioManager>().stopSmokeAmbient();
			transform.GetComponent<MouseParallax>().mouseParallaxControl = true;

    	}
	    else if (Input.GetKeyUp("return") && gameState == "title") {
    		zMove = 6; 
    	}

    	if (textState != 1) { // only if we're not typing something
	    	if (Input.GetKey("space") && zMove == 0 && !closeToGlass && gameState == "active") {
	    		SendAction("stepFoward");
	    		zMove = 1;
	    		closeToGlass = true;
	    		stareTime = Time.time;
	    		transform.GetComponent<audioManager>().closeToTank();
	    	}
			if (Input.GetKey("space") && zMove == 2 && closeToGlass && gameState == "active") {
	    		SendAction("stepBack");
	    		zMove = -1;    		
	    		transform.GetComponent<audioManager>().awayFromTank();
	    		stareTime = 0.0f;
			}
	    	if (Input.GetKeyUp("return")) { 
	    		if (templateComplete && currentDay < maxDays) { // exiting day -> interstitial
		    		transform.GetComponent<audioManager>().stopTankBubbles();
    				transform.GetComponent<audioManager>().stopTankAmbient();
    				transform.GetComponent<audioManager>().playWalking();
		    		zMove = 3;
		    		holdTime = Time.time;
		    		currentDay = currentDay + 1;
		    		lastShowDay = Time.time;
		    		gameState = "endDay";
		    		transform.GetComponent<MouseParallax>().mouseParallaxControl = false;

	    		}
	    		else if (templateComplete && currentDay == maxDays) {
	    			transform.GetComponent<MouseParallax>().mouseParallaxControl = false;
	    			zMove = 5;
	    			gameState = "outro";

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