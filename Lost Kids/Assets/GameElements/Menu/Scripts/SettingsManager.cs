using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour {

    public Slider masterSlider;
    public Slider soundsSlider;
    public Slider musicSlider;

    public Toggle masterToggle;
    public Toggle soundsToggle;
    public Toggle musicToggle;

    public Toggle fullScreenToggle;
    public Toggle tooltipsToggle;

    //Instancia para el singleton
    public static PausePanel instance { get; private set; }
    // Use this for initialization

    void OnEnable() {
        UpdateSettings();
        EventSystem.current.SetSelectedGameObject(masterSlider.gameObject);
    }


    void Awake() {
        if (instance != null && instance != this) {

            Destroy(gameObject);
        }
        if (instance == null) {

            if (musicSlider == null)
                musicSlider = transform.Find("Sounds/Music").GetComponentInChildren<Slider>();
            if (masterSlider == null)
                masterSlider = transform.Find("Sounds/Master").GetComponentInChildren<Slider>();
            if (soundsSlider == null)
                soundsSlider = transform.Find("Sounds/Sounds").GetComponentInChildren<Slider>();
            if (musicToggle == null)
                musicToggle = transform.Find("Sounds/Music").GetComponentInChildren<Toggle>();
            if (masterToggle == null)
                masterToggle = transform.Find("Sounds/Master").GetComponentInChildren<Toggle>();
            if (soundsToggle == null)
                soundsToggle = transform.Find("Sounds/Sounds").GetComponentInChildren<Toggle>();
            if (fullScreenToggle == null) {
                fullScreenToggle = transform.Find("Graphics/FullScreen/FullScreenCheck").GetComponent<Toggle>();
            }
            if (tooltipsToggle == null) {
                tooltipsToggle = transform.Find("Graphics/Tooltips/TooltipsCheck").GetComponent<Toggle>();
            }
            UpdateSettings();
        }

    }
    /// <summary>
    /// Actualiza los valores que se muestran en la pantalla de opciones, leyendolos de las preferencias de juego
    /// </summary>
    public void UpdateSettings() {
 
        float volume = GameSettings.GetMusicVolume();
        musicSlider.value = volume;

        if (musicToggle.isOn && volume > 0) {
            musicToggle.isOn = false;
        } else if (!musicToggle.isOn && volume <= 0) {
            musicToggle.isOn = true;
        }

        volume = GameSettings.GetSoundsVolume();
        soundsSlider.value = volume;

        if (soundsToggle.isOn && volume > 0) {
            soundsToggle.isOn = false;
        } else if (!soundsToggle.isOn && volume <= 0) {
            soundsToggle.isOn = true;
        }

        if (!fullScreenToggle.isOn && GameSettings.IsFullScreen()) {
            fullScreenToggle.isOn = true;
        } else if (fullScreenToggle.isOn && !GameSettings.IsFullScreen()) {
            fullScreenToggle.isOn = false;
        }

        if (!tooltipsToggle.isOn && TooltipManager.On) {
            tooltipsToggle.isOn = true;
        } else if (tooltipsToggle.isOn && !TooltipManager.On) {
            tooltipsToggle.isOn = false;
        }

    }
    
    public void ChangeMasterVolume(float value) {

        GameSettings.SetMasterVolume(value);

        if (value > -80 && masterToggle.isOn) {
            masterToggle.isOn = false;
        }
    }

    public void ChangeMusicVolume(float value) {
        GameSettings.SetMusicVolume(value);
        if (value > -80 && musicToggle.isOn) {
            musicToggle.isOn = false;
        }
    }

    public void ChangeSoundsVolume(float value) {
        GameSettings.SetSoundsVolume(value);
        if (value > -80 && soundsToggle.isOn) {
            soundsToggle.isOn = false;
        }

    }


    public void MuteMusic(bool mute) {
        if (mute) {
            musicSlider.value = -80;
        }
    }

    public void MuteMaster(bool mute) {
        if (mute) {
            masterSlider.value = -80;
        }
    }

    public void MuteSounds(bool mute) {
        if (mute) {
            soundsSlider.value = -80;
        }
    }

    public void ChangeFullScreen(bool value) {
        GameSettings.SetFullScreen(value);
    }

    public void ChangeTooltips(bool value) {
        TooltipManager.On = value;
    }
}
