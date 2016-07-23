using UnityEngine;
using System.Collections;

public class ParallaxCatcher : MonoBehaviour {

    public Transform target;

    private Vector3 relLightPos;
	// Use this for initialization
	void Start () {
	    
        relLightPos = transform.position - target.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //transform.position = player.position + relLightPos;
        transform.position = new Vector3(target.position.x + relLightPos.x,transform.position.y,target.position.z + relLightPos.z);
	}
}
