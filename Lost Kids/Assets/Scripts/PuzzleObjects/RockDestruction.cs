using UnityEngine;
using System.Collections;

public class RockDestruction : MonoBehaviour {
	public GameObject rock;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Rock") {
			Destroy (rock.gameObject);
		}}
}
