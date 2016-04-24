using UnityEngine;
using System.Collections;

public class TooltipDetector : MonoBehaviour {
	public GameObject canvas;

    public CharacterAbility requiredAbility;
    //private CameraManager cameraManager;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other){
		if (CharacterManager.IsActiveCharacter(other.gameObject)){
            if(requiredAbility==null || other.gameObject.GetComponent(requiredAbility.GetType())!=null)
			    canvas.SetActive (true);
		}
	}

	void OnTriggerExit (Collider other){
        if (CharacterManager.IsActiveCharacter(other.gameObject))
        {
            canvas.SetActive(false);
        }
	}
}
