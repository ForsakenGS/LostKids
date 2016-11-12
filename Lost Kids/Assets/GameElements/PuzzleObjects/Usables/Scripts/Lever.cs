using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Script que representa el comportamiento de una palanca
/// Hereda gran parte de sus variables y funciones de la clase UsableObject
/// </summary>
public class Lever : UsableObject {

    private AudioSource leverOnSound;
    private AudioSource leverOffSound;
    private Animator leverAnimator;

    void Awake() {
        leverAnimator = GetComponentInParent<Animator>();
    }

    // Use this for initialization
    new void Start () {
        //Se llama al start de UsableObject
        base.Start();

        leverOnSound = audioLoader.GetSound("LeverOn");
        leverOffSound = audioLoader.GetSound("LeverOff");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Metodo que se activa al usar el objeto. Incluye un comportamiento base generico
    /// y comportamiendo especifico para el objeto
    /// </summary>
    override public void Use()
    {
        //Comportamiento generico de un usable. (Activar objeto o notificar al puzzle segun situacion)

        if (!onUse)
        {
            base.Use();

            //Es necesario añadir funcionalidad adicional como Sonido o animaciones
            AudioManager.Play(leverOnSound, false, 1);
            leverAnimator.SetTrigger("UseActivation");
        }
    }

    /// <summary>
    /// Metodo que se activa al dejar de usar el objeto. Incluye un comportamiento base generico
    /// y comportamiendo especifico para el objeto
    /// </summary>
    override public void CancelUse()
    {
        if(onUse) {
            // Efectos de cancelación
            AudioManager.Play(leverOffSound, false, 1);
            leverAnimator.SetTrigger("UseDeactivation");
            // Cancela el uso
            base.CancelUse();
        }
    }

}
