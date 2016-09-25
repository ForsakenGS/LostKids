using UnityEngine;
using System.Collections;

public class TimedDirectionArrow : MonoBehaviour {

    public float inactiveTime;
    private bool reached = false;
    void OnEnable()
    {
        GetComponent<Renderer>().enabled = false;
        iTween.ScaleTo(gameObject, Vector3.zero, 1);
        Invoke("ShowDirection", inactiveTime);
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void ShowDirection()
    {
        GetComponent<Renderer>().enabled = true;
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "time", 2));
    }

    void HideDirection() {
        iTween.ScaleTo(gameObject,iTween.Hash("scale",Vector3.zero,"time",2, "onComplete", "Hide"));
    }

    void Hide() {
        GetComponent<Renderer>().enabled = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if(!reached && CharacterManager.IsActiveCharacter(col.gameObject))
        {
            HideDirection();
        }
    }
}
