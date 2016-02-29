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

        GetComponent<Renderer>().enabled = true;
    }

    public void Hide()
    {
        visible = false;
        GetComponent<Renderer>().enabled = false;
    }
}
