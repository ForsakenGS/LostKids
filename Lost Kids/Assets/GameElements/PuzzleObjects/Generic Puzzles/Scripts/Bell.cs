using UnityEngine;
using System.Collections;

[RequireComponent(typeof (AudioSource))]
public class Bell : UsableObject {

    AudioSource bellSound;

    void Awake()
    {
        bellSound = GetComponent<AudioSource>();
    }
	// Use this for initialization
	void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Use()
    {
        base.Use();
    }

    public void PlayAnimation()
    {
        //Se ejecuta la animacion de movimiento de la campana 
        //( se ha separado del sonido para poder hacerlo opcional)
    }

    public void PlaySound()
    {
        AudioManager.Play(bellSound,false,1);
    }


}
