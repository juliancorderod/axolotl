using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFadeIn : MonoBehaviour {

	AxMain3D am;
	MouseParallax mp;
	[SerializeField] AxolotlParalax ap;

	bool previousCloseToGlass;
	float lerpTime = 0;

	SpriteRenderer[] backgroundSpriteRenderers;
	List<float> alphas = new List<float>();

	[SerializeField] SpriteRenderer room;
	[SerializeField] SpriteRenderer[] dayTenObjects;

	IEnumerator FadeBois () {
		for (float f = 0f; f <= 2f; f += 0.1f) {
			for (int i = 0; i < backgroundSpriteRenderers.Length; i++) {
				backgroundSpriteRenderers [i].color = Color.Lerp (
					new Color (1f, 1f, 1f, alphas [i] * 0.1f),
					new Color (1f, 1f, 1f, 1f / 255f),
					f / 2);
			}
			for (int i = 0; i < dayTenObjects.Length; i++) {
				dayTenObjects [i].color = Color.Lerp (
					dayTenObjects [i].color,
					new Color (1f, 1f, 1f, 10f / 255f),
					f / 2);
			}

			yield return new WaitForSecondsRealtime (0.1f);
		}
	}

	IEnumerator ShowRoom () {
		for (float f = 0f; f <= 5f; f += 0.1f) {
			for (int i = 0; i < backgroundSpriteRenderers.Length; i++) {
				room.color = Color.Lerp (
					new Color (1f, 1f, 1f, 0f),
					new Color (1f, 1f, 1f, 61f / 255f),
					f / 5f);
			}
			yield return new WaitForSecondsRealtime (0.1f);
		}
	}

	IEnumerator MoveRight () {
		yield return new WaitForSecondsRealtime (2f);

		for (float f = 0f; f <= 8f; f += 0.1f) {
			float paralaxPosition = (f / 8f) * 0.7f;

			mp.MainCamera.transform.localPosition = new Vector3 (
				mp.mainCamStartPos + (paralaxPosition * mp.mainCamModifier),
				mp.MainCamera.transform.localPosition.y,
				mp.MainCamera.transform.localPosition.z);

			mp.TankCamera.transform.localPosition = new Vector3 (
				mp.tankCamStartPos + (paralaxPosition * mp.tankCamModifier),
				mp.TankCamera.transform.localPosition.y,
				mp.TankCamera.transform.localPosition.z);				

			mp.FaceGroup.transform.localPosition = new Vector3 (
				mp.faceGroupStartPos + (paralaxPosition * mp.faceGroupModifier),
				mp.FaceGroup.transform.localPosition.y,
				mp.FaceGroup.transform.localPosition.z);	

			mp.AllDays.transform.localPosition = new Vector3 (
				mp.allDaysStartPos + (paralaxPosition * mp.allDaysModifier),
				mp.AllDays.transform.localPosition.y,
				mp.AllDays.transform.localPosition.z);

			ap.gameObject.transform.position = new Vector3 (
				ap.axStartPos + (paralaxPosition * 4f),
				ap.gameObject.transform.position.y,
				ap.gameObject.transform.position.z);


			yield return new WaitForSecondsRealtime (0.1f);
		}
	}

	public void GameEndScreenSetUp() {
		room.gameObject.SetActive (true);
		//Make all the Aquarium Backgrounds Fade out. We know at this point they'll be zooemed out.
		StartCoroutine ("FadeBois");
		//Show that room
		StartCoroutine ("ShowRoom");
		//Move right
		StartCoroutine ("MoveRight");
	}


	// Use this for initialization
	void Start () {
		am = GameObject.Find ("AxMain").GetComponent<AxMain3D> ();
		mp = GameObject.Find ("AxMain").GetComponent<MouseParallax> ();

		previousCloseToGlass = am.closeToGlass;

		backgroundSpriteRenderers = GetComponentsInChildren<SpriteRenderer> ();

		for (int i = 0; i < backgroundSpriteRenderers.Length; i++) {
			alphas.Add (backgroundSpriteRenderers [i].color.a);
			backgroundSpriteRenderers [i].color = new Color (1f, 1f, 1f, 0.1f);
		}
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyUp(KeyCode.T)) {
//			mp.mouseParallaxControl = false;
//			GameEndScreenSetUp ();
//		}

		if (previousCloseToGlass != am.closeToGlass) {
			if (am.closeToGlass == true) {
				lerpTime += Time.deltaTime / 5f;
				for (int i = 0; i < backgroundSpriteRenderers.Length; i++) {
					backgroundSpriteRenderers [i].color =
						Color.Lerp (
							new Color (1f, 1f, 1f, alphas [i] * 0.1f),
							new Color (1f, 1f, 1f, alphas [i]),
							lerpTime);
				}
			} else if (am.closeToGlass == false) {
				lerpTime += Time.deltaTime / 1f;
				for (int i = 0; i < backgroundSpriteRenderers.Length; i++) {
					backgroundSpriteRenderers [i].color =
						Color.Lerp (
							new Color (1f, 1f, 1f, alphas [i]),
							new Color (1f, 1f, 1f, alphas [i] * 0.1f),
							lerpTime);
				}
			}
		} else {
			lerpTime = 0f;
		}

		if (lerpTime >= 1f) {
			previousCloseToGlass = am.closeToGlass;
		}
	}
}
