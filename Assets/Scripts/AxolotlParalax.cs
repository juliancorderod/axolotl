using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxolotlParalax : MonoBehaviour {

	MouseParallax mp;
	float startPos;

	// Use this for initialization
	void Start () {
		mp = GameObject.Find ("AxMain").GetComponent<MouseParallax> ();
		startPos = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		float paralaxPosition = Mathf.Clamp (Input.mousePosition.x, 0, Screen.width) / Screen.width * 2f - 1f;

		if (mp.mouseParallaxControl) {
			this.transform.position = new Vector3 (
				startPos + (paralaxPosition * mp.axolotlModifier),
				transform.position.y,
				transform.position.z);
		}
	}
}
