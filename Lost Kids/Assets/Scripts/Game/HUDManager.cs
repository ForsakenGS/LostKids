using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public GameObject aoi;
    public GameObject akai;
    public GameObject murasaki;
    public float transparency = 0.5f;

    private GameObject aoiUI;
    private GameObject akaiUI;
    private GameObject murasakiUI;
    private GameObject activeCharacterUI;
    private CharacterManager characterManager;

    //Use this for references
    void Awake() {
        aoiUI = GameObject.Find("1AoiUI");
        akaiUI = GameObject.Find("2AkaiUI");
        murasakiUI = GameObject.Find("3MurasakiUI");
    }

    // Use this for initialization
    void Start() {
        // Oculta la interfaz relativa a los jugadores deshabilitados
        if (aoi == null) {
            DestroyImmediate(aoiUI);
        }
        if (akai == null) {
            DestroyImmediate(akaiUI);
        }
        if (murasaki == null) {
            DestroyImmediate(murasakiUI);
        }
        // Inicialización UI
        UIStart();
        CharacterChanged();
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent += CharacterChanged;
        AbilityController.ActiveAbilityChangedEvent += AbilityChanged;
    }

    void AbilityChanged() {
        if (activeCharacterUI.transform.Find("Ability1Full").GetComponent<CanvasRenderer>().GetAlpha() == 1) {
            // Habilidad 2 activa desde ahora
            activeCharacterUI.transform.Find("Ability1Full").GetComponent<CanvasRenderer>().SetAlpha(transparency);
            activeCharacterUI.transform.Find("Ability1Empty").GetComponent<CanvasRenderer>().SetAlpha(transparency);
            activeCharacterUI.transform.Find("Ability2Full").GetComponent<CanvasRenderer>().SetAlpha(1);
            activeCharacterUI.transform.Find("Ability2Empty").GetComponent<CanvasRenderer>().SetAlpha(1);
        } else {
            // Habilidad 1 activa desde ahora
            activeCharacterUI.transform.Find("Ability2Full").GetComponent<CanvasRenderer>().SetAlpha(transparency);
            activeCharacterUI.transform.Find("Ability2Empty").GetComponent<CanvasRenderer>().SetAlpha(transparency);
            activeCharacterUI.transform.Find("Ability1Full").GetComponent<CanvasRenderer>().SetAlpha(1);
            activeCharacterUI.transform.Find("Ability1Empty").GetComponent<CanvasRenderer>().SetAlpha(1);
        }
    }

    void CharacterChanged() {
        GameObject newActiveCharacter = CharacterManager.GetActiveCharacter();
        int activeAbilityIndex = newActiveCharacter.GetComponent<AbilityController>().GetActiveAbilityIndex();
        if (activeCharacterUI != null) {
            // Establece transparencias del anterior personaje seleccionado
            activeCharacterUI.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(transparency);
            if (activeCharacterUI.transform.Find("Ability1Full").GetComponent<CanvasRenderer>().GetAlpha() == 1) {
                // Habilidad 1 activa
                activeCharacterUI.transform.Find("Ability1Full").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                activeCharacterUI.transform.Find("Ability1Empty").GetComponent<CanvasRenderer>().SetAlpha(transparency);
            } else {
                // Habilidad 2 activa
                activeCharacterUI.transform.Find("Ability2Full").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                activeCharacterUI.transform.Find("Ability2Empty").GetComponent<CanvasRenderer>().SetAlpha(transparency);
            }
        }
        if (newActiveCharacter.Equals(aoi)) {
            activeCharacterUI = aoiUI;
        } else if (newActiveCharacter.Equals(akai)) {
            activeCharacterUI = akaiUI;
        } else {
            activeCharacterUI = murasakiUI;
        }
        activeCharacterUI.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(1);
        if (activeAbilityIndex == 1) {
            // Habilidad 1 activa
            activeCharacterUI.transform.Find("Ability1Full").GetComponent<CanvasRenderer>().SetAlpha(1);
            activeCharacterUI.transform.Find("Ability1Empty").GetComponent<CanvasRenderer>().SetAlpha(1);
        } else {
            // Habilidad 2 activa
            activeCharacterUI.transform.Find("Ability2Full").GetComponent<CanvasRenderer>().SetAlpha(1);
            activeCharacterUI.transform.Find("Ability2Empty").GetComponent<CanvasRenderer>().SetAlpha(1);
        }
    }

    void UIStart() {
        for (int i = 0; i < 3; ++i) {
            // Elige la UI del personaje
            GameObject ui = aoiUI;
            switch (i) {
                case 1:
                    ui = akaiUI;
                    break;
                case 2:
                    ui = murasakiUI;
                    break;
            }
            // Establece las imágenes semi-transparentes
            if (ui != null) {
                ui.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                ui.transform.Find("CharacterUnselected").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                ui.transform.Find("Ability1Full").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                ui.transform.Find("Ability1Empty").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                ui.transform.Find("Ability2Full").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                ui.transform.Find("Ability2Empty").GetComponent<CanvasRenderer>().SetAlpha(transparency);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (aoi != null) {
            // Carga/Recarga habilidades de Aoi
            float amount = aoi.GetComponent<BigJumpAbility>().GetAvailableEnergy() / aoi.GetComponent<BigJumpAbility>().GetMaxEnergy();
            aoiUI.transform.Find("Ability1Full").GetComponent<Image>().fillAmount = amount;
            amount = aoi.GetComponent<SprintAbility>().GetAvailableEnergy() / aoi.GetComponent<SprintAbility>().GetMaxEnergy();
            aoiUI.transform.Find("Ability2Full").GetComponent<Image>().fillAmount = amount;
            // Salud Aoi
            if (aoi.GetComponent<CharacterStatus>().IsAlive()) {
                if (activeCharacterUI.Equals(aoiUI)) {
                    aoiUI.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(1);
                } else {
                    aoiUI.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                }
            } else {
                aoiUI.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(0);
            }
        }
        if (akai != null) {
            // Carga/Recarga habilidades de Akai
            float amount = akai.GetComponent<BreakAbility>().GetAvailableEnergy() / akai.GetComponent<BreakAbility>().GetMaxEnergy();
            akaiUI.transform.Find("Ability1Full").GetComponent<Image>().fillAmount = amount;
            amount = akai.GetComponent<PushAbility>().GetAvailableEnergy() / akai.GetComponent<PushAbility>().GetMaxEnergy();
            akaiUI.transform.Find("Ability2Full").GetComponent<Image>().fillAmount = amount;
            // Salud Akai
            if (akai.GetComponent<CharacterStatus>().IsAlive()) {
                if (activeCharacterUI.Equals(akaiUI)) {
                    akaiUI.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(1);
                } else {
                    akaiUI.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(transparency);
                }
            } else {
                akaiUI.transform.Find("CharacterSelected").GetComponent<CanvasRenderer>().SetAlpha(0);
            }
        }
        if (murasaki != null) {
            // Carga/Recarga habilidades de Murasaki    
            //float amount = murasaki.GetComponent<TelekinesisAbility>().GetAvailableEnergy() / murasaki.GetComponent<TelekinesisAbility>().GetMaxEnergy();
            //murasakiUI.transform.Find("Ability1Full").GetComponent<Image>().fillAmount = amount;
            //amount = murasaki.GetComponent<TeletransportAbility>().GetAvailableEnergy() / murasaki.GetComponent<TeletransportAbility>().GetMaxEnergy();
            //murasakiUI.transform.Find("Ability2Full").GetComponent<Image>().fillAmount = amount;
        }
    }
}
