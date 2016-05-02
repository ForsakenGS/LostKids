using UnityEngine;
using System.Collections;

/// <summary>
/// Script para controlar la configuración general de un pasillo entre dos habitaciones
/// </summary>
public class WaySettings : MonoBehaviour {
    /// <summary>
    /// Referencia a la <c>room</c> que precede 
    /// </summary>
    public RoomSettings prevRoom;
    /// <summary>
    /// Referencia a la <c>room</c> que se encuentra a continuación
    /// </summary>
    public RoomSettings nextRoom;

    private GameObject exit;

    // Use this for references & content generation
    void Awake() {
        // Generación automática de los elementos de decoración

        // Referencias a componentes de la habitación
        exit = transform.FindChild("Exit").gameObject;
    }

    // Use this for initialization
    void Start () {
	
	}

    /// <summary>
    /// Función para conocer la entrada a la habitación
    /// </summary>
    /// <returns><c>Vector3</c> con la posición de entrada</returns>
    public Vector3 GetEntry() {
        return transform.position;
    }

    /// <summary>
    /// Función para conocer la salida de la habitación
    /// </summary>
    /// <returns><c>Vector3</c> con la posición de salida</returns>
    public Vector3 GetExit() {
        return exit.transform.position;
    }
}
