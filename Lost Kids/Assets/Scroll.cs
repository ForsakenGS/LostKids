using UnityEngine;
using System.Collections;

public class Scroll : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 offset = new Vector2 (Time.time * 2, 0);

		//renderer.material.mainTextureOffset = offset;
	}
}
