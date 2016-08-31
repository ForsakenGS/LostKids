using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CooldownNotification : MonoBehaviour {

    public float duration = 0.5f;
    public float maxAlpha = 0.4f;
    public float fadeTime = 0.4f;


	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowNotification(Sprite spr) {
        StopAllCoroutines();
        GetComponent<Image>().sprite = spr;
        GetComponent<Image>().color = new Color(1,1,1,0);
        Invoke("HideNotification", duration);
        iTween.FadeTo(gameObject, maxAlpha, fadeTime);
        
    }

    public void HideNotification() {
       iTween.FadeTo(gameObject, 0, fadeTime);
    }
}
