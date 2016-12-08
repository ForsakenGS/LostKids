using UnityEngine;
using System.Collections;

public class KappaProjectile : MonoBehaviour {

    public ParticleSystem destroyParticles;

    private AudioSource destroySound;
    void OnEnable()
    {
        
    }

    void Destroy()
    {
        
        Instantiate(destroyParticles,transform.position,Quaternion.identity);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
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
