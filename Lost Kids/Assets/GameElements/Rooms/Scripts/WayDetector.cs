using UnityEngine;
using System.Collections;

public class WayDetector : MonoBehaviour {
    /// <summary>
    /// Indica si la función que se ejecutará tras la detección será para mostrar u ocultar
    /// </summary>
    public bool toShow = true;
    /// <summary>
    /// Indica si afecta a la habitación que le precede o la siguiente
    /// </summary>
    public bool isNext = true;

    private GameObject affectedRoom;
    private RoomSettings affectedRoomSettings;
    private WaySettings way;

    // Use this for references
    void Awake() {
        // Configuración general del pasillo
        way = transform.parent.parent.GetComponent<WaySettings>();

    }

    // Use this for initialization
    void Start() {
        // Habitación a la que afecta
        if (isNext)
        {
            affectedRoomSettings = way.nextRoom;
        }
        else {
            affectedRoomSettings = way.prevRoom;
        }
        if (affectedRoomSettings == null)
        {
            Destroy(gameObject);
        }
        else {
            affectedRoom = affectedRoomSettings.gameObject;
        }

    }

    // Se ejecuta al entrar en contacto con otro GameObject
    void OnTriggerEnter(Collider col) {
        // Comprueba que sea el jugador activo
        if (CharacterManager.IsActiveCharacter(col.gameObject)) {
            // Comprueba si el efecto es de mostrar u ocultar
            if (toShow) {
                affectedRoomSettings.enabled = true;
                if (!affectedRoom.activeInHierarchy) {
                    // La habitación no está habilitada, luego la habilita y muestra
                    affectedRoom.SetActive(true);
                } else {
                    // Muestra la habitación
                    affectedRoomSettings.ShowRoom();
                }
            } else if (affectedRoom.activeInHierarchy) {
                // Oculta la habitación
                affectedRoomSettings.HideRoom();
            }
        }
    }
}
