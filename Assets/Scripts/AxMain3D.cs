using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AxMain3D : MonoBehaviour {
	public GameObject MainCamera;
	public GameObject InterCamera;
	public GameObject FaceGroup;
	public GameObject AllDays;
	public GameObject FadeSquare;
	public GameObject InterFadeSquare;
	public GameObject UICanvas;
	
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
    bool[] fxState = new bool[3];
    GameObject[] dayObjs = new GameObject[10];
    GameObject UI_title, UI_main, UI_intro, UI_words, UI_prompts, UI_storyL, UI_storyR;
	string currentPhrases = "", currentTemplate = "", gameState = "title", outroText = "", lastPrompt = ""; // gameState = title, intro, startDay, active, endDay, inter
	float lastTickCheck = 0.0f, dayStart = 0.0f, stareTime = 0.0f, holdTime = 0.0f, faceResetX = 0.0f, camResetX = 0.0f, lastDayUIDelay = 0.0f;
    int zMove = 0, currentDay = 1, maxDays = 10, textState = -1, currentIndex = 0, currentSent = 0, todaysBlanks, currentBlankInSent, dayBreak = 0; 
	bool releaseTyping = false, templateComplete = false, phraseMismatch = false, transOnce = false, needsEntry = false, canLeaveInter = false, confirmExit = false;
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

    string[] ReadString(string inp) {
    	string[] returnstr = inp.Split('\n');
    	return returnstr;
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
		int checkent = 0;
		for (int i=0; i<todaysTemplate.Count; i++) {
			if (currentSent == i) {
				for (int j=0; j<todaysTemplate[i].Count; j++) {
					if (blankMarkers[currentIndex][0] == i && blankMarkers[currentIndex][1] == j)
						checkent++;
					if (IsBlank(i,j)) {
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
		if (checkent > 0)
			needsEntry = true;
		else
			needsEntry = false;
		UI_main.GetComponent<UnityEngine.UI.Text>().text = currentTemplate;
		UI_words.GetComponent<UnityEngine.UI.Text>().text = currentPhrases;

		UI_storyL.GetComponent<UnityEngine.UI.Text>().text = "";
		UI_storyR.GetComponent<UnityEngine.UI.Text>().text = "";
		for (int i=0;i<completedText.Count;i++) {
			if (dayBreak != 0 && i >= dayBreak)
				UI_storyR.GetComponent<UnityEngine.UI.Text>().text += completedText[i];
			else
				UI_storyL.GetComponent<UnityEngine.UI.Text>().text += completedText[i];
		}
    }

    public bool TriggerScenePhrase(string phraseID) {
    	if (gameState == "active") {
	    	for (int i=0; i<allDays[(currentDay-1)].dayPhrases.Count; i++) {
	    		if (allDays[(currentDay-1)].dayPhrases[i].triggerType == "scene" && allDays[(currentDay-1)].dayPhrases[i].triggerVal == phraseID && !allDays[(currentDay-1)].dayPhrases[i].wasTriggered) {
	    			allDays[(currentDay-1)].dayPhrases[i].wasTriggered = true;
	    			allDays[(currentDay-1)].dayPhrasesActive.Add(allDays[(currentDay-1)].dayPhrases[i]);
	    			if (!templateComplete)
						transform.GetComponent<audioManager>().playChime();
					UpdateStrings();
	    			return true;
	    		}
	    	}	
    	}	
    	return false;
    }

    bool CheckPhraseMatch() {
    	bool foundMatch = false;
    	for (int i=0; i<allDays[(currentDay-1)].dayPhrasesActive.Count; i++) {
			if (allDays[(currentDay-1)].dayPhrasesActive[i].text == todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]]) {
				foundMatch = true;
				break;
			}
		}
		return foundMatch;
    }

    string CleanString(string instr) { // we call this to remove formatting from text before we put it into completedText
    	instr = Regex.Replace(instr, "<.*?>", string.Empty); // remove rich text formatting
    	instr = Regex.Replace(instr, @"\[|\]", string.Empty); // remove brackets
    	return instr;	
    }

    void EnterText(KeyCode ltr, bool withShift) {
    	phraseMismatch = false;
    	if (ltr >= KeyCode.A && ltr <= KeyCode.Z) { // we only accept alphabetical characters
    		string postenter = "";
    		if (withShift) {
    			todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] += ltr;
    		}
    		else if (!withShift) {

    			postenter = "" + ltr;
    			postenter = postenter.ToLower();
    			todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] += postenter;
    		}
    	}
    	//else if (ltr == KeyCode.Space)
    		//todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] += " ";
    	else if (ltr == KeyCode.Backspace && todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]].Length > 0) {
    		todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] = todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]].Substring(0, todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]].Length - 1);
    	}
    	else if (ltr == KeyCode.Return) {
    		if (todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]].Length == 0) {
    			releaseTyping = true;
    			todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] = "___";
    		}
	   		else { // attempt to lock text in
	   			if (CheckPhraseMatch()) {
	   				
	   				// remove the phrase we used
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
	    				completedText.Add(CleanString(currentTemplate));
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
		fxState = new bool[3]{false,false,false}; // fish, chrome, glass
		todaysBlanks = 0;
		dayStart = 0.0f;
		currentIndex = 0;
		currentSent = 0;
		templateComplete = false;
		needsEntry = false;
		canLeaveInter = false;
		textState = -1;
		currentBlankInSent = 0;
		MainCamera.GetComponent<Camera>().orthographicSize = 4.5f;
		FaceGroup.transform.localPosition = new Vector3(faceResetX, FaceGroup.transform.localPosition.y, FaceGroup.transform.localPosition.z);	
		FaceGroup.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
		MainCamera.transform.localPosition = new Vector3(camResetX, MainCamera.transform.localPosition.y, MainCamera.transform.localPosition.z);	
		UI_storyL.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,0f);
		UI_storyR.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,0f);
		UI_main.SetActive(false);
		UI_words.SetActive(false);
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

		/*for (int i=0;i<blankMarkers.Count;i++) {
			print(blankMarkers[i][0]+","+blankMarkers[i][1]);
		}*/

		// Change alpha of tankQuad
		Color color = TankMat.color;
		color.a = 1.0f-(0.07f*currentDay);
		TankMat.color = color;

		// Change alpha of face
		FaceGroup.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,(float)0.2+(0.06f*currentDay));


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
		completedText = new List<string>();
		currentDay = startDay;
		faceResetX = FaceGroup.transform.localPosition.x;
		camResetX = MainCamera.transform.localPosition.x;
		
		// hard coding templates and phrases
		string hardPhrases = "A,0,jumped|A,0,drank|S,1,floated|T,4,spoke|T,8,laughed|C,6,stared|S,2,waved|T,12,wondered|C,11,unraveled\nA,0,stupid|A,0,happy|T,4,jittery|S,1,stoic|S,2,soulless|C,8,homeless|T,10,confused|C,14,hopeless|T,14,hungry\nS,1,salty|S,2,pungent|A,0,chewy|A,0,furious|T,5,curly|S,3,heavy|T,11,lonely|C,8,empty|C,16,dull|S,4,sinking|T,17,enclosed\nS,1,grasses|S,2,coral|S,3,seaweed|A,0,oceans|T,4,animals|T,10,creatures|T,17,people|C,5,reflections|C,10,you|C,10,I\nS,1,sticky|A,0,curious|A,0,dark|A,0,big|S,2,small|T,6,googly|S,3,carefree|C,6,bitter|T,10,lost|S,4,desolate|T,16,focused|C,10,real\nA,0,squishy|A,0,fast|S,1,tall|A,0,nice|T,5,calm|S,2,small|C,6,anxious|C,10,caged|C,16,buried|S,3,deserted|T,12,detache\nA,0,hands|A,0,freckles|A,0,shape|T,6,softness|T,10,gills|T,14,eyes|T,17,face|T,20,glare|C,6,stillness|C,13,turmoil\nC,3,affections|C,7,worlds|C,15,feelings|C,19,views|C,22,panic|C,26,desperation\nC,3,space|C,8,glass|C,16,air|C,19,water|C,23,silence\nC,7,me|C,7,us|C,7,me";
		string hardTemplates = "I don't remember how or when it started, but I started seeing it every day...\nWe met across the glass.|Remaining so still, it _.|With its expressionless face, I _ and did not feel judged.\nIts _ eyes drew me into a mystery.|Looking at my reflection, I felt _.|Is it _?\nThere was a single _ rock.|Translucent, _ seaweed.|More _ faces began to stare with it.|But never as long, and never as close.\nWhy do _ stare?|Everyday _ would stay and look at me for hours, judging me.|But staring with _ feels close.\nThe eyes I became addicted to started to look like my own.|They were _, _, and _.|In front of those eyes, I was _.\nThe new log seems _.|Does it feel _ now that the coral has moved?|Does it like its _ home?|Home makes me feel _.\nI wonder if it can recognize me now.|I don’t think I can ever forget its _.|But does it feel the same way?\nI feel like we are beginning to understand each other’s _.|Or maybe I’m just projecting _.|But I’m starting to think that we share in our _.\nThe _ between us started to disappear.|I’m face to face with it.|And I’m still lost in its eyes.\nI looked into its eyes and the mystery became clear.|The words it has been telling _ began to ring in my head.|''Save _.''|''Save _.''\nThe stillness between us became silent words. It was hard to find an understanding in tranquility. But I find myself waiting to hear your silent voice again.";


		// populate the template and phrase arrays
		//ReadFile("Assets/template.txt", allTmps);
	    //ReadFile("Assets/phrase.txt", allPhrases);
	    allPhrases = ReadString(hardPhrases);
	    allTmps = ReadString(hardTemplates);

	    /*for (int i=0;i<allTmps.Length;i++)
	    	print(allTmps[i]);
	    for (int i=0;i<allPhrases.Length;i++)
	    	print(allPhrases[i]);*/

	    // Setup our UI Canvas objects
		UI_title = GameObject.Find("/Canvas/title");
		UI_main = GameObject.Find("/Canvas/main");
		UI_intro = GameObject.Find("/Canvas/intro");
		UI_words = GameObject.Find("/Canvas/words");
		UI_prompts = GameObject.Find("/Canvas/prompts");
		UI_storyL = GameObject.Find("/Canvas/storyLeft");
		UI_storyR = GameObject.Find("/Canvas/storyRight");
		UI_main.SetActive(false);
		UI_words.SetActive(false);
		//UI_prompts.SetActive(false);
		UI_intro.SetActive(false);
		UI_storyL.SetActive(false);
		UI_storyR.SetActive(false);

	    // Set up the text for Days
	    UI_intro.GetComponent<UnityEngine.UI.Text>().text = allTmps[0];
	    outroText = allTmps[11];
	    for (int i=0;i<10;i++) 
	    	allDays[i] = new Day(allTmps[i+1],allPhrases[i],i+1);
		NewDayUpdate();

		MainCamera.SetActive(false);
		InterCamera.SetActive(true);
		transform.GetComponent<MouseParallax>().mouseParallaxControl = false;
		
	}	

	void OnGUI() {
		
		if (gameState == "active" && !closeToGlass) {

			if (textState > -1) {

				if (textState == 1) { // we're typing
					bool wShift = false;
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
						wShift = true;
					}
					Event e = Event.current;
		        	if (e.isKey && e.type == EventType.KeyUp) // key up
		        		EnterText(e.keyCode,wShift);
				}
				/*
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
				*/

			}
			/*
			else if (textState == -1) {
				GUI.skin.label.fontSize = 15;
				GUI.Label(new Rect(Screen.width-60, Screen.height-30, 200, 200), "[ Enter ]");	
			}*/
		}
		/*
		else if (gameState == "inter" && zMove != 4) {
			guiColor = Color.grey;
			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(Screen.width-60, Screen.height-30, 200, 200), "[ Enter ]");	
		}
		*/
	}
	
	void Update () {
		if (gameState == "startDay") {
			if (FadeSquare.GetComponent<SpriteRenderer>().color.a > 0) {
				FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a-fadeInTime);
			}
			else {
				gameState = "active";
			}
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
				UI_main.SetActive(true);
				UI_words.SetActive(true);
			}
				
			
		}
		else if (zMove == 3) { // fading scene out to interstitial

			if (FaceGroup.transform.localPosition.x < 12) { // face walking
				FaceGroup.transform.localPosition = new Vector3(FaceGroup.transform.localPosition.x + (float)0.02, FaceGroup.transform.localPosition.y, FaceGroup.transform.localPosition.z);	
				FaceGroup.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,FaceGroup.GetComponent<SpriteRenderer>().color.a-(float)0.002);
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
						UI_storyL.SetActive(true);
						UI_storyR.SetActive(true);
						transform.GetComponent<imageEffects>().FishEyeOn(false);
						transform.GetComponent<imageEffects>().ChromAbOn(false);
						transform.GetComponent<imageEffects>().glassColorOn(false);
					}
					
					if (UI_storyL.GetComponent<UnityEngine.UI.Text>().color.a < 1)
						UI_storyL.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,UI_storyL.GetComponent<UnityEngine.UI.Text>().color.a+0.05f);

					if (UI_storyR.GetComponent<UnityEngine.UI.Text>().color.a < 1)
						UI_storyR.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,UI_storyR.GetComponent<UnityEngine.UI.Text>().color.a+0.05f);


					if (InterFadeSquare.GetComponent<SpriteRenderer>().color.a > 0) {
						InterFadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,InterFadeSquare.GetComponent<SpriteRenderer>().color.a-fadeInTime);
					}
					else { // done
						templateComplete = false;
						zMove = 0;
						gameState = "inter";
						transOnce = false;
						holdTime = 0.0f;
						canLeaveInter = true;
					}
				}
			}
		}
		else if (zMove == 4) { // fading out from intersitial to next day
			if (UI_storyL.GetComponent<UnityEngine.UI.Text>().color.a > 0)
				UI_storyL.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,UI_storyL.GetComponent<UnityEngine.UI.Text>().color.a-0.05f);
			
			if (UI_storyR.GetComponent<UnityEngine.UI.Text>().color.a > 0)
				UI_storyR.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,UI_storyR.GetComponent<UnityEngine.UI.Text>().color.a-0.05f);

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
						UI_storyL.SetActive(false);
						UI_storyR.SetActive(false);
					}
				}
			}
		}
		else if (zMove == 5) { // face is walking away
			if (FaceGroup.transform.localPosition.x < 10) { 
				FaceGroup.transform.localPosition = new Vector3(FaceGroup.transform.localPosition.x + (float)0.02, FaceGroup.transform.localPosition.y, FaceGroup.transform.localPosition.z);	
				FaceGroup.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,FaceGroup.GetComponent<SpriteRenderer>().color.a-(float)0.005);
			}
			else { // end-game triggers
				if (!transOnce) {
					lastDayUIDelay = Time.time;
					UpdateStrings();
					UI_storyL.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,0f);
					UI_storyR.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,0f);
					UI_storyL.SetActive(true);
					UI_storyR.SetActive(true);
					UI_main.SetActive(false);
					transOnce = true;
					GameObject fb = GameObject.Find("/FarBackInsideAquarium");
					fb.transform.GetComponent<BackgroundFadeIn>().GameEndScreenSetUp();
				}
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
					UI_intro.SetActive(true);
				}
					
				gameState = "intro";
				zMove = 0;
				transOnce = false;	
			}

		}
		else if (zMove == 7) { // fade out from last day to restart
			if (FadeSquare.GetComponent<SpriteRenderer>().color.a < 1) {
				FadeSquare.GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,FadeSquare.GetComponent<SpriteRenderer>().color.a+fadeOutTime);
			}
			else {
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}

		if (lastDayUIDelay != 0.0 && Time.time - lastDayUIDelay >= 8 && UI_storyL.GetComponent<UnityEngine.UI.Text>().color.a < 1) {
			if (UI_storyL.GetComponent<UnityEngine.UI.Text>().color.a < 1)
				UI_storyL.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,UI_storyL.GetComponent<UnityEngine.UI.Text>().color.a+0.05f);

			if (UI_storyR.GetComponent<UnityEngine.UI.Text>().color.a < 1)
				UI_storyR.GetComponent<UnityEngine.UI.Text>().color = new Color(1f,1f,1f,UI_storyR.GetComponent<UnityEngine.UI.Text>().color.a+0.05f);
					
		}

		// UI Prompts
		string promptText = "";
		if (gameState == "title" && zMove == 0) {
			promptText = "( ENTER ) to START";
		}
		else if (gameState == "intro") {
			// built-in to intro text
		}
		else if (gameState == "outro" && lastDayUIDelay != 0.0 && Time.time - lastDayUIDelay >= 8) {
			promptText = "( ESCAPE ) TO QUIT\n\r( ENTER ) TO RESTART";	
		}
		else if (gameState == "active" && !closeToGlass) {
			if (textState > -1) {

				if (templateComplete && currentSent >= todaysTemplate.Count) 
					promptText = "( ENTER ) TO DEPART";	
				else {
					if (needsEntry && textState == 1)
						promptText = "( ENTER ) TO CONFIRM\n\r( SPACE ) TO STARE";
					else if (needsEntry)
						promptText = "( ENTER ) TO WRITE\n\r( SPACE ) TO STARE";
					else
						promptText = "( ENTER ) TO PROCEED";
				}
			}

			else if (textState == -1) {
				promptText = "( ENTER ) TO BEGIN";	
			}
		}
		else if (gameState == "inter" && zMove != 4) {
			promptText = " ( ENTER ) FOR NEXT DAY";
		}
		if (!confirmExit)
			UI_prompts.GetComponent<UnityEngine.UI.Text>().text = promptText;




		// Check for time-based triggers

		// word additions
		if (Time.time - lastTickCheck >= 1 && gameState == "active") { // so we're not looping through the list array every frame, just every second
			for (int i=0; i<allDays[(currentDay-1)].dayPhrases.Count; i++) {
				if (dayStart != 0.0 && allDays[(currentDay-1)].dayPhrases[i].triggerType == "time" && float.Parse(allDays[(currentDay-1)].dayPhrases[i].triggerVal) <= (Time.time - dayStart)) {
					allDays[(currentDay-1)].dayPhrasesActive.Add(allDays[(currentDay-1)].dayPhrases[i]);
					allDays[(currentDay-1)].dayPhrases.RemoveAt(i);
					i--;
					UpdateStrings();
					if (!templateComplete)
						transform.GetComponent<audioManager>().playChime(); // sound chime for new word
				}
				else if (stareTime != 0.0 && allDays[(currentDay-1)].dayPhrases[i].triggerType == "close" && float.Parse(allDays[(currentDay-1)].dayPhrases[i].triggerVal) <= (Time.time - stareTime)) {
					allDays[(currentDay-1)].dayPhrasesActive.Add(allDays[(currentDay-1)].dayPhrases[i]);
					allDays[(currentDay-1)].dayPhrases.RemoveAt(i);
					i--;
					UpdateStrings();
					if (!templateComplete)
						transform.GetComponent<audioManager>().playChime(); // sound chime for new word
				}
			}

			// imageEffects triggers
			if (dayStart != 0 && Time.time - dayStart >= 8 && !fxState[0]) { // fish (extra time accounting for fade-in)
				transform.GetComponent<imageEffects>().FishEyeOn(true);
				fxState[0] = true;
			}

			if (dayStart != 0 && Time.time - dayStart >= 28 && !fxState[1]) { // chrome
				transform.GetComponent<imageEffects>().ChromAbOn(true);
				fxState[1] = true;
			}
			if (dayStart !=0 && Time.time - dayStart >= 18 && !fxState[2]) { // glass
				transform.GetComponent<imageEffects>().glassColorOn(true);
				fxState[2] = true;
			}

			lastTickCheck = Time.time;
		}
		
		// Keystrokes

		if (Input.GetKeyUp("escape")) {
    		if (!confirmExit) {
 				lastPrompt = UI_prompts.GetComponent<UnityEngine.UI.Text>().text;
 				UI_prompts.GetComponent<UnityEngine.UI.Text>().text = "<color=#FF0000>( ENTER ) TO EXIT</color>\r\n( ESC ) TO RESUME";
 				confirmExit = true;
    		}
    		else if (confirmExit) {
	    		UI_prompts.GetComponent<UnityEngine.UI.Text>().text = lastPrompt;
    			confirmExit = false;
    		}
		}	

		if (Input.GetKeyUp("return")) {
			//print("gameState: "+gameState+" textState: "+textState + " needsEntry: "+needsEntry+" closeToGlass:"+closeToGlass+" templateComplete: "+templateComplete);
			if (confirmExit)
				Application.Quit();
			if (gameState == "active") {
				if (textState == -1 && !closeToGlass) { // start the day
		    		transform.GetComponent<MouseParallax>().mouseParallaxControl = true;
			    	textState = 0;
		  			UI_main.SetActive(true);
		  			UI_words.SetActive(true);
			    	UpdateStrings();
			    	if (dayStart == 0)
			    		dayStart = Time.time;
	    		}
				else if (textState == 0 && !templateComplete && needsEntry && !closeToGlass) {
					// entering typing mode
					textState = 1;
					todaysTemplate[blankMarkers[currentIndex][0]][blankMarkers[currentIndex][1]] = "";
					UpdateStrings();
				}
				else if (textState == 0 && !needsEntry && !closeToGlass && currentSent < todaysTemplate.Count) { // advance to next sentence
		    		completedText.Add(CleanString(currentTemplate));
		    		currentSent++;
		    		UpdateStrings();
	    		}
				else if (textState != 1 && !closeToGlass && templateComplete && currentSent >= todaysTemplate.Count) { 
					UI_main.SetActive(false);
					UI_main.SetActive(false);
					if (!needsEntry) {
						completedText.Add(CleanString(currentTemplate));
						UpdateStrings();
					}

		    		if (currentDay < maxDays) { // exiting day -> interstitial
			    		transform.GetComponent<audioManager>().stopTankBubbles();
	    				transform.GetComponent<audioManager>().stopTankAmbient();
	    				transform.GetComponent<audioManager>().playWalking();
			    		zMove = 3;
			    		holdTime = Time.time;
			    		currentDay = currentDay + 1;
			    		gameState = "endDay";
			    		transform.GetComponent<MouseParallax>().mouseParallaxControl = false;

		    		}
		    		else if (currentDay == maxDays) { // end of game
		    			transform.GetComponent<MouseParallax>().mouseParallaxControl = false;
		    			transform.GetComponent<audioManager>().stopTankAmbient();
		    			zMove = 5;
		    			gameState = "outro";
		    			completedText.Add(outroText);
		    		}

		    		completedText.Add("\n");
	    			if (currentDay == 6)
	    				dayBreak = completedText.Count;
		    	}
			}
	    	else if (gameState == "intro") { 
	    		transform.GetComponent<audioManager>().playTankBubbles();
	    		transform.GetComponent<audioManager>().playTankAmbient();
		    	gameState = "startDay";
		    	UI_intro.SetActive(false);
	    	}

    		else if (gameState == "inter" && canLeaveInter) {
	    		zMove = 4; 
	    		holdTime = Time.time;
	    		transform.GetComponent<audioManager>().stopCig();
				transform.GetComponent<audioManager>().stopSmokeAmbient();
				canLeaveInter = false;
    		}
		    else if (gameState == "title") {
		    	UI_title.SetActive(false);
	    		zMove = 6; 
	    	}
	    	else if (gameState == "outro" && lastDayUIDelay != 0.0 && Time.time - lastDayUIDelay >= 8) { // restart
				transform.GetComponent<audioManager>().stopTankBubbles();
				UI_prompts.SetActive(false);
				UI_storyL.SetActive(false);
				UI_storyR.SetActive(false);
	    		zMove = 7;
	    	}



		} 
		if (Input.GetKey("space")) {
			if (gameState == "active" && (textState == 0 || textState == 1)) {
				if (zMove == 0 && !closeToGlass ) {
		    		SendAction("stepFoward");
		    		zMove = 1;
		    		closeToGlass = true;
		    		UI_main.SetActive(false);
		    		UI_words.SetActive(false);
		    		stareTime = Time.time;
		    		transform.GetComponent<audioManager>().closeToTank();
		    	}
				if (zMove == 2 && closeToGlass) {
		    		SendAction("stepBack");
		    		zMove = -1;    		
		    		transform.GetComponent<audioManager>().awayFromTank();
		    		stareTime = 0.0f;
				}
			}

		}
		
		if (releaseTyping) {
			textState = 0;
			releaseTyping = false;
		}
	}
}
