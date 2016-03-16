using UnityEngine;
using System.Collections;

public class MortalZone2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.CompareTag("Player"))
		{
			col.gameObject.GetComponent<CharacterStatus>().Kill();
			Destroy (this.gameObject);
		}
	}
}

