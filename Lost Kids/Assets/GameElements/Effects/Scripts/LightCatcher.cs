using UnityEngine;
using System.Collections;

public class LightCatcher : MonoBehaviour {

    public Transform player;

    private Vector3 relLightPos;
	// Use this for initialization
	void Start () {
	    
        relLightPos = transform.position - player.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = player.position + relLightPos;
	}
}
