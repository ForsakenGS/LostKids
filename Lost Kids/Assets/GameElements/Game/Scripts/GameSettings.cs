using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public enum Languages {English,Spanish};


public class GameSettings :MonoBehaviour {

    public float masterVolume = 1;
    public float musicVolume = 1;
    public float soundsVolume = 1;

    public bool fullScreen = true;

    public Languages language = Languages.English;

    //Instancia del singleton
    public static GameSettings instance = null;

    private AudioMixer audioMixer;

    //Delegates para cambios de volumen
    public delegate void VolumeChangedEvent();
    public static event VolumeChangedEvent VolumeChanged;

    void Awake()
    {

        if (instance == null)
        {

            instance = this;

            masterVolume = 1;
            
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            soundsVolume = PlayerPrefs.GetFloat("SoundsVolume", 1f);
            UpdateAudioMixer();
            int fs = 1;
            if (!Screen.fullScreen) {
                fs = 0;
            }
            fullScreen = PlayerPrefs.GetInt("FullScreen", fs) == 1;
            string lang = PlayerPrefs.GetString("Language", "English");
            if (lang.Equals("Spanish"))
            {
                language = Languages.Spanish;
            }
            else
            {
                language = Languages.English;
            }
        }
        else if (instance != this)
        {

            Destroy(gameObject);

        }

        DontDestroyOnLoad(gameObject);

    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateAudioMixer()
    {
        if(audioMixer==null)
        {

            audioMixer = AudioManager.GetAudioMixer();
        }
        audioMixer.SetFloat("masterVolume", masterVolume);
        audioMixer.SetFloat("musicVolume", musicVolume);
        audioMixer.SetFloat("soundsVolume", soundsVolume);
    }

    public static float GetMusicVolume()
    {
        return  Instance.musicVolume;
    }

    public static float GetMasterVolume()
    {
        return Instance.masterVolume;
    }

    public static float GetSoundsVolume()
    {
        return Instance.soundsVolume;
    }

    /// <summary>
    /// Develve el lenguaje en que se encuentra el juego en el momento
    /// </summary>
    /// <returns></returns>
    public static Languages GetLanguage()
    {
        return Instance.language;
    }


    /// <summary>
    /// Devuelve si el juego se encuentra en modo pantalla completa
    /// </summary>
    /// <returns></returns>
    public static bool IsFullScreen()
    {
        return Instance.fullScreen;
    }

    /// <summary>
    /// Establece el valor de volumen global
    /// </summary>
    /// <param name="value"></param>
    public static void SetMasterVolume(float value)
    {
        Instance.masterVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        Instance.audioMixer.SetFloat("masterVolume", Instance.masterVolume);

    }

    /// <summary>
    /// Establece el valor del volumen de la musica
    /// </summary>
    /// <param name="value"></param>
    public static void SetMusicVolume(float value)
    {
        Instance.musicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        Instance.audioMixer.SetFloat("musicVolume", Instance.musicVolume);

    }

    /// <summary>
    /// Establece el valor del volumen de los sonidos
    /// </summary>
    /// <param name="value"></param>
    public static void SetSoundsVolume(float value)
    {
        Instance.soundsVolume = value;
        PlayerPrefs.SetFloat("SoundsVolume", value);
        Instance.audioMixer.SetFloat("soundsVolume", Instance.soundsVolume);
    }


    /// <summary>
    /// Cambia el modo de pantalla completa
    /// </summary>
    /// <param name="value"></param>
    public static void SetFullScreen(bool value)
    {
        Instance.fullScreen = value;

        Screen.fullScreen = value;
        if(value)
        {
            
            PlayerPrefs.SetInt("FullScreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("FullScreen", 0);
        }

    }

    /// <summary>
    /// Cambia el lenguaje del juego
    /// </summary>
    /// <param name="lang"></param>
    public static void ChangeLanguage(Languages lang)
    {
        Instance.language = lang;
        PlayerPrefs.SetString("Language",lang.ToString());
    }



    public static GameSettings Instance
    {
        //Devuelve la instancia actual o crea una nueva si aun no existe
        get { return instance ?? (instance = new GameObject("GameSettings").AddComponent<GameSettings>()); }
    }


}
