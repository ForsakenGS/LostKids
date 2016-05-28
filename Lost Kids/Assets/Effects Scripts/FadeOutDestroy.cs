using UnityEngine;
using System.Collections;

public class FadeOutDestroy : MonoBehaviour {

    public float fadeTime=1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FadeAndDestroy()
    {
        iTween.FadeTo(this.gameObject, iTween.Hash( "alpha",0f,"time",fadeTime, "onComplete","DestroyThis"));
    }

    void DestroyThis()
    {
        Destroy(this.gameObject);
    }

}
