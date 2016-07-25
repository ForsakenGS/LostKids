using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapTrigger : MonoBehaviour {

    public List<AbstractTrap> targets;

    public bool oneShot;

    public bool disableOnShot;

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
            if(oneShot) {
                foreach (AbstractTrap trap in targets)
                {
                    trap.FireTrapOneShot();
                }
            } else {
                foreach (AbstractTrap trap in targets)
                {
                    trap.FireTrap();
                }
            }
            if (disableOnShot) {
                this.gameObject.SetActive(false);
            }

        }
    }
}
