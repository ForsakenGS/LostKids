using UnityEngine;
using System.Collections;

public class DestroyParticles : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, GetComponent<ParticleSystem>().duration);
    }

}
