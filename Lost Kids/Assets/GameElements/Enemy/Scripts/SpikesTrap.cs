using UnityEngine;
using System.Collections;

public class SpikesTrap : AbstractTrap
{

    public ParticleSystem moveParticles;

    public float moveTime = 0.5f;
    public bool loop = true;

    public float activeTime = 1f;
    public float inactiveTime = 1.5f;

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



    public void Show()
    {
        if (isEnabled)
        {
            active = true;
            moveParticles.Play();
            AudioManager.Play(moveSound, false, 1, 0.8f, 1.1f);
            iTween.MoveTo(gameObject, iTween.Hash("y", 0, "time", moveTime, "easetype", iTween.EaseType.easeOutQuad, "oncomplete", "Active"));
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
        moveParticles.Play();
        AudioManager.Play(moveSound, false, 1, 0.8f, 1.1f);
        iTween.MoveTo(gameObject, iTween.Hash("y", -2, "time", moveTime, "easetype", iTween.EaseType.easeInQuad, "oncomplete", "Inactive"));    
    }

    public void Inactive()
    {
        if(loop)
        {
            Invoke("Show", inactiveTime);
        }
    }

}
