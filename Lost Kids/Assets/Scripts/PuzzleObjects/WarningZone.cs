using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarningZone : MonoBehaviour {


    private List<string> affected;
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            if (affected.Contains(col.gameObject.name))
            {
                //col.gameObject.GetComponent<CharacterStatus>().ShowIcon(Scared,true);


            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //col.gameObject.GetComponent<CharacterStatus>().ShowIcon(Scared,false);
        }
    }

    public void SetAffectedCharacters(List<string> characters)
    {
        affected = characters;
    }

    void OnTriggerStay(Collider col)
    {
        if (CharacterManager.IsActiveCharacter(col.gameObject) && affected.Contains(col.gameObject.name))
        {

        }
    }

    public void EnableZone()
    {
        GetComponent<Collider>().enabled = true;
    }

    public void DisableZone()
    {
        GetComponent<Collider>().enabled = false;
    }
}
