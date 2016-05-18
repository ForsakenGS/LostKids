using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour {

    GameObject mainPanel;
    GameObject settingsPanel;
    GameObject menuConfirmPanel;
    GameObject desktopConfirmPanel;

    public Slider masterSlider;
    public Slider soundsSlider;
    public Slider musicSlider;

    public Toggle masterToggle;
    public Toggle soundsToggle;
    public Toggle musicToggle;

    public Toggle fullScreenToggle;
    
    

    //Instancia para el singleton
    public static PausePanel instance { get; private set; }
    // Use this for initialization

    void Awake()
    {
        if (instance != null && instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
        if (instance == null)
        {
            instance = this;
            mainPanel = transform.Find("MainPanel").gameObject;
            settingsPanel = transform.Find("MainPanel/Settings").gameObject;
            menuConfirmPanel = transform.Find("MainPanel/MenuConfirm").gameObject;
            desktopConfirmPanel = transform.Find("MainPanel/DesktopConfirm").gameObject;
            if(musicSlider==null)
                musicSlider = transform.Find("MainPanel/Settings/Sounds/Music").GetComponentInChildren<Slider>();
            if (masterSlider == null)
                masterSlider = transform.Find("MainPanel/Settings/Sounds/Master").GetComponentInChildren<Slider>();
            if (soundsSlider == null)
                soundsSlider = transform.Find("MainPanel/Settings/Sounds/Sounds").GetComponentInChildren<Slider>();
            if(musicToggle==null)
                musicToggle = transform.Find("MainPanel/Settings/Sounds/Music").GetComponentInChildren<Toggle>();
            if (masterToggle == null)
                masterToggle = transform.Find("MainPanel/Settings/Sounds/Master").GetComponentInChildren<Toggle>();
            if (soundsToggle == null)
                soundsToggle = transform.Find("MainPanel/Settings/Sounds/Sounds").GetComponentInChildren<Toggle>();
            if(fullScreenToggle==null)
            {
                fullScreenToggle = transform.Find("MainPanel/Settings/Graphics").GetComponentInChildren<Toggle>();
            }

        }
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


	
	}



    public void Show()
    {
        
        mainPanel.SetActive(true);
    }

    public void Hide()
    {
        GameManager.ResumeGame();
        mainPanel.SetActive(false);
    }

    public void ShowMenuConfirmationPanel()
    {
        menuConfirmPanel.SetActive(true);
    }
    public void HideMenuConfirmationPanel()
    {
        menuConfirmPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ShowDesktopConfirmationPanel()
    {
        desktopConfirmPanel.SetActive(true);
    }
    public void HideDesktopConfirmationPanel()
    {
        desktopConfirmPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ShowSettingsPanel()
    {
        UpdateSettings();
        settingsPanel.SetActive(true);
        
    }


    /// <summary>
    /// Actualiza los valores que se muestran en la pantalla de opciones, leyendolos de las preferencias de juego
    /// </summary>
    public void UpdateSettings()
    {
        float volume= GameSettings.GetMasterVolume();
        masterSlider.value = volume;

        if (masterToggle.isOn && volume > 0)
        {
            masterToggle.isOn = false;
        }
        else if(!masterToggle.isOn && volume<=0)
        {
            masterToggle.isOn = true;
        }


        volume = GameSettings.GetMusicVolume();
        musicSlider.value = volume;

        if (musicToggle.isOn && volume > 0)
        {
            musicToggle.isOn = false;
        }
        else if (!musicToggle.isOn && volume <= 0)
        {
            musicToggle.isOn = true;
        }

        volume = GameSettings.GetSoundsVolume();
        soundsSlider.value = volume;

        if (soundsToggle.isOn && volume > 0)
        {
            soundsToggle.isOn = false;
        }
        else if (!soundsToggle.isOn && volume <= 0)
        {
            soundsToggle.isOn = true;
        }


        if (!fullScreenToggle.isOn && GameSettings.IsFullScreen())
        {
            fullScreenToggle.isOn = true;
        }
        else if(fullScreenToggle.isOn && !GameSettings.IsFullScreen())
        {
            fullScreenToggle.isOn = false;
        }

    }

    public void HideSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void ChangeScene(string sc)
    {
        GameManager.GoToScene(sc);
    }

    public void QuitGame()
    {
        GameManager.QuitGame();
    }

    public void ChangeMasterVolume(float value)
    {

        GameSettings.SetMasterVolume(value);

        if (value > 0 && masterToggle.isOn)
        {
            masterToggle.isOn = false;
        }
    }

    public void ChangeMusicVolume(float value)
    {
        GameSettings.SetMusicVolume(value);
        if (value > 0 && musicToggle.isOn)
        {
            musicToggle.isOn = false;
        }
    }

    public void ChangeSoundsVolume(float value)
    {
        GameSettings.SetSoundsVolume(value);
        if (value > 0 && soundsToggle.isOn)
        {
            soundsToggle.isOn = false;
        }

    }


    public void MuteMusic(bool mute)
    {
        if(mute)
        {
            musicSlider.value = 0;
        }
    }

    public void MuteMaster(bool mute)
    {
        if (mute)
        {
            masterSlider.value = 0;
        }
    }

    public void MuteSounds(bool mute)
    {
        if (mute)
        {
            soundsSlider.value = 0;
        }
    }


    public void ChangeFullScreen(bool value)
    {
        GameSettings.SetFullScreen(value);
    }

    public static void ShowPanel()
    {
        instance.Show();
    }

    public static void HidePanel()
    {
        instance.Hide();
    }

    


}
