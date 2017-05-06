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
		//Takes the mouse position relative to the width of the screen and maps it to a range from -1 to 1
		//Also the mouse.x is clamped to be between 0 and game screen width. So now the paralax we see
		//in the unity window is representative of the final game.
		float paralaxPosition = Mathf.Clamp (Input.mousePosition.x, 0, Screen.width) / Screen.width * 2f - 1f;
		Debug.Log (paralaxPosition);
		//Debug.Log ("X: " + Mathf.Clamp (Input.mousePosition.x, 0, Screen.width) + ", VP: " + (Mathf.Clamp (Input.mousePosition.x, 0, Screen.width)/Screen.width) + ", pP: " + paralaxPosition);

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
}
