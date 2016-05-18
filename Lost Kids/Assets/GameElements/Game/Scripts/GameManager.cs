using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private AudioLoader audioLoader;

    private GameObject menuMusic;

    //Variable global para el estado pausado
    public static bool paused;

    //Delegates para el estado de pausa
    public delegate void PauseUnPauseEvent();
    public static event PauseUnPauseEvent PauseEvent;
    public static event PauseUnPauseEvent UnPauseEvent;

    // Use this for initialization
    void Start () {
        menuMusic = GameObject.FindGameObjectWithTag("MenuMusic");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ExitApplication (){
        Application.Quit ();
	}

	public static void GoToScene(string sc) {
        SceneManager.LoadScene(sc);
    }

    public void PrepareScene(string sc) {
        switch(sc) {
            case "Tutorial1":
                AudioManager.Stop(menuMusic.GetComponent<AudioLoader>().GetSound("MenuMusic"));
                Destroy(menuMusic);
                break;
            case "Level2":
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

        if(sc.Equals("Exit")) {
            ExitApplication();
        } else {
            SceneManager.LoadScene(sc);
        }
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

    public static void PauseGame()
    {
        paused = true;
        Time.timeScale = 0;
        if(PauseEvent!=null)
        {
            PauseEvent();
        }
    }

    public static void ResumeGame()
    {
        paused = false;
        Time.timeScale = 1;
        if(UnPauseEvent!=null)
        {
            UnPauseEvent();
        }
    }

    public static void QuitGame()
    {
        Application.Quit();
    }


}



	
