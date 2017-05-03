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
		float paralaxPosition = RangeReMap (Input.mousePosition.x, 0f, Screen.width, -1, 1);

		this.transform.position = new Vector3 (
			startPos + (paralaxPosition * mp.axolotlModifier),
			transform.position.y,
			transform.position.z);
	}

	float RangeReMap (float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}
