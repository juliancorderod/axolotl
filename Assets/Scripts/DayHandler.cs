using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayHandler : MonoBehaviour {

	public class SceneObj {
		public GameObject oRef;
		public Vector3 oPos;
		public string oID;
		public float oXMin;
		public float oXMax;
		public bool oAtGlass;
		
		public SceneObj(GameObject goref, string id, float xmin, float xmax, Vector3 pos, bool ag) { // constructor
			oID = id;
			oXMin = xmin;
			oXMax = xmax;
			oPos = pos;
			oRef = goref;
			oAtGlass = ag;
		}
	}

	public int dayNumber; // set manually in Unity
	public GameObject sceneObject_1;
	public float sceneObjMin_1 = -1f;
	public float sceneObjMax_1 = 1f;
	public bool atGlass_1;

	public GameObject sceneObject_2;
	public float sceneObjMin_2 = -1f;
	public float sceneObjMax_2 = 1f;
	public bool atGlass_2;

	public GameObject sceneObject_3;
	public float sceneObjMin_3 = -1f;
	public float sceneObjMax_3 = 1f;
	public bool atGlass_3;

	public GameObject sceneObject_4;
	public float sceneObjMin_4 = -1f;
	public float sceneObjMax_4 = 1f;
	public bool atGlass_4;

	public GameObject sceneObject_5;
	public float sceneObjMin_5 = -1f;
	public float sceneObjMax_5 = 1f;
	public bool atGlass_5;

	bool lookingAtGlass = false;
	List<SceneObj> sceneObjs;

	// Called when the player steps forward
	public void ActionStepForward() {
		lookingAtGlass = true;
	}

	// Called when player steps back
	public void ActionStepBack() {
		lookingAtGlass = false;
	}

	public void SendPhraseTrigger(string triggerID) {
		GameObject.Find("/AxMain").GetComponent<AxMain3D>().TriggerScenePhrase(triggerID);
	}

	// Use this for initialization
	void Start () {
		
		sceneObjs = new List<SceneObj>();

		if (sceneObject_1 != null)
			sceneObjs.Add(new SceneObj(sceneObject_1, "1", sceneObjMin_1, sceneObjMax_1, sceneObject_1.transform.position, atGlass_1));
		if (sceneObject_2 != null)
			sceneObjs.Add(new SceneObj(sceneObject_2, "2", sceneObjMin_2, sceneObjMax_2, sceneObject_2.transform.position, atGlass_2));
		if (sceneObject_3 != null)
			sceneObjs.Add(new SceneObj(sceneObject_3, "3", sceneObjMin_3, sceneObjMax_3, sceneObject_3.transform.position, atGlass_3));
		if (sceneObject_4 != null)
			sceneObjs.Add(new SceneObj(sceneObject_4, "4", sceneObjMin_4, sceneObjMax_4, sceneObject_4.transform.position, atGlass_4));
		if (sceneObject_5 != null)
			sceneObjs.Add(new SceneObj(sceneObject_5, "5", sceneObjMin_5, sceneObjMax_5, sceneObject_5.transform.position, atGlass_5));
	}
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<sceneObjs.Count; i++) {	
			//if (Mathf.Abs(sceneObjs[i].oRef.transform.position.x-sceneObjs[i].oPos.x) >= sceneObjs[i].oXThresh && sceneObjs[i].oAtGlass == lookingAtGlass)
			float paralaxPosition = Mathf.Clamp (Input.mousePosition.x, 0, Screen.width) / Screen.width * 2f - 1f;

			if (paralaxPosition <= sceneObjs [i].oXMax &&
				paralaxPosition >= sceneObjs [i].oXMin &&
				sceneObjs[i].oAtGlass == lookingAtGlass) {

				SendPhraseTrigger (sceneObjs [i].oID);
			}
		}
		
	}
}
