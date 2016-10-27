using UnityEngine;
using System.Collections;

public class AstralDoorActivator : MonoBehaviour {

    public ParticleSystem astralDoor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col) {

        if(col.gameObject.CompareTag("Player") && !astralDoor.isPlaying) {
            astralDoor.Play();
            Destroy(this.gameObject);
        }
    }

}
