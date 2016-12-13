using UnityEngine;
using System.Collections;

public class FlashingImage : MonoBehaviour {


    void OnEnable()
    {
        if (GetComponent<iTween>() == null)
        {
            iTween.FadeTo(gameObject, iTween.Hash("alpha", 0f, "time", 0.2f, "delay", 0.5f, "looptype", iTween.LoopType.pingPong));
        }
        else
        {
            GetComponent<iTween>().isRunning = true;
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
