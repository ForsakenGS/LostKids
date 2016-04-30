using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {

    private CameraManager cameraManager;
    public int room;

	// Use this for initialization
	void Start () {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();

	}

    void OnTriggerEnter(Collider other) {
        GameObject activeCharacter = CharacterManager.GetActiveCharacter();
        if(other.gameObject.Equals(activeCharacter)) {

            //Se actualiza la nueva habitacion en el character status
            activeCharacter.GetComponent<CharacterStatus>().currentRoom = room;

            //Se activa el cambio de cámara
            cameraManager.ChangeCamera(room);



        }
    }
}
