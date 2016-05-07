using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarningZone : MonoBehaviour {

    private List<CharacterName> affected;

    public Sprite tooltipImage;
    private CharacterIcon icon;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player") && affected.Contains(col.gameObject.GetComponent<CharacterStatus>().characterName))
        {

                icon = col.gameObject.GetComponentInChildren<CharacterIcon>();
                icon.ActiveCanvas(true);
                icon.SetImage(tooltipImage);

        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            icon.ActiveCanvas(false);
        }
    }

    public void SetAffectedCharacters(List<CharacterName> characters)
    {
        affected = characters;
    }

    void OnTriggerStay(Collider col)
    {
        if (CharacterManager.IsActiveCharacter(col.gameObject) 
            && affected.Contains(col.gameObject.GetComponent<CharacterStatus>().characterName))
        {

        }
    }

    public void EnableZone()
    {
        GetComponent<Collider>().enabled = true;
    }

    public void DisableZone()
    {
        icon.ActiveCanvas(false);
        GetComponent<Collider>().enabled = false;
    }
}
