using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

public enum GameLevels
{
    Tutorial1,Tutorial2,Tutorial3,Level1_1,Level1_2, Level1_3, BossKappa
}


public class GameManager : MonoBehaviour {

    private AudioLoader audioLoader;

    private GameObject menuMusic;

    //Variable global para el estado pausado
    public static bool paused;

    //Delegates para el estado de pausa
    public delegate void PauseUnPauseEvent();
    public static event PauseUnPauseEvent PauseEvent;
    public static event PauseUnPauseEvent UnPauseEvent;

    //Listado de todos los niveles del juego
    static List<string> levelList;

    // Use this for initialization
    void Start () {
        Time.timeScale = 1;
        levelList = Enum.GetNames(typeof(GameLevels)).ToList();
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
                /*
                if (menuMusic != null)
                {
                    AudioManager.Stop(menuMusic.GetComponent<AudioLoader>().GetSound("MenuMusic"));
                    Destroy(menuMusic);
                }
                */
                break;

            case "Tutorial2":
                /*
                if (menuMusic != null)
                {
                    AudioManager.Stop(menuMusic.GetComponent<AudioLoader>().GetSound("MenuMusic"));
                    Destroy(menuMusic);
                }
                */
                break;
            case "Tutorial3":
                /*
                if (menuMusic != null)
                {
                    AudioManager.Stop(menuMusic.GetComponent<AudioLoader>().GetSound("MenuMusic"));
                    Destroy(menuMusic);
                }
                break;
                */
            case "Level1":
                /*
                if (menuMusic != null)
                {
                    AudioManager.Stop(menuMusic.GetComponent<AudioLoader>().GetSound("MenuMusic"));
                    Destroy(menuMusic);
                }
                break;
                */

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

    public static void CompleteLevel(string level)
    {
        GameData.CompleteLevel(level);

        int pos = levelList.IndexOf(level)+1;
        if(pos<levelList.Count)
        {
            GameData.UnlockLevel(levelList[pos]);
        }
        DataManager.Save();
    }


}



	
