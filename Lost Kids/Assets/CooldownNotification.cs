using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CooldownNotification : MonoBehaviour {


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
        Invoke("HideNotification", 0.8f);
        iTween.FadeTo(gameObject, 0.4f, 0.5f);
        
    }

    public void HideNotification() {
       iTween.FadeTo(gameObject, 0, 0.5f);
    }
}
