using UnityEngine;
using UnityEngine.SceneManagement;
    using InControl;

public class SkipVideoManager : MonoBehaviour {
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
