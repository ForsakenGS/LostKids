using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {

    private CameraController cameraController;
    public int room;

	// Use this for initialization
	void Start () {
        cameraController = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraController>();
	}

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {

            cameraController.ChangeCamera(room);

        }
    }
}
