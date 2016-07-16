using UnityEngine;
using System.Collections.Generic;
using SmartLocalization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalizationManager : MonoBehaviour {
    public List<string> scenesWithPauseMenu;

    // Categoría de la escena
    private string sceneName;
    // Instancia única del manager
    private static LocalizationManager instance;
    // Gestor del idioma
    private LanguageManager languageManager;
    // Idioma elegido
    public string language {
        get { return _language; }
        set { _language = value; languageManager.ChangeLanguage(_language); }
    }
    public string _language;

    void Awake() {
        // Patrón Singleton
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (!instance.Equals(this)) {
            Destroy(gameObject);
        }
    }

    void OnLevelWasLoaded() {
        // Inicilización del gestor
        languageManager = LanguageManager.Instance;
        languageManager.ChangeLanguage(_language);
        languageManager.OnChangeLanguage += OnChangeLanguage;
        // Localización de la escena
        sceneName = SceneManager.GetActiveScene().name;
        LocalizateScene(sceneName);
    }

    void OnChangeLanguage(LanguageManager newLanguageManager) {
        languageManager = newLanguageManager;
        // Localización de la escena
        LocalizateScene(sceneName);
    }

    public void ChangeLanguage(string lang) {
        language = lang;
        languageManager.ChangeLanguage(language);
    }

    void LocalizateScene(string scene) {
        // Actualiza elementos Text de la intefaz
        UpdateTextComponents("TLK.UI." + sceneName);
        // Actualiza los componentes Text del menú de pausa, si está presente
        if (scenesWithPauseMenu.Contains(scene)) {
            UpdateTextComponents("TLK.PauseMenu");
        }
    }

    void UpdateTextComponents(string category) {
        // Obtiene las keys de la escena
        List<string> keys = languageManager.GetKeysWithinCategory(category);
        // Actualiza los componentes Text con key
        foreach (string key in keys) {
            string path = key.Substring(category.Length + 1).Replace('.', '/');
            Debug.Log(path);
            GameObject.Find(path).GetComponent<Text>().text = languageManager.GetTextValue(key);
        }
    }
}