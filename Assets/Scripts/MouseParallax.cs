using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseParallax : MonoBehaviour {

	public GameObject MainCamera;
	public GameObject TankCamera;
	public GameObject FaceGroup;
	public GameObject AllDays;

	[SerializeField] float mainCamModifier = 1f / 200f;
	[SerializeField] float tankCamModifier = 1f / 200f;
	[SerializeField] float faceGroupModifier = 1f / 100f;
	[SerializeField] float allDaysModifier = 1f / 50f;
	public float axolotlModifier = 0f;

	float mainCamStartPos;
	float tankCamStartPos;
	float faceGroupStartPos;
	float allDaysStartPos;

	public bool mouseParallaxControl = true;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Confined;

		mainCamStartPos = MainCamera.transform.position.x;
		tankCamStartPos = TankCamera.transform.position.x;
		faceGroupStartPos = FaceGroup.transform.position.x;
		allDaysStartPos = AllDays.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		float paralaxPosition = RangeReMap (Input.mousePosition.x, 0f, Screen.width, -1, 1);

		if (mouseParallaxControl == true) {
			MainCamera.transform.localPosition = new Vector3 (
				mainCamStartPos + (paralaxPosition * mainCamModifier),
				MainCamera.transform.localPosition.y,
				MainCamera.transform.localPosition.z);

			TankCamera.transform.localPosition = new Vector3 (
				tankCamStartPos + (paralaxPosition * tankCamModifier),
				TankCamera.transform.localPosition.y,
				TankCamera.transform.localPosition.z);				

			FaceGroup.transform.localPosition = new Vector3 (
				faceGroupStartPos + (paralaxPosition * faceGroupModifier),
				FaceGroup.transform.localPosition.y,
				FaceGroup.transform.localPosition.z);	

			AllDays.transform.localPosition = new Vector3 (
				allDaysStartPos + (paralaxPosition * allDaysModifier),
				AllDays.transform.localPosition.y,
				AllDays.transform.localPosition.z);	
		}
	}

	float RangeReMap (float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}
