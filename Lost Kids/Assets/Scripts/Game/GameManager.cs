using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private AudioLoader audioLoader;

	// Use this for initialization
	void Start () {
        audioLoader = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioLoader>();
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
            case "Level1 Corregido":
                AudioManager.Stop(audioLoader.GetSound("MenuMusic"));
                break;
            case "options":
                break;
            case "credits":
                break;
            case "loadScene":
                break;
            case "menu":
                break;
            case "yokai":
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



	
