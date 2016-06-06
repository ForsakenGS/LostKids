using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapTrigger : MonoBehaviour {

    public List<AbstractTrap> targets;

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
            foreach(AbstractTrap trap in targets)
            {
                trap.FireTrap();
            }
        }
    }
}
