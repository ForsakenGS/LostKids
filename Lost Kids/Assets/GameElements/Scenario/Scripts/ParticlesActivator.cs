using UnityEngine;
using System.Collections;

public class VortexActivator : MonoBehaviour {

    private ParticleSystem particles;

	// Use this for initialization
	void Awake () {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show() {
        if(!particles.isPlaying) {
            particles.Play();
        }
    }

    public void Hide() {
        if(particles.isPlaying) {
            particles.Stop();
        }
    }
}
