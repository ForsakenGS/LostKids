using UnityEngine;
using System.Collections;

public class SpecialTooltipCamera : MonoBehaviour {
	public Transform cameraManager;
	//public GameObject canvas;



	// Use this for initialization
	void Start () {
		//Vector3 relativeCameraPos = cameraManager.CurrentCamera().transform.position;
		//transform.localRotation =Quaternion.LookRotation(relativeCameraPos );
		//cameraManager=GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>().transform;
		//transform.LookAt (cameraManager.CurrentCamera().transform);
		//transform.rotation=Quaternion.identity;
		//this.transform.LookAt(cameraManager);


	}


	
	// Update is called once per frame
	void Update () {
		transform.LookAt(cameraManager.position);
		transform.Rotate(0,90, 0);


}
}
