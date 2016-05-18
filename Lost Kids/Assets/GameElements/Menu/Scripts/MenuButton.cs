using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButton : MonoBehaviour {

    private AudioLoader audioLoader;

    private GameManager gameManager;

	// Use this for initialization
	void Start () {
        audioLoader = GetComponent<AudioLoader>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ButtonClicked(string sc) {
        AudioManager.Play(audioLoader.GetSound("Button"), false, 1);
        gameManager.PrepareScene(sc);
    }
}
