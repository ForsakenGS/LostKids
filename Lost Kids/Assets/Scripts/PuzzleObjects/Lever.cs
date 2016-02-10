using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Script que representa el comportamiento de una palanca
/// Hereda gran parte de sus variables y funciones de la clase UsableObject
/// </summary>
public class Lever : UsableObject {

    // Use this for initialization
    new void Start () {
        //Se llama al start de UsableObject
        base.Start();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Metodo que se activa al usar el objeto. Incluye un comportamiento base generico
    /// y comportamiendo especifico para el objeto
    /// </summary>
    new public void Use()
    {
        //Comportamiento generico de un usable. (Activar objeto o notificar al puzzle segun situacion)

        base.Use();
        
        //Es necesario añadir funcionalidad adicional como Sonido o animaciones
    }

    /// <summary>
    /// Metodo que se activa al dejar de usar el objeto. Incluye un comportamiento base generico
    /// y comportamiendo especifico para el objeto
    /// </summary>
    new public void CancelUse()
    {
        base.CancelUse();

        //Es necesario añadir funcionalidad adicional como Sonido o animaciones
    }

}
