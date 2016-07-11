using UnityEngine;
using System.Collections;

public class Dart : MonoBehaviour {

    public float lifeSpan = 3;
    private bool disabled = false;

    public ParticleSystem hitParticles;
    private Rigidbody rigidBody;
    private TrailRenderer trail;

    private AudioSource hitSound;
    void OnEnable()
    {
        Invoke("Disable", lifeSpan);
        disabled = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
        trail.enabled = true;
    }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        if(hitParticles == null)
        {
            hitParticles.GetComponentInChildren<ParticleSystem>();
        }
        hitSound = GetComponent<AudioSource>();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if (!disabled)
        {
            trail.enabled = false;
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<CharacterStatus>().Kill();
                Disable();
            }
            else
            {
                AudioManager.Play(hitSound, false, 1, 0.8f, 1.2f);
                hitSound.Play();
                hitParticles.Play();
                disabled = true;
                rigidBody.constraints = RigidbodyConstraints.None;
                rigidBody.AddTorque(new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0), ForceMode.Impulse);
                rigidBody.AddForce(-transform.forward, ForceMode.Impulse);
                Invoke("Disable", 0.5f);
            }
        }
    }

    void Disable()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }


}
