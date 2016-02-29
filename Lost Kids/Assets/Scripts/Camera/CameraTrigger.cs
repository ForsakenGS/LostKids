﻿using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {

    private CameraManager cameraManager;
    private CharacterManager characterManager;
    public int room;

	// Use this for initialization
	void Start () {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        characterManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<CharacterManager>();
	}

    void OnTriggerEnter(Collider other) {
        GameObject activeCharacter = characterManager.GetActiveCharacter();
        if(other.gameObject.Equals(activeCharacter)) {

            //Se activa el cambio de cámara
            cameraManager.ChangeCamera(room);

            //Se actualiza la nueva habitacion en el character status
            activeCharacter.GetComponent<CharacterStatus>().currentRoom = room;

        }
    }
}
