using UnityEngine;
using System.Collections;

public class Tooltip : MonoBehaviour {
	public GameObject canvas;
	//private CameraManager cameraManager;

	// Use this for initialization
	void Start () {
		//Vector3 relativeCameraPos = cameraManager.CurrentCamera().transform.position;
		//transform.localRotation =Quaternion.LookRotation(relativeCameraPos );
		//cameraManager=GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
		//transform.LookAt (cameraManager.CurrentCamera().transform);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other){
		if ((other.gameObject.GetComponent<CharacterMovement> () != null) & (CharacterManager.IsActiveCharacter(other.gameObject))){
			canvas.SetActive (true);
		}
	}

	void OnTriggerExit (Collider other){
		canvas.SetActive (false);
	}
}
