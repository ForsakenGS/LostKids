using UnityEngine;
using System.Collections;

public class FlashingImage : MonoBehaviour {


    void OnEnable()
    {
        iTween.FadeTo(gameObject, iTween.Hash("alpha", 0f, "time", 0.2f, "delay",0.5f,"looptype", iTween.LoopType.pingPong));
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
