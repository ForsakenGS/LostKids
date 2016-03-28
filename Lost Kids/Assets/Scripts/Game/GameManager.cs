using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private AudioLoader audioLoader;

    private GameObject menuMusic;

	// Use this for initialization
	void Start () {
        menuMusic = GameObject.FindGameObjectWithTag("MenuMusic");
        audioLoader = menuMusic.GetComponent<AudioLoader>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ExitApplication (){
        Application.Quit ();
	}

	public void GoToScene(string sc) {
        PrepareScene(sc);
	}

    public void PrepareScene(string sc) {
        switch(sc) {
            case "Level1":
                AudioManager.Stop(audioLoader.GetSound("MenuMusic"));
                Destroy(menuMusic);
                break;
            case "Settings":
                break;
            case "Credits":
                break;
            case "LoadScene":
                break;
            case "Menu":
                break;
            case "Yokai":
                break;
            
        }

        SceneManager.LoadScene(sc);
    }

    public void GoToStartGame(string sc) {
        SceneManager.LoadScene(sc);
    }

    public void GoToOptions(string sc) {
        SceneManager.LoadScene(sc);
    }

    public void GoToCredits(string sc) {
        SceneManager.LoadScene(sc);
    }

    public void GoToLoadGame(string sc) {
        SceneManager.LoadScene(sc);
    }

    public void GoToMenu(string sc) {
        SceneManager.LoadScene(sc);
    }

    public void GoToYokaisDocs(string sc) {
        SceneManager.LoadScene(sc);
    }
}



	
