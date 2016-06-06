using UnityEngine;
using System.Collections;

public class MouseDestroyer : MonoBehaviour {

 

    public bool disableOnStart = true;
	// Use this for initialization
	void Start () {
        
        if (disableOnStart)
        {
            DisableMouse();
        }
	    
	}
	
	// Update is called once per frame
	void Update () {


    }


    void DisableMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void EnableMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
