using UnityEngine;
using UnityEngine.SceneManagement;
    using InControl;

public class SkipVideoManager : MonoBehaviour {
    public GameObject videoSP;
    public GameObject videoEN;

    // Use his for references
    void Awake() {
        // Elimina el vídeo del idioma que no se haya seleccionado
        if (LocalizationManager.language.Equals(LocalizationManager.ESLanguage)) {
            videoEN.SetActive(false);
            Destroy(videoEN);
        } else {
            videoSP.SetActive(false);
            Destroy(videoSP);
        }
    }

	// Update is called once per frame
	void Update () {
        // Comprueba si se pulsa algún botón del mando
        bool pressed = InputManager.ActiveDevice.AnyButton.IsPressed;
        if (!pressed) {
            // Comprueba si se pulsa el teclado
            pressed = Input.anyKey;
        }
        if (pressed) {
            // Se termina el vídeo y se cambia de escena
            SceneManager.LoadScene("Intro");
        }
    }
}
