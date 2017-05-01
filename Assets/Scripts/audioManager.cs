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

	public float fadeVal;

	public bool testStart;
	public bool testStop;

	string fade = "";
	//float t = 0;
	// Use this for initialization
	void Start () {

		//Debug.Log( tankAmbient.volume	);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space)){
			testStart = true;
		}
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			testStop = true;
		}

		if(testStart){
		//playTankAmbient();
			playTankBubbles();
			playTankAmbient();
			testStart = false;
		}
		if(testStop){
			//playTankAmbient();
			stopTankBubbles();
			stopTankAmbient();
			testStop = false;
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

	}


	public void playTankBubbles(){
		tankBubbles.Play();
		fade = "tankBubblesIn";
	}
	public void stopTankBubbles(){
		fade = "tankBubblesOut";
	}


	public void playTankAmbient(){
		tankAmbient.Play();
		fade = "tankAmbientIn";
	}
	public void stopTankAmbient(){
		fade = "tankAmbientOut";
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
