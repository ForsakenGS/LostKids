using UnityEngine;

public class TelekinesisObject : MonoBehaviour {
    /// <summary>
    /// Distancia de detección del objeto respecto al personaje con la habilidad 'Telekinesis'
    /// </summary>
    public float detectionDistance = 2.0f;

    private TelekinesisAbility ability;

    void Awake() {
        // Actualiza el collider con la distancia de detección
        gameObject.GetComponent<SphereCollider>().radius = detectionDistance;
    }

    // Use this for initialization
    void Start() {
        // Inicialización variables
        ability = null;
    }

    void OnTriggerEnter(Collider col) {
        // Comprueba que la colisión se realiza con el jugador con la habilidad 'Telekinesis'
        if ((ability == null) && (col.gameObject.tag.Equals("Player"))) {
            ability = col.gameObject.GetComponent<TelekinesisAbility>();
        }
        if (ability != null) {
            // Asigna el objeto a la habilidad
            ability.SetUsableObject(gameObject.GetComponent<UsableObject>());
        }
    }

    void OnTriggerExit(Collider col) {
        // Comprueba si se ha estado en colisión con el jugador de la habilidad 'Telekinesis' y es el mismo que abandona el collider
        if ((ability != null) && (col.gameObject.GetComponent<TelekinesisAbility>() != null)) {
            // El objeto deja de poder ser activable con la habilidad
            ability.SetUsableObject(null);
            ability = null;
        }
    }
}
