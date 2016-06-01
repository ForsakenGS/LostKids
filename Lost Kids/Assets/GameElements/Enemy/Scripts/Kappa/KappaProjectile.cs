﻿using UnityEngine;
using System.Collections;

public class KappaProjectile : MonoBehaviour {

    
    void OnEnable()
    {
        
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (!col.isTrigger)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<CharacterStatus>().Kill();
            }
            Invoke("Destroy", 0);
        }
    }
}
