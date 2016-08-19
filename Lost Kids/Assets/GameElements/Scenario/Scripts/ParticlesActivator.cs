using UnityEngine;
using System.Collections;

public class ParticlesActivator : MonoBehaviour {

    private ParticleSystem particles;
    private float emissionRate;

    private Color color;
    private float alphaBase;

	// Use this for initialization
	void Awake () {
        particles = GetComponent<ParticleSystem>();

        var em = particles.emission;
        emissionRate = em.rate.constantMax;

        color = particles.startColor;
        alphaBase = color.a;

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

    public void IncreaseEmission(float em) {
        var emis = particles.emission;
        var rate = emis.rate;
        rate.constantMax = em;

        emis.rate = rate;
    }

    public void DecreaseEmission() {
        var emis = particles.emission;
        var rate = emis.rate;
        rate.constantMax = emissionRate;

        emis.rate = rate;
    }

    public void IntensifyColor() {
        particles.startColor = new Color(color.r, color.g, color.b, 1.0f);

    }

    public void AttenuateColor() {
        particles.startColor = new Color(color.r, color.g, color.b, alphaBase);

    }
}
