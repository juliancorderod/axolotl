using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour {

	//tankscene
	public AudioSource tankBubbles;//must fade in when fading into tankScene, fadeout when fading out of tankScene, must Loop, max volume = 1
	public AudioSource tankAmbient;//must fade in when fading into tankScene, fadeout when fading out of tankScene, must loop, max volume = 0.2
	public AudioSource standUpCloseDoor;//must play once when fading out of tankScene , max volume = 1;

	//smokescene
	//public AudioSource lightCig;//must play once before fading into smokescene, max volume = 1
	public AudioSource cigBurn;//must loop after lightCig plays, max volume = 1
	public AudioSource smokeRoomAmbient;//must fadein when fading into smokescene, fadeout when fading out of smokescene, must loop, max volume 0.5f

	List<AxSound> toFadeIn;
	List<AxSound> toFadeOut;
	
	float tmpA = 0.0f;
	float tmpB = 0.0f;

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
	}

	// AxSound syntax = audioSource, fadeAmt, targetVol
	public void playTankBubbles(){
		tankBubbles.volume = 0.0f;
		tankBubbles.Play();
		toFadeIn.Add(new AxSound(tankBubbles, 0.01f, 0.4f));
	}
	public void stopTankBubbles(){
		toFadeOut.Add(new AxSound(tankBubbles, 0.005f, 0.0f));
	}

	public void playTankAmbient(){
		tankAmbient.volume = 0.0f;
		tankAmbient.Play();
		toFadeIn.Add(new AxSound(tankAmbient, 0.001f, 0.4f));
	}
	public void stopTankAmbient(){
		toFadeOut.Add(new AxSound(tankAmbient, 0.005f, 0.0f));
	}
	public void closeToTank() {
		tmpA = tankAmbient.volume;
		tmpB = tankBubbles.volume;
		tankAmbient.volume = 0.025f;
		tankBubbles.volume = 1.0f;
	}
	public void awayFromTank() {
		tankAmbient.volume = tmpA;
		tankBubbles.volume = tmpB;
	}
	public void playWalking(){
		standUpCloseDoor.Play();
	}
	public void playCig(){
		cigBurn.volume = 0.0f;
		cigBurn.Play();
		toFadeIn.Add(new AxSound(cigBurn, 0.004f, 0.5f));
	}
	public void stopCig() {
		toFadeOut.Add(new AxSound(cigBurn, 0.0009f, 0.0f));
	}
	public void playSmokeAmbient(){
		smokeRoomAmbient.volume = 0.0f;
		smokeRoomAmbient.Play();
		toFadeIn.Add(new AxSound(smokeRoomAmbient, 0.004f, 0.4f));
	}
	public void stopSmokeAmbient() {
		toFadeOut.Add(new AxSound(smokeRoomAmbient, 0.0009f, 0.0f));
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
