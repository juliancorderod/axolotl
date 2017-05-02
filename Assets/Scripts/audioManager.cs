using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour {

	//tankscene
	public AudioSource tankBubbles;//must fade in when fading into tankScene, fadeout when fading out of tankScene, must Loop, max volume = 1
	public AudioSource tankAmbient;//must fade in when fading into tankScene, fadeout when fading out of tankScene, must loop, max volume = 0.2
	public AudioSource standUpCloseDoor;//must play once when fading out of tankScene , max volume = 1;

	//smokescene
	public AudioSource lightCig;//must play once before fading into smokescene, max volume = 1
	public AudioSource cigBurn;//must loop after lightCig plays, max volume = 1
	public AudioSource smokeRoomAmbient;//must fadein when fading into smokescene, fadeout when fading out of smokescene, must loop, max volume 0.5f

	List<AxSound> toFadeIn;
	List<AxSound> toFadeOut;
	

	public class AxSound {
		public AudioSource aSource;
		public float fadeVal;
		public float fadeTarget;

		public AxSound(AudioSource aus, float fv, float ft) { // constructor
			aSource = aus;
			fadeVal = fv;
			fadeTarget = ft;
		}
	}

	// Use this for initialization
	void Start () {
		toFadeIn = new List<AxSound>();
		toFadeOut = new List<AxSound>();
	}
	
	void FadeIn() {
		for (int i=0;i<toFadeIn.Count;i++) {
			toFadeIn[i].aSource.volume += toFadeIn[i].fadeVal;
			if (toFadeIn[i].aSource.volume >= toFadeIn[i].fadeTarget) {
				toFadeIn.RemoveAt(i);
				i--;
			}
		}
	}

	void FadeOut() {
		for (int i=0;i<toFadeOut.Count;i++) {
			toFadeOut[i].aSource.volume -= toFadeOut[i].fadeVal;
			if (toFadeOut[i].aSource.volume <= toFadeOut[i].fadeTarget) {
				toFadeOut[i].aSource.Stop();
				toFadeOut.RemoveAt(i);
				i--;
			}
		}
	}

	// Update is called once per frame
	void Update () {

	

		FadeIn();
		FadeOut();
		/*

		if(Input.GetKeyDown(KeyCode.Space)){
			testStart = true;
		}
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			testStop = true;
		}

		if(Input.GetKeyDown(KeyCode.B)){
			testStartB = true;
		}
		if(Input.GetKeyDown(KeyCode.N)){
			testStopB = true;
		}
		if(testStart){
		//playTankAmbient();
			playTankBubbles();
			//playTankAmbient();
			testStart = false;
		}
		if(testStop){
			//playTankAmbient();
			stopTankBubbles();
			testStop = false;
		}

		if(testStartB){
		    playTankAmbient();
			testStartB = false;
		}
		if(testStopB){
			stopTankAmbient();
			testStopB = false;
		}
		

		
		if (fade == "tankBubblesIn"){

			if(tankBubbles.volume < 1){
				tankBubbles.volume += fadeVal;
			} else if(tankBubbles.volume >=1){
				fade = "";
			}
		}
		if (fade == "tankBubblesOut"){

			if(tankBubbles.volume > 0){
				tankBubbles.volume -= fadeVal;
			} else if(tankBubbles.volume <=0){
				fade = "";
				tankBubbles.Stop();
			}
		}

		if (fade == "tankAmbientIn"){

			if(tankAmbient.volume < 1){
				tankAmbient.volume += fadeVal;
			} else if(tankAmbient.volume >=1){
				fade = "";
			}
		}
		if (fade == "tankAmbientOut"){

			if(tankAmbient.volume > 0){
				tankAmbient.volume -= fadeVal;
			} else if(tankAmbient.volume <=0){
				fade = "";
				tankAmbient.Stop();
			}
		}
		*/

	}


	public void playTankBubbles(){
		tankBubbles.volume = 0.0f;
		tankBubbles.Play();
		toFadeIn.Add(new AxSound(tankBubbles, 0.01f, 0.4f));
	}
	public void stopTankBubbles(){
		toFadeOut.Add(new AxSound(tankBubbles, 0.05f, 0.0f));
	}

	public void playTankAmbient(){
		tankAmbient.volume = 0.0f;
		tankAmbient.Play();
		toFadeIn.Add(new AxSound(tankAmbient, 0.001f, 0.05f));
	}
	public void stopTankAmbient(){
		toFadeOut.Add(new AxSound(tankAmbient, 0.05f, 0.0f));
	}
	public void playWalking(){
		standUpCloseDoor.Play();
	}
	public void playLightCig(){
		lightCig.Play();
	}
	public void playCig(){
		cigBurn.volume = 0.0f;
		cigBurn.Play();
		toFadeIn.Add(new AxSound(cigBurn, 0.001f, 0.05f));
	}
	public void stopCig() {
		toFadeOut.Add(new AxSound(cigBurn, 0.05f, 0.0f));
	}
	public void playSmokeAmbient(){
		smokeRoomAmbient.volume = 0.0f;
		smokeRoomAmbient.Play();
		toFadeIn.Add(new AxSound(smokeRoomAmbient, 0.001f, 0.05f));
	}
	public void stopSmokeAmbient() {
		toFadeOut.Add(new AxSound(smokeRoomAmbient, 0.05f, 0.0f));
	}





//	public void fade(AudioSource audioSource, float targetVolume, float fadeTime, bool fadeIn){//true for fadein, false for fadeout
//
//		if(fadeIn){
//
//			if(!audioSource.isPlaying){
//				audioSource.Play();
//			}
//
//			float t = 0f;
//			t += Time.deltaTime/fadeTime;
//			audioSource.volume = Mathf.Lerp(0f, targetVolume, t);
//		}
//
//		if(!fadeIn){
//
//			float t = 0f;
//			t += Time.deltaTime/fadeTime;
//			audioSource.volume = Mathf.Lerp(targetVolume, 0f, t);
//
//			if(t >= 1f){
//				audioSource.Stop();
//			}
//		}
//
//	
//
//	}





}
