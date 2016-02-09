using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	private static GameObject currentCamera;

	// Use this for initialization
	void Start () {
		currentCamera = GameObject.FindGameObjectWithTag("MainCamera"); // Hay que cambiarlo!
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Returns the camera is currently being used
	public static GameObject CurrentCamera() {
		return currentCamera;
	}
}