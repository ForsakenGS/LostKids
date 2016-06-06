using UnityEngine;
using System.Collections;

public class RockController : MonoBehaviour {

    public GameObject wall;
    private KillerSphere rock;
    public bool isEnabled = true;

    [HideInInspector]
    public bool active = false;
	// Use this for initialization
	void Start () {
        rock = GetComponent<KillerSphere>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Activate()
    {
        active = true;
        rock.Launch();
        //wall.GetComponent<MeshRenderer>().enabled = false;
        //wall.GetComponent<BoxCollider>().isTrigger = true;
    }

    public void Reset()
    {
        CancelInvoke();
        active = false;
        rock.Reset();
        //wall.GetComponent<MeshRenderer>().enabled = true;
        //wall.GetComponent<BoxCollider>().isTrigger = false;
    }

    void Deactivate()
    {
        isEnabled = false;
        Reset();
    }
}
