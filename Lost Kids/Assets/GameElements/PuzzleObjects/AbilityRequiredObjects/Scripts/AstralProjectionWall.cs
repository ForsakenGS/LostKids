using UnityEngine;
using System.Collections;

public class AstralProjectionWall : MonoBehaviour {
    private Transform astralDestination;

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