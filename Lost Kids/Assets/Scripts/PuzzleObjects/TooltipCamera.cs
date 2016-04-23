using UnityEngine;
using System.Collections;

public class TooltipCamera : MonoBehaviour {
	private CameraManager cameraManager;

	// Use this for initialization
	void Start () {
		Vector3 relativeCameraPos = cameraManager.CurrentCamera().transform.position;
		transform.localRotation =Quaternion.LookRotation(relativeCameraPos );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
