using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayHandler : MonoBehaviour {

	public class SceneObj {
		public GameObject oRef;
		public Vector3 oPos;
		public string oID;
		public float oXThresh;
		
		public SceneObj(GameObject goref, string id, float xthresh, Vector3 pos) { // constructor
			oID = id;
			oXThresh = xthresh;
			oPos = pos;
			oRef = goref;
		}
	}

	public int dayNumber; // set manually in Unity
	public GameObject sceneObject_1;
	public float sceneObjThresh_1;

	public GameObject sceneObject_2;
	public float sceneObjThresh_2;

	public GameObject sceneObject_3;
	public float sceneObjThresh_3;

	public GameObject sceneObject_4;
	public float sceneObjThresh_4;

	public GameObject sceneObject_5;
	public float sceneObjThresh_5;

	List<SceneObj> sceneObjs;

	// Called when the player steps forward
	public void ActionStepForward() {

	}

	// Called when player steps back
	public void ActionStepBack() {

	}

	public void SendPhraseTrigger(string triggerID) {
		GameObject.Find("/AxMain").GetComponent<AxMain3D>().TriggerScenePhrase(triggerID);
	}

	// Use this for initialization
	void Start () {
		
		sceneObjs = new List<SceneObj>();

		if (sceneObject_1 != null)
			sceneObjs.Add(new SceneObj(sceneObject_1, "1", sceneObjThresh_1, sceneObject_1.transform.position));
		if (sceneObject_2 != null)
			sceneObjs.Add(new SceneObj(sceneObject_2, "2", sceneObjThresh_2, sceneObject_2.transform.position));
		if (sceneObject_3 != null)
			sceneObjs.Add(new SceneObj(sceneObject_3, "3", sceneObjThresh_3, sceneObject_3.transform.position));
		if (sceneObject_4 != null)
			sceneObjs.Add(new SceneObj(sceneObject_4, "4", sceneObjThresh_4, sceneObject_4.transform.position));
		if (sceneObject_5 != null)
			sceneObjs.Add(new SceneObj(sceneObject_5, "5", sceneObjThresh_5, sceneObject_5.transform.position));
	}
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<sceneObjs.Count; i++) {	
			if (Mathf.Abs(sceneObjs[i].oRef.transform.position.x-sceneObjs[i].oPos.x) >= sceneObjs[i].oXThresh)
				SendPhraseTrigger(sceneObjs[i].oID);
		}
		
	}
}
