using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFadeIn : MonoBehaviour {

	AxMain3D am;
	bool previousCloseToGlass;
	float lerpTime = 0;

	SpriteRenderer[] backgroundSpriteRenderers;
	List<float> alphas = new List<float>();

	[SerializeField] SpriteRenderer room;
	[SerializeField] SpriteRenderer[] dayTenObjects;

	IEnumerator FadeBois () {
		for (float f = 0f; f <= 2f; f += 0.01f) {
			for (int i = 0; i < backgroundSpriteRenderers.Length; i++) {
				backgroundSpriteRenderers [i].color = Color.Lerp (
					new Color (1f, 1f, 1f, alphas [i] * 0.1f),
					new Color (1f, 1f, 1f, 10f / 255f),
					f / 2);
			}
			for (int i = 0; i < dayTenObjects.Length; i++) {
				dayTenObjects [i].color = Color.Lerp (
					dayTenObjects [i].color,
					new Color (1f, 1f, 1f, 20f / 255f),
					f / 2);
			}

			yield return new WaitForSecondsRealtime (0.01f);
		}
	}

	IEnumerator ShowRoom () {
		for (float f = 0f; f <= 5f; f += 0.01f) {
			for (int i = 0; i < backgroundSpriteRenderers.Length; i++) {
				room.color = Color.Lerp (
					new Color (1f, 1f, 1f, 0f),
					new Color (1f, 1f, 1f, 61f / 255f),
					f / 5f);
			}
			yield return new WaitForSecondsRealtime (0.01f);
		}
	}

	IEnumerator MoveCamLeft () {
		yield return new WaitForSecondsRealtime (7f);

		for (float f = 0f; f <= 7f; f += 0.01f) {
			Camera.main.transform.position = Vector3.Lerp (
				Camera.main.transform.position,
				new Vector3 (-2.57f, Camera.main.transform.position.y, Camera.main.transform.position.z),
				f / 7f);
			
			yield return new WaitForSecondsRealtime (0.01f);
		}
	}

	public void GameEndScreenSetUp() {
		room.gameObject.SetActive (true);
		//Make all the Aquarium Backgrounds Fade out. We know at this point they'll be zooemed out.
		StartCoroutine ("FadeBois");
		//Show that room
		StartCoroutine ("ShowRoom");
		//Move right
		StartCoroutine ("MoveCamLeft");
	}


	// Use this for initialization
	void Start () {
		am = GameObject.Find ("AxMain").GetComponent<AxMain3D> ();

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
//			GameEndScreenSetUp ();
//
//			GameObject.Find ("AxMain").GetComponent<MouseParallax> ().mouseParallaxControl = false;
//		}

		if (previousCloseToGlass != am.closeToGlass) {
			if (am.closeToGlass == true) {
				lerpTime += Time.deltaTime / 3f;
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
