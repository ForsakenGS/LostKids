using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseIgnorer : MonoBehaviour {
    GameObject lastselect;
    // Use this for initialization
    void Start () {
        lastselect = null;
    }
	
	// Update is called once per frame
	void Update () {
        if (EventSystem.current.currentSelectedGameObject == null && lastselect!=null)
        {
            EventSystem.current.SetSelectedGameObject(lastselect);
        }
        else
        {
            lastselect = EventSystem.current.currentSelectedGameObject;
        }

    }
}
