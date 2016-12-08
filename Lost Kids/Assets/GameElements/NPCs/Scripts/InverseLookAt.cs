using UnityEngine;
using System.Collections;

public class InverseLookAt : MonoBehaviour {

    Vector3 targetPos;
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {
        if (CharacterManager.GetActiveCharacter() != null)
        { 
        targetPos = CharacterManager.GetActiveCharacter().transform.position;
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
        }
	}
}
