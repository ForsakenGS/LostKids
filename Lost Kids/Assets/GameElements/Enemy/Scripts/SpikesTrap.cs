using UnityEngine;
using System.Collections;
using System;

public class SpikesTrap : AbstractTrap
{

    public ParticleSystem moveParticles;

    public float moveTime = 0.5f;
    public bool loop = true;

    public float activeTime = 1f;
    public float inactiveTime = 1.5f;

    public float moveDistance = 2.0f;

    private Vector3 initPosition;

    AudioSource moveSound;

    void Awake()
    {
        
        if (moveParticles == null)
        {
            moveParticles = GetComponentInChildren<ParticleSystem>();

        }
        moveSound = GetComponent<AudioSource>();

    }
    // Use this for initialization
    void Start()
    {
        initPosition = transform.position;

        if (enabled && fireOnEnable)
        {
            Show();
        }

    }

    // Update is called once per frame
    void Update()
    {

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
        CancelInvoke();
        iTween.Stop();
        Hide();

    }

    public override void EnableTrap()
    {
        isEnabled = true;
        if (fireOnEnable)
        {
            FireTrap();
        }

    }

    public override void FireTrap()
    {
        if (isEnabled && !active)
        {
            Show();
        }

    }


    public override void FireTrapOneShot()
    {
        FireTrap();
        Invoke("Hide", activeTime);
    }


    public void Show()
    {
        if (isEnabled)
        {
            active = true;
            //GetComponent<Renderer>().enabled = true;
            moveParticles.Play();
            AudioManager.Play(moveSound, false, 1, 0.8f, 1.1f);
            iTween.MoveTo(gameObject, iTween.Hash("y", initPosition.y+moveDistance, "time", moveTime, "easetype", iTween.EaseType.easeOutQuad, "oncomplete", "Active"));
        }
    }

    public void Active()
    {
        if(loop)
        {
            Invoke("Hide", activeTime);
        }
    }

    public void Hide()
    {
        active = false;
        //GetComponent<Renderer>().enabled = false;
        moveParticles.Play();
        AudioManager.Play(moveSound, false, 1, 0.8f, 1.1f);
        iTween.MoveTo(gameObject, iTween.Hash("y", initPosition.y, "time", moveTime, "easetype", iTween.EaseType.easeInQuad, "oncomplete", "Inactive"));    
    }

    public void Inactive()
    {
        if(loop)
        {
            Invoke("Show", inactiveTime);
        }
    }

}
