using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FearZone : MonoBehaviour {

    private List<string> affected;

 
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (affected.Contains(col.gameObject.name))
            {
                GetComponentInParent<FearSource>().CharacterOnFearZone(col.gameObject, true);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            GetComponentInParent<FearSource>().CharacterOnFearZone(col.gameObject, false);
        }
    }

    public void SetAffectedCharacters(List<string> characters)
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
