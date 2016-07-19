using UnityEngine;
using System.Collections;

public abstract class AbstractTrap : MonoBehaviour,IActivable {

    public bool fireOnEnable = false;
    public bool isEnabled = true;

    [HideInInspector]
    public bool active = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void DisableTrap();
    public abstract void EnableTrap();
    public abstract void FireTrap();
    public abstract void FireTrapOneShot();

    public abstract void Activate();
    public abstract void CancelActivation();
}
