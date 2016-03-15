using UnityEngine;
using System.Collections;

public class FadeOutDestroy : MonoBehaviour {

 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FadeAndDestroy()
    {
        iTween.FadeTo(this.gameObject, iTween.Hash( "alpha",0f,"time",0.5f, "onComplete","DestroyThis"));
    }

    void DestroyThis()
    {
        Destroy(this.gameObject);
    }

}
