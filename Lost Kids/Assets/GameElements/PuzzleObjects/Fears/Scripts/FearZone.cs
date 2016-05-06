using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FearZone : MonoBehaviour {

    private List<CharacterName> affected;

 
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player")
            && affected.Contains(col.gameObject.GetComponent<CharacterStatus>().characterName))
            {
                GetComponentInParent<FearSource>().CharacterOnFearZone(col.gameObject, true);
            }
        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            GetComponentInParent<FearSource>().CharacterOnFearZone(col.gameObject, false);
        }
    }

    public void SetAffectedCharacters(List<CharacterName> characters)
    {
        affected = characters;
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
