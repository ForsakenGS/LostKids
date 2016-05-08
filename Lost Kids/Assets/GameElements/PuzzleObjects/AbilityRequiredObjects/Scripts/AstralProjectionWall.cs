using UnityEngine;
using System.Collections;

public class AstralProjectionWall : MonoBehaviour {
    /// <summary>
    /// Distancia de detección del objeto respecto al personaje con la habilidad 'Telekinesis'
    /// </summary>
    public float detectionDistance = 2.0f;

    private AstralProjectionAbility ability;
    private Transform astralDestination;

    void Awake() {
        // Actualiza el collider con la distancia de detecciónç
        Vector3 size = GetComponent<BoxCollider>().size;
        Debug.Log(size);
        gameObject.GetComponent<BoxCollider>().size = new Vector3(size.x,size.y,detectionDistance);
    }

    // Use this for initialization
    void Start() {
        // Inicialización variables
        ability = null;
        astralDestination = transform.FindChild("AstralDestination");
    }

    /// <summary>
    /// Devuelve el punto en el que se debe crear la proyección astral del personaje
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAstralProjectionPosition() {
        return astralDestination.position;
    }

    void OnTriggerEnter(Collider col) {
        // Comprueba que la colisión se realiza con el jugador con la habilidad 'AstralProjection'
        ability = col.gameObject.GetComponent<AstralProjectionAbility>();
        if (ability != null) {
            // Asigna la pared a la habilidad
            ability.SetWall(gameObject.GetComponent<AstralProjectionWall>());
        }
    }

    void OnTriggerExit(Collider col) {
        // Comprueba si se ha estado en colisión con el jugador de la habilidad 'AstralProjection' y es el mismo que abandona el collider
        if ((ability != null) && (col.gameObject.GetComponent<TelekinesisAbility>() != null)) {
            // La pared deja de poder ser activable con la habilidad
            ability.SetWall(null);
            ability = null;
        }
    }
}
