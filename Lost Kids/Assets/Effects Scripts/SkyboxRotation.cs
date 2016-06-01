using UnityEngine;
using System.Collections;

public class SkyboxRotation : MonoBehaviour {

    private float rot;

    public float speed = 1;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        rot = RenderSettings.skybox.GetFloat("_Rotation");
        RenderSettings.skybox.SetFloat("_Rotation", rot += speed);
	}
}
