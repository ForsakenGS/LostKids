using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour {
    private CameraManager cameraManager;
    private GameObject character;
    private Image tooltipImage;

    // Use this for references
    void Awake() {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        character = transform.parent.gameObject;
        tooltipImage = GetComponent<Canvas>().GetComponent<Image>();
    }

    public void ActiveCanvas(bool active) {
        GetComponent<Image>().enabled = active;
    }

    void CharacterChanged(GameObject activeCharacter) {
        // Comprueba si no es el personaje activo
        if (!character.Equals(activeCharacter)) {
            ActiveCanvas(false);
        }
    }

    void LateUpdate() {
        transform.LookAt(cameraManager.CurrentCamera().transform);
    }

    void OnDisable() {
        // Suscripción a eventos
        CharacterManager.ActiveCharacterChangedEvent -= CharacterChanged;
    }

    void OnEnable() {
        // Suscripción a eventos
        CharacterManager.ActiveCharacterChangedEvent += CharacterChanged;
    }

    public void SetImage(Sprite image) {
        tooltipImage.sprite = image;
    }
}
