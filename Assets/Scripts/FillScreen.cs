using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script takes the distance a sprite is from the camera and makes it fill the screen
//regardless of the distance. I am making all my assets in a 1980x1080 illustrator file. I export all my
//assets out as a 1980x1080 sprite to preserve all the proportions and composotion I set up in the
//illustrator file. This script will keep all those assets in proportion regardless of their distance from
//the camera. Make sure the x and y values of all your sprites are the same.

public class FillScreen : MonoBehaviour {

	//The default values are for a 1980x1080 sprite. If your sprite values are different, you can
	//override these values in the inspector.
	[SerializeField] float spriteX = 1980f;
	[SerializeField] float spriteY = 1080f;

	void Start () {
		Camera myCamera = Camera.main;
		float zDiff = Vector3.Distance (myCamera.transform.position, this.transform.position);
		Sprite mySprite = this.GetComponent<SpriteRenderer> ().sprite;

		float height = 2* zDiff * Mathf.Tan (Mathf.Deg2Rad * myCamera.fieldOfView / 2f);
		Vector2 sizeFactor = new Vector2 (height * myCamera.aspect, height);
		sizeFactor *= mySprite.pixelsPerUnit;

		this.transform.localScale = new Vector3 (sizeFactor.x / spriteX, sizeFactor.y / spriteY, 0f);
	}
}
