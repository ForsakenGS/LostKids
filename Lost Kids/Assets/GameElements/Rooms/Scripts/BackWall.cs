using UnityEngine;
using System.Collections;

public class BackWall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
    }

    public void Hide() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
    }
}
