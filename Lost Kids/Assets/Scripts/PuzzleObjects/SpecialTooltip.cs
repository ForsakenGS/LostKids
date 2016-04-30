using UnityEngine;
using System.Collections;

public class SpecialTooltip : MonoBehaviour {
	public GameObject canvas;
	public CharacterAbility ability;

	// Use this for initialization
	void Start () {
		
		//cameraManager=GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
		//transform.LookAt (cameraManager.CurrentCamera().transform);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other){
		if ((other.gameObject.GetComponent(ability.GetType())!=null) & (CharacterManager.IsActiveCharacter(other.gameObject))){
			canvas.SetActive (true);
		}
	}

	void OnTriggerExit (Collider other){
		canvas.SetActive (false);
	}
}
