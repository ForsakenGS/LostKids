using UnityEngine;
using System.Collections;

public class KappaProjectile : MonoBehaviour {

    public ParticleSystem destroyParticles;

    private AudioSource destroySound;


    private bool isActivated = false;

    void OnEnable()
    {
        Instantiate(destroyParticles, transform.position, Quaternion.identity);
    }

    void OnDisable()
    {
        Instantiate(destroyParticles, transform.position, Quaternion.identity);
    }

    void Destroy()
    {
        isActivated = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        destroySound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (isActivated)
        {
            if (!col.isTrigger)
            {
                if (col.gameObject.CompareTag("Player"))
                {
                    col.gameObject.GetComponent<CharacterStatus>().Kill();
                }
                if (!destroySound.isPlaying)
                {
                    AudioManager.Play(destroySound, false, 0.5f);
                }
                Invoke("Destroy", 0.1f);
            }
        }
    }


    public void Activate()
    {
        isActivated = true;
        transform.parent = null;
    }
}
