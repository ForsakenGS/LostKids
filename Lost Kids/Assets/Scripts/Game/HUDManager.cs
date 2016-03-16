using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDManager : MonoBehaviour {
    public GameObject aoi;
    public GameObject akai;
    public GameObject murasaki;
    public float transparency = 0.25f;

    private Transform aoiUI;
    private Transform akaiUI;
    private Transform murasakiUI;
    private Transform bigJumpAbilityUI;
    private Transform sprintAbilityUI;
    private Transform breakAbilityUI;
    private Transform pushAbilityUI;
    private Transform telekinesisAbilityUI;
    private Transform teletransportAbilityUI;
    private Transform sakeUI;
    private CharacterAbility selectedAbility;
    private GameObject selectedCharacter;

    //Use this for references
    void Awake() {
        Transform trfPH = transform.Find("HUDCanvas").Find("PlayerHabilitiesUI");
        // Interfaz personaje Aoi
        aoiUI = trfPH.Find("1AoiUI");
        bigJumpAbilityUI = aoiUI.Find("BigJumpAbility");
        sprintAbilityUI = aoiUI.Find("SprintAbility");
        // Interfaz personaje Akai
        akaiUI = trfPH.Find("2AkaiUI");
        breakAbilityUI = akaiUI.Find("BreakAbility");
        pushAbilityUI = akaiUI.Find("PushAbility");
        // Interfaz personaje Murasaki
        murasakiUI = trfPH.Find("3MurasakiUI");
        telekinesisAbilityUI = murasakiUI.Find("TelekinesisAbility");
        teletransportAbilityUI = murasakiUI.Find("TeletransportAbility");
        // Interfaz del inventario
        Transform trfI = transform.Find("HUDCanvas").Find("InventoryUI");
        sakeUI = trfI.Find("SakeBottle");
    }

    // Use this for initialization
    void Start() {
        // Oculta la interfaz relativa a los jugadores deshabilitados
        if (aoi == null) {
            DestroyImmediate(aoiUI.gameObject);
        }
        if (akai == null) {
            DestroyImmediate(akaiUI.gameObject);
        }
        if (murasaki == null) {
            DestroyImmediate(murasakiUI.gameObject);
        }
        // Inicialización UI
        TransparencyInitialization();
        selectedCharacter = CharacterManager.GetActiveCharacter();
        CharacterSelection(selectedCharacter, true, 1);
        selectedAbility = selectedCharacter.GetComponent<AbilityController>().GetActiveAbility();
        AbilitySelection(selectedAbility.GetType(), 1);
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent += CharacterChanged;
        AbilityController.SelectedAbilityEvent += AbilitySelected;
        CharacterAbility.ModifiedAbilityEnergyEvent += EnergyModified;
        CharacterInventory.ObjectAddedEvent += ObjectAdded;
        CharacterInventory.ObjectRemovedEvent += ObjectRemoved;
        CharacterInventory.ObjectRequestedEvent += ObjectRequested;
        CharacterStatus.KillCharacterEvent += CharacterKilled;
        CharacterStatus.ResurrectCharacterEvent += CharacterResurrected;
    }

    void AbilitySelected(CharacterAbility ability) {
        AbilitySelection(selectedAbility.GetType(), transparency);
        selectedAbility = ability;
        AbilitySelection(selectedAbility.GetType(), 1);
    }

    void AbilitySelection(System.Type abilityType, float alphaSelection) {
        // Selecciona la interfaz relativa a la habilidad y modifica su apariencia
        Transform abilityUI = GetAbilityUITransform(abilityType);
        abilityUI.Find("Full").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
        abilityUI.Find("Empty").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
    }

    void CharacterChanged() {
        // Modifica la interfaz del personaje seleccionado
        GameObject newActiveCharacter = CharacterManager.GetActiveCharacter();
        CharacterSelected(newActiveCharacter);
        AbilitySelected(newActiveCharacter.GetComponent<AbilityController>().GetActiveAbility());
        UpdateInventory();
    }

    void CharacterKilled(GameObject character) {
        CharacterSelection(character, true, 0);
        CharacterSelection(character, false, 2*transparency);
    }

    void CharacterResurrected(GameObject character) {
        CharacterSelection(character, true, transparency);
        CharacterSelection(character, false, transparency);
    }

    void CharacterSelection(GameObject character, bool selected, float alphaSelection) {
        // Selecciona la interfaz relativa a la habilidad y modifica su apariencia
        Transform characterUI = GetCharacterUITransform(character);
        if (selected) {
            characterUI.Find("Selected").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
        } else {
            characterUI.Find("Deselected").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
        }
    }

    void CharacterSelected(GameObject character) {
        CharacterSelection(selectedCharacter, true, transparency);
        selectedCharacter = character;
        CharacterSelection(selectedCharacter, true, 1);
    }

    void EnergyModified(CharacterAbility ability) {
        // Selecciona la interfaz relativa a la habilidad y modifica el relleno de la imagen
        Transform abilityUI = GetAbilityUITransform(ability.GetType());
        float amount = ability.GetAvailableEnergy() / ability.GetMaxEnergy();
        abilityUI.Find("Full").GetComponent<Image>().fillAmount = amount;
    }

    Transform GetAbilityUITransform(System.Type abilityType) {
        Transform abilityUI = null;
        switch (abilityType.ToString()) {
            case "BigJumpAbility":
                abilityUI = bigJumpAbilityUI;
                break;
            case "SprintAbility":
                abilityUI = sprintAbilityUI;
                break;
            case "BreakAbility":
                abilityUI = breakAbilityUI;
                break;
            case "PushAbility":
                abilityUI = pushAbilityUI;
                break;
            case "TelekinesisAbility":
                abilityUI = telekinesisAbilityUI;
                break;
            case "TeletransportAbility":
                abilityUI = teletransportAbilityUI;
                break;
        }

        return abilityUI;
    }

    Transform GetCharacterUITransform(GameObject character) {
        Transform characterUI = null;
        if (character.Equals(aoi)) {
            characterUI = aoiUI;
        } else if (character.Equals(akai)) {
            characterUI = akaiUI;
        } else if (character.Equals(murasaki)) {
            characterUI = murasakiUI;
        }

        return characterUI.Find("Character");
    }

    Transform GetInventoryObjectUITransform(string obj) {
        Transform res = null;
        switch (obj) {
            case "SakeBottle":
                res = sakeUI;
                break;
        }

        return res;
    }

    void ObjectAdded(string obj) {
        switch (obj) {
            case "SakeBottle":
                ShowInventoryObject(true, "SakeBottle");
                break;
        }
    }

    void ObjectRemoved(string obj) {
        switch (obj) {
            case "SakeBottle":
                ShowInventoryObject(false, "SakeBottle");
                break;
        }
    }

    void ObjectRequested(string obj) {
        switch (obj) {
            case "SakeBottle":
                ShowEmptyInventoryObject(true, "SakeBottle");
                StartCoroutine(ObjectRequestedOff(obj, 3.0f));
                break;
        }
    }

    IEnumerator ObjectRequestedOff(string obj, float waitTime) {
        yield return new WaitForSeconds(waitTime);
        sakeUI.Find("Empty").GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    void ShowEmptyInventoryObject(bool show, string obj) {
        float alphaObj = 0;
        if (show) {
            alphaObj = 1;
        }
        GetInventoryObjectUITransform(obj).Find("Empty").GetComponent<CanvasRenderer>().SetAlpha(alphaObj);
    }

    void ShowInventoryObject(bool show, string obj) {
        float alphaObj = 0;
        if (show) {
            alphaObj = 1;
        }
        GetInventoryObjectUITransform(obj).Find("Full").GetComponent<CanvasRenderer>().SetAlpha(alphaObj);
    }

    void TransparencyInitialization() {
        // AoiUI
        if (aoi != null) {
            CharacterSelection(aoi, true, transparency);
            CharacterSelection(aoi, false, transparency);
            AbilitySelection(typeof(BigJumpAbility), transparency);
            AbilitySelection(typeof(SprintAbility), transparency);
        }
        // AkaiUI
        if (akai != null) {
            CharacterSelection(akai, true, transparency);
            CharacterSelection(akai, false, transparency);
            AbilitySelection(typeof(BreakAbility), transparency);
            AbilitySelection(typeof(PushAbility), transparency);
        }
        // MurasakiUI
        if (murasaki != null) {
            CharacterSelection(murasaki, true, transparency);
            CharacterSelection(murasaki, false, transparency);
            //AbilitySelection(typeof(TelekinesisAbility), transparency);
            //AbilitySelection(typeof(TeletransportAbility), transparency);
        }
        // InventoryUI
        ShowInventoryObject(false, "SakeBottle");
        ShowEmptyInventoryObject(false, "SakeBottle");
    }

    void UpdateInventory() {
        ShowEmptyInventoryObject(false, "SakeBottle");
        if (selectedCharacter.GetComponent<CharacterInventory>().HasObject("SakeBottle")) {
            ShowInventoryObject(true, "SakeBottle");
        } else {
            ShowInventoryObject(false, "SakeBottle");
        }
    }
}
