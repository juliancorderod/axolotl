using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayHandler : MonoBehaviour {

	public class SceneObj {
		public GameObject oRef;
		public SpriteRenderer oGlow;
		public Vector3 oPos;
		public string oID;
		public float oXMin;
		public float oXMax;
		public float oHoverTime;
		public bool oAtGlass;
		
		public SceneObj(GameObject goref, SpriteRenderer glow, string id, float xmin, float xmax, float hovertime, Vector3 pos, bool ag) { // constructor
			oID = id;
			oXMin = xmin;
			oXMax = xmax;
			oHoverTime = hovertime;
			oPos = pos;
			oRef = goref;
			oGlow = glow;
			oAtGlass = ag;
		}
	}

	public int dayNumber; // set manually in Unity
	public GameObject sceneObject_1;
	public SpriteRenderer glow_1;
	public float sceneObjMin_1 = -1f;
	public float sceneObjMax_1 = 1f;
	public bool atGlass_1;

	public GameObject sceneObject_2;
	public SpriteRenderer glow_2;
	public float sceneObjMin_2 = -1f;
	public float sceneObjMax_2 = 1f;
	public bool atGlass_2;

	public GameObject sceneObject_3;
	public SpriteRenderer glow_3;
	public float sceneObjMin_3 = -1f;
	public float sceneObjMax_3 = 1f;
	public bool atGlass_3;

	public GameObject sceneObject_4;
	public SpriteRenderer glow_4;
	public float sceneObjMin_4 = -1f;
	public float sceneObjMax_4 = 1f;
	public bool atGlass_4;

	public GameObject sceneObject_5;
	public SpriteRenderer glow_5;
	public float sceneObjMin_5 = -1f;
	public float sceneObjMax_5 = 1f;
	public bool atGlass_5;

	public bool lookingAtGlass = false;
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
			sceneObjs.Add (new SceneObj (sceneObject_1, glow_1, "1", sceneObjMin_1, sceneObjMax_1, 0f, sceneObject_1.transform.position, atGlass_1));
		if (sceneObject_2 != null)
			sceneObjs.Add (new SceneObj (sceneObject_2, glow_2, "2", sceneObjMin_2, sceneObjMax_2, 0f, sceneObject_2.transform.position, atGlass_2));
		if (sceneObject_3 != null)
			sceneObjs.Add (new SceneObj (sceneObject_3, glow_3, "3", sceneObjMin_3, sceneObjMax_3, 0f, sceneObject_3.transform.position, atGlass_3));
		if (sceneObject_4 != null)
			sceneObjs.Add (new SceneObj (sceneObject_4, glow_4, "4", sceneObjMin_4, sceneObjMax_4, 0f, sceneObject_4.transform.position, atGlass_4));
		if (sceneObject_5 != null)
			sceneObjs.Add (new SceneObj (sceneObject_5, glow_5, "5", sceneObjMin_5, sceneObjMax_5, 0f, sceneObject_5.transform.position, atGlass_5));
	}
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<sceneObjs.Count; i++) {	
			//if (Mathf.Abs(sceneObjs[i].oRef.transform.position.x-sceneObjs[i].oPos.x) >= sceneObjs[i].oXThresh && sceneObjs[i].oAtGlass == lookingAtGlass)
			if (sceneObjs [i].oHoverTime != -5f) {
				float paralaxPosition = Mathf.Clamp (Input.mousePosition.x, 0, Screen.width) / Screen.width * 2f - 1f;

				if (paralaxPosition <= sceneObjs [i].oXMax && paralaxPosition >= sceneObjs [i].oXMin && (sceneObjs[i].oAtGlass && sceneObjs[i].oAtGlass == lookingAtGlass || !sceneObjs[i].oAtGlass)) {

					//Debug.Log (sceneObjs [i].oHoverTime);
					sceneObjs [i].oHoverTime += Time.deltaTime / 5f;

					if (sceneObjs [i].oHoverTime >= 1f) {
						//Ensures the sendPhaseTrigger is called only once.
						SendPhraseTrigger (sceneObjs [i].oID);
						sceneObjs [i].oHoverTime = -5f;
					}
				} else {
					sceneObjs [i].oHoverTime = 0;
				}

				sceneObjs [i].oGlow.color = Color.Lerp (new Color (1f, 1f, 1f, 0f), new Color (1f, 1f, 1f, 0.9f), sceneObjs [i].oHoverTime);
			} else {
				sceneObjs [i].oGlow.color = new Color (1f, 1f, 1f, 0f);
			}
		}
		
	}
}
