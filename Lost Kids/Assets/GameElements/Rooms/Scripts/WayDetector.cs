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
    private BackWall backWall;

    // Use this for references
    void Awake() {
        // Configuración general del pasillo
        way = transform.parent.parent.GetComponent<WaySettings>();
        backWall = way.GetComponentInChildren<BackWall>();

        if(backWall) backWall.Hide();
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
                affectedRoomSettings.ShowRoom();
                GetComponent<Collider>().enabled = false;
            } else if (affectedRoom.activeInHierarchy) {
                // Oculta la habitación
                affectedRoomSettings.HideRoom();
                GetComponent<Collider>().enabled = false;
                //Muestra la pared de no retorno
                backWall.Show();
                // Nombre puzzle, para debug
                HUDManager.SetPuzzleName("No Puzzle");
            }
        }
    }
}
