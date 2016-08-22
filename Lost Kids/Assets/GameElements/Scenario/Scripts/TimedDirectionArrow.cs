using UnityEngine;
using System.Collections;

public class TimedDirectionArrow : MonoBehaviour {

    public float inactiveTime;
    private bool reached = false;
    void OnEnable()
    {
        GetComponent<Renderer>().enabled = false;
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
        iTween.FadeFrom(gameObject, 0, 1);
    }

    void OnTriggerEnter(Collider col)
    {
        if(!reached && CharacterManager.IsActiveCharacter(col.gameObject))
        {
            Destroy(gameObject);
        }
    }
}
