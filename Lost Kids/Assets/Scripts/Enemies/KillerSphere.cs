using UnityEngine;
using System.Collections;

public class KillerSphere : MonoBehaviour {
	public int force;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody> ().AddForce (force * transform.forward);
	}
	
	// Update is called once per frame
	void Update () {
	}


}

