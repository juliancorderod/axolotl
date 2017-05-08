using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFadeIn : MonoBehaviour {

	AxMain3D am;
	bool previousCloseToGlass;
	float lerpTime = 0;

	SpriteRenderer[] backgroundSpriteRenderers;
	List<float> alphas = new List<float>();


	public void GameEndScreenSetUp() {
		//Make all the Aquarium Background Fade out. 
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
