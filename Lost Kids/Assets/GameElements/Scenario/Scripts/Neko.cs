using UnityEngine;
using System.Collections;

public class Neko : MonoBehaviour {

    private bool visible;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show()
    {
        visible = true;

        //Animacion del gato

        Renderer[] nekoParts = GetComponentsInChildren<Renderer>();
        
        for(int i = 0; i < nekoParts.Length; i++) {
            nekoParts[i].enabled = true;
        }
        
    }

    public void Hide()
    {
        visible = false;
        
        Renderer[] nekoParts = GetComponentsInChildren<Renderer>();
        
        for(int i = 0; i < nekoParts.Length; i++) {
            nekoParts[i].enabled = true;
        }
    }

    public void MIAU()
    {
        AudioManager.Play(GetComponent<AudioSource>(), false, 0.5f);
    }


}
