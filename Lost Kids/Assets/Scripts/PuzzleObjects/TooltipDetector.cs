using UnityEngine;
using System.Collections;

public class TooltipDetector : MonoBehaviour {


    public Sprite tooltipImage;
    public CharacterAbility requiredAbility;

    private CharacterIcon icon;
    //private CameraManager cameraManager;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other){
        if (tooltipImage != null)
        {
            if (CharacterManager.IsActiveCharacter(other.gameObject))
            {
                if (requiredAbility == null || other.gameObject.GetComponent(requiredAbility.GetType()) != null)

                    icon = other.gameObject.GetComponentInChildren<CharacterIcon>();
                icon.ActiveCanvas(true);
                icon.SetImage(tooltipImage);

            }
        }
	}

	void OnTriggerExit (Collider other){
        if (tooltipImage != null)
        {
            if (CharacterManager.IsActiveCharacter(other.gameObject))
            {
                icon.ActiveCanvas(false);
            }
        }
	}
}
