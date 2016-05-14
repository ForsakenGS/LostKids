using UnityEngine;
using System.Collections;

public class AstralProjectionWall : MonoBehaviour {
    /// <summary>
    /// Distancia de detección del objeto respecto al personaje con la habilidad 'Telekinesis'
    /// </summary>
    public float detectionDistance = 2.0f;

    private Transform astralDestination;

    void Awake() {
        // Actualiza el collider del TooltipDetector
        BoxCollider bc = transform.Find("TooltipDetector").GetComponent<BoxCollider>();
        Vector3 size = bc.size;
        bc.size = new Vector3(size.x, size.y, detectionDistance);
        bc.center = new Vector3(0, 0, -detectionDistance / 2);
    }

    // Use this for initialization
    void Start() {
        // Inicialización variables
        astralDestination = transform.FindChild("AstralDestination");
    }

    /// <summary>
    /// Devuelve el punto en el que se debe crear la proyección astral del personaje
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAstralProjectionPosition() {
        return astralDestination.position;
    }
}