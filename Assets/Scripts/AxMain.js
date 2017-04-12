#pragma strict

public var FrontBox:GameObject;
public var BackBox:GameObject;
public var HumanBox:GameObject;
public var AxBox:GameObject;
public var MoveRange = 0.5;

function Start () {

}

function Update () {
	if (Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width) { // only allow mouse movements that are within game window

		// Move front box left and right at same speed of mouse x axis (higher divisor is, the slower it moves)
		if(Input.GetAxis("Mouse X") != 0) {
			FrontBox.transform.localPosition.x += Input.GetAxis("Mouse X")/200;	
			AxBox.transform.localPosition.x += Input.GetAxis("Mouse X")/300;	
			BackBox.transform.localPosition.x += Input.GetAxis("Mouse X")/400;	
			HumanBox.transform.localPosition.x -= Input.GetAxis("Mouse X")/100;	

		}
	
	}
	
 	// Don't let it get outside of our desired boundaries
 	if (FrontBox.transform.localPosition.x < -MoveRange) {
 		FrontBox.transform.localPosition.x = -MoveRange;
 	}
 	if (FrontBox.transform.localPosition.x > MoveRange) {
 		FrontBox.transform.localPosition.x = MoveRange;
 	}
 	

 	// Keystrokes
 	if (Input.GetKey("escape"))
    	Application.Quit();
}
