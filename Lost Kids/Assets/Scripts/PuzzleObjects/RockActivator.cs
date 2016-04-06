using UnityEngine;
using System.Collections;

public class RockActivator : MonoBehaviour {
	public GameObject rock;

    private RockController rockController;

	// Use this for initialization
	void Start () {
        rockController = rock.GetComponent<RockController>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (rockController.isEnabled && !rockController.active)
            {
                rock.GetComponent<RockController>().Activate();
            }
        }
    }
}
