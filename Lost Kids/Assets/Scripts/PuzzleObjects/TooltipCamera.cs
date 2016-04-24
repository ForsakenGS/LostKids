using UnityEngine;
using System.Collections;

public class TooltipCamera : MonoBehaviour {
	private CameraManager cameraManager;
    Vector3 lookPosition;
	// Use this for initialization
	void Start () {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        transform.rotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void LateUpdate () {

        transform.rotation = Quaternion.identity;
        /*
        lookPosition = cameraManager.CurrentCamera().transform.position;
        lookPosition.y = transform.position.y;
        lookPosition.x = transform.position.x;
        transform.LookAt(lookPosition);
        */

        
        transform.LookAt(transform.position + cameraManager.CurrentCamera().transform.rotation * Vector3.forward,
            cameraManager.CurrentCamera().transform.rotation * Vector3.up);

        //transform.rotation *=  Quaternion.Inverse(transform.parent.rotation);

        //transform.rotation = cameraManager.CurrentCamera().transform.rotation;

    }
}
