using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Bell : UsableObject {

    AudioSource bellSound;

    void Awake()
    {
        bellSound = GetComponent<AudioSource>();
    }
    // Use this for initialization
    void Start() {
        base.Start();
    }

    // Update is called once per frame
    void Update() {

    }

    public override void Use()
    {
        base.Use();
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        PlaySound();
        iTween.PunchRotation(gameObject, iTween.Hash("z", -70, "time", 2f,"delay",0.5f));
        Invoke("ResetUse", 2.1f);
    }

    public void PlaySound()
    {
        AudioManager.Play(bellSound, false, 1);
    }

    public void ResetUse()
    {
        onUse = false;
    }


}
