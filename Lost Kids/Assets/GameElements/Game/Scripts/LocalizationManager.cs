using UnityEngine;
using System.Collections.Generic;
using SmartLocalization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalizationManager : MonoBehaviour {
    public List<string> scenesWithPauseMenu;

    // Idioma elegido
    public static string language {
        get { return _language; }
        set { _language = value; languageManager.ChangeLanguage(_language); }
    }
    private static string _language;

    // Categoría de la escena
    private string sceneName;
    // Gestor del idioma
    private static LanguageManager languageManager;
    // Instancia de LocalizationManager
    private static LocalizationManager instance;

    // Constantes con los idiomas
    public static string ENLanguage = "en";
    public static string ESLanguage = "es";

    void Awake() {
        if (instance == null) {
            // Primera instancia de LocalizationManager
            instance = this;
            DontDestroyOnLoad(gameObject);
            _language = ESLanguage;
            InitializationAndLocalization();
        } else if (!instance.Equals(this)) {
            // Otra instancia creada
            Destroy(gameObject);
        }
    }

    void OnLevelWasLoaded() {
        // Comprueba si es la instancia correcta
        if ((instance != null) && (instance.Equals(this))) {
            InitializationAndLocalization();
        }
    }

    public void ChangeLanguage(string lang) {
        if (instance != null) {
            language = lang;
        }
    }

    void InitializationAndLocalization() {
        // Inicilización del gestor
        languageManager = LanguageManager.Instance;
        languageManager.ChangeLanguage(_language);
        languageManager.OnChangeLanguage += OnChangeLanguage;
        // Localización de la escena
        sceneName = SceneManager.GetActiveScene().name;
        LocalizateScene(sceneName);
    }

    void LocalizateScene(string scene) {
        // Actualiza elementos Text de la intefaz
        UpdateTextComponents("TLK.UI." + sceneName);
        // Actualiza los componentes Text del menú de pausa, si está presente
        if (scenesWithPauseMenu.Contains(scene)) {
            UpdateTextComponents("TLK.PauseMenu");
        }
    }

    void OnChangeLanguage(LanguageManager newLanguageManager) {
        languageManager = newLanguageManager;
        // Localización de la escena
        LocalizateScene(sceneName);
    }

    void UpdateTextComponents(string category) {
        // Obtiene las keys de la escena
        List<string> keys = languageManager.GetKeysWithinCategory(category);
        // Actualiza los componentes Text con key
        foreach (string key in keys) {
            string path = key.Substring(category.Length + 1).Replace('.', '/');
            GameObject.Find(path).GetComponent<Text>().text = languageManager.GetTextValue(key);
        }
    }
}