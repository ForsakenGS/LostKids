using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DartLauncherTrap : AbstractTrap {

    public ParticleSystem shootParticles;

    public GameObject dartPrefab;

    public float dartSpeed=10;

    public float shootCooldown = 1;

    public float shooterOffset = 0.5f;

    private List<GameObject> projectilePool;

    private int projectilePoolSize = 10;

    AudioSource shootSound;

    void Awake()
    {
        projectilePool = new List<GameObject>(projectilePoolSize);
        if(shootParticles==null)
        {
            shootParticles = GetComponentInChildren<ParticleSystem>();
        }
        shootSound = GetComponent<AudioSource>();
        
    }
    // Use this for initialization
    void Start () {
        /*
        if(enabled && fireOnEnable)
        {
            Shoot();
        }
        */
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        if(isEnabled && gameObject.activeInHierarchy)
        {
            FireTrap();
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public override void Activate()
    {
        DisableTrap();
    }

    public override void CancelActivation()
    {
        EnableTrap();
    }

    public override void DisableTrap()
    {
        isEnabled = false;
        active = false;
        CancelInvoke("Shoot");

    }

    public override void EnableTrap()
    {
        
        isEnabled = true;
        if(fireOnEnable)
        {
            FireTrap();
        }

    }

    public override void FireTrap()
    {
        if (isEnabled && !active)
        {
            Invoke("Shoot",shootCooldown);
        }

    }

    public override void FireTrapOneShot()
    {
        
    }

    public void Shoot()
    {
        if (isEnabled)
        {
            active = true;
            //Se detecta que no haya nadad justo delante para que no  se instancie el dardo dentro de un objeto
            Ray detectRay = new Ray(transform.position, transform.forward);
#if UNITY_EDITOR
            Debug.DrawRay(detectRay.origin, detectRay.direction, Color.green, shooterOffset + 1);
#endif
            RaycastHit hitInfo;
            if (!Physics.Raycast(detectRay, out hitInfo, shooterOffset + 1))             
            {
        
                GameObject dart = GetNextDart();
                if (dart != null)
                {
                    dart.SetActive(true);
                    dart.transform.position = this.transform.position + transform.forward * shooterOffset;
                    dart.transform.rotation = this.transform.rotation;
                    dart.GetComponent<Rigidbody>().velocity = transform.forward * dartSpeed;
                    AudioManager.Play(shootSound, false, 1, 0.8f, 1.2f);
                    shootParticles.Play();
                }
            }
            Invoke("Shoot", shootCooldown);
        }
    }

    private GameObject GetNextDart()
    {
        GameObject nextDart=null;
        for(int i=0;i<projectilePool.Count;i++)
        {
            if(!projectilePool[i].activeInHierarchy)
            {
                nextDart = projectilePool[i];
                break;
            }
        }
        if(nextDart==null && projectilePool.Count<projectilePoolSize)
        {
            nextDart = Instantiate(dartPrefab);
            projectilePool.Add(nextDart);
        }
        return nextDart;
    }
}
