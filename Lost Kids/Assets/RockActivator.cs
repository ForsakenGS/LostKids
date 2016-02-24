using UnityEngine;
using System.Collections;

public class RockActivator : MonoBehaviour {
	public GameObject wall;
	public GameObject rock;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Player") {
			rock.GetComponent<KillerSphere> ().enabled = true;
			wall.GetComponent<MeshRenderer> ().enabled = false;
		}}
}
