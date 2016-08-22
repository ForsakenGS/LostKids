using UnityEngine;
using System.Collections;

public class LightToExit : MonoBehaviour {

    public Transform exit;

    public float time = 4.0f;

    private bool done = false;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col) {
        if(exit != null && !done && CharacterManager.IsActiveCharacter(col.gameObject)) {
            done = true;
            iTween.MoveTo(gameObject, iTween.Hash("position", exit.position, "time", time, "delay", 0.2f, "easeType", iTween.EaseType.easeInOutSine));
        }
        
    }
}
