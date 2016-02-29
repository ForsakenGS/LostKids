using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {

    private CameraController cameraController;
    private CharacterStatus characterStatus;
    private CharacterManager characterManager;
    public int room;

	// Use this for initialization
	void Start () {
        cameraController = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraController>();
        characterStatus = GameObject.FindGameObjectWithTag("CharacterStatus").GetComponent<CharacterStatus>();
        characterManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<CharacterManager>();
	}

    void OnTriggerEnter(Collider other) {
        if(other.Equals(characterManager.GetActiveCharacter())) {

            //Se activa el cambio de cámara
            cameraController.ChangeCamera(room);

            //Se actualiza la nueva habitacion en el character status
            characterStatus.currentRoom = room;

        }
    }
}
