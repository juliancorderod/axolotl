using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class imageEffects : MonoBehaviour {

	public GameObject tankCam;
	public GameObject mainCam;
	public GameObject glass;

	bool turnFishEyeOn = false;
	float fishEyeCounter;

	bool turnChromAbOn = false;

	bool changeGlassColor = false;
	float rVal, gVal, bVal;

	bool setRandomValues = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//debug triggers
		if(Input.GetKey(KeyCode.Alpha1)){
			FishEyeOn(true);
		}
		if(Input.GetKey(KeyCode.Alpha2)){
			ChromAbOn(true);
		}

		if(Input.GetKey(KeyCode.Alpha3)){
			glassColorOn (true);
		}

		if(Input.GetKey(KeyCode.Alpha0)){
			FishEyeOn(false);
			ChromAbOn(false);
			glassColorOn (false);
		}



		if(turnFishEyeOn){

			fishEyeCounter+= Time.deltaTime;
			tankCam.GetComponent<Fisheye>().strengthY = (Mathf.Sin((fishEyeCounter-15f) * 0.1f)/8f) + 0.13f;
			mainCam.GetComponent<Fisheye>().strengthY += 0.003f * Time.deltaTime;
			mainCam.GetComponent<Fisheye>().strengthX += 0.001f * Time.deltaTime;

		}else{
			fishEyeCounter = 0f;
			mainCam.GetComponent<Fisheye>().strengthY = 0f;
			mainCam.GetComponent<Fisheye>().strengthX = 0f;
		}



		if(turnChromAbOn){

			tankCam.GetComponent<VignetteAndChromaticAberration>().chromaticAberration += 1f * Time.deltaTime;

		}else{
			tankCam.GetComponent<VignetteAndChromaticAberration>().chromaticAberration = 0f;
		}



		if(changeGlassColor){
			
			glass.GetComponent<SpriteRenderer>().color += new Color(rVal * Time.deltaTime * 0.1f,
				gVal * Time.deltaTime * 0.1f, bVal * Time.deltaTime * 0.1f, 0f);

		}else{
			
			glass.GetComponent<SpriteRenderer>().color = new Color(48f/255f, 79f/255f, 139f/255f, 127f/255f);
			rVal = 0f;
			gVal = 0f;
			bVal = 0f;


		}

		if(setRandomValues){
			rVal = Random.Range(-0.1f, 0.1f);
			gVal = Random.Range(-0.1f, 0.1f);
			bVal = Random.Range(-0.1f, 0.1f);

			//Debug.Log(rVal);
			setRandomValues = false;
		}
		
	}

	public void FishEyeOn(bool trueOrFalse){//this should be triggered semi-early every day (maybe 5-10 seconds in or something like that) 
		//true for on, false for off (should be turned off at the end of everyday-during smoke scene)

		if (trueOrFalse){
			turnFishEyeOn = true;
		}else {
			turnFishEyeOn = false;
		}
	}

	public void ChromAbOn(bool trueOrFalse){//this should be triggered later in the day (maybe 30-45 sec in)
		//true for on, false for off (should be turned off at the end of everyday-during smoke scene)

		if (trueOrFalse){
			turnChromAbOn = true;
		}else {
			turnChromAbOn = false;
		}
	}

	public void glassColorOn(bool trueOrFalse){//this should be triggered later in the day (maybe 30-45 sec in)
		//true for on, false for off (should be turned off at the end of everyday-during smoke scene)

		if (trueOrFalse){
			changeGlassColor = true;
			setRandomValues = true;
		}else {
			changeGlassColor = false;
		}
		//arreglar estooooooo

	}
}
