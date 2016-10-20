using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class HUDManager : MonoBehaviour {
    // Tamaño del HUD del personaje activo
    public float modificationSelectedCharacter;
    public float modificationActiveAbility;
    // Transparencia del HUD
    public float transparency = 0.25f;
    // Referencias a los personajes
    [Header("Characters")]
    public GameObject aoi;
    public GameObject akai;
    public GameObject ki;
    // Referencias a los elementos del HUD
    [Header("HUD Elements")]
    public RectTransform selectedCharacter;
    public RectTransform unselectedCharacters;
    public RectTransform aoiCharacterUI;
    public RectTransform bigJumpAbilityUI;
    public RectTransform sprintAbilityUI;
    public RectTransform akaiCharacterUI;
    public RectTransform breakAbilityUI;
    public RectTransform pushAbilityUI;
    public RectTransform kiCharacterUI;
    public RectTransform telekinesisAbilityUI;
    public RectTransform astralProjectionAbilityUI;
    public RectTransform sakeUI;
    public RectTransform timerUI;

    private Text timerText;
    private bool timerActive = false;

    private CharacterAbility selectedAbility;
    private GameObject activeCharacter;

    public GameObject cooldownNotification;

    public static HUDManager instance;
    //Use this for references
    void Awake() {
        // Oculta la interfaz relativa a los jugadores deshabilitados
        if (aoi == null) {
            Destroy(aoiCharacterUI.parent.gameObject);
        }
        if (akai == null) {
            Destroy(akaiCharacterUI.parent.gameObject);
        }
        if (ki == null) {
            Destroy(kiCharacterUI.parent.gameObject);
        }
        timerText = timerUI.GetComponentInChildren<Text>();
        instance = this;
    }

    // Use this for initialization
    void Start() {
        // Inicialización UI
        TransparencyInitialization();
        // Interfaz del jugador seleccionado
        activeCharacter = CharacterManager.GetActiveCharacter();
        RectTransform trf = GetCharacterUIRectTransform(activeCharacter);
        trf.parent.SetParent(selectedCharacter);
        RectTransform parentRectTrf = trf.parent.GetComponent<RectTransform>();
        parentRectTrf.anchoredPosition = new Vector2(0, 0);
        parentRectTrf.sizeDelta = new Vector2(315, 210);
        foreach (RectTransform t1 in trf.parent) {
            foreach (RectTransform t2 in t1) {
                t2.sizeDelta *= (1 + modificationSelectedCharacter);
            }
        }
        CharacterSelection(activeCharacter, true, 1);
        //selectedAbility = activeCharacter.GetComponent<AbilityController>().GetActiveAbility();
        //AbilitySelection(selectedAbility.GetType(), 1);
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent += CharacterChanged;
        //AbilityController.SelectedAbilityEvent += AbilitySelected;
        CharacterAbility.ModifiedAbilityEnergyEvent += EnergyModified;
        CharacterAbility.ActivateAbilityEvent += AbilityExecutionStart;
        CharacterAbility.DeactivateAbilityEvent += AbilityExecutionEnd;
        CharacterInventory.ObjectAddedEvent += ObjectAdded;
        CharacterInventory.ObjectRemovedEvent += ObjectRemoved;
        CharacterInventory.ObjectRequestedEvent += ObjectRequested;
        CharacterStatus.KillCharacterEvent += CharacterKilled;
        CharacterStatus.ResurrectCharacterEvent += CharacterResurrected;
    }

    // Se ejecuta cuando se termina la ejecución de una habilidad
    void AbilityExecutionEnd(CharacterAbility ability) {
        // Habilidad afectada
        RectTransform abilityUI = GetAbilityUIRectTransform(ability.GetType());
        // Reduce tamaño del icono
        foreach (RectTransform trf in abilityUI) {
            trf.sizeDelta /= (1 + modificationActiveAbility);
        }
    }

    // Se ejecuta cuando comienza la ejecución de una habilidad
    void AbilityExecutionStart(CharacterAbility ability) {
        // Habilidad afectada
        RectTransform abilityUI = GetAbilityUIRectTransform(ability.GetType());
        // Aumenta tamaño del icono
        foreach (RectTransform trf in abilityUI) {
            trf.sizeDelta *= (1 + modificationActiveAbility);
        }
    }

    // Se ejecuta cuando se selecciona una habilidad
    void AbilitySelected(CharacterAbility ability) {
        // Deselección de habilidad anterior
        AbilitySelection(selectedAbility.GetType(), transparency);
        // Selección de nueva habilidad
        selectedAbility = ability;
        AbilitySelection(selectedAbility.GetType(), 1);
    }

    // Modifica la transparencia de la habilidad determinada
    void AbilitySelection(System.Type abilityType, float alphaSelection) {
        // Selecciona la interfaz relativa a la habilidad y modifica su apariencia
        RectTransform abilityUI = GetAbilityUIRectTransform(abilityType);
        abilityUI.Find("Full").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
        abilityUI.Find("Empty").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
    }

    // Modifica la transparencia de la habilidad determinada
    void AbilitySelection(string abilityType, float alphaSelection) {
        // Selecciona la interfaz relativa a la habilidad y modifica su apariencia
        RectTransform abilityUI = GetAbilityUIRectTransform(abilityType);
        abilityUI.Find("Full").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
        abilityUI.Find("Empty").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
    }

    // Se ejecuta cuando se produce un cambio de personaje
    void CharacterChanged(GameObject character) {
        // Modifica la interfaz del personaje seleccionado
        GameObject newActiveCharacter = character;
        CharacterSelected(newActiveCharacter);
        //AbilitySelected(newActiveCharacter.GetComponent<AbilityController>().GetActiveAbility());
        UpdateInventory();
    }

    // Se ejecuta cuando se produce la muerte de un personaje
    void CharacterKilled(GameObject character) {
        CharacterSelection(character, true, 0);
        CharacterSelection(character, false, 2 * transparency);
    }

    // Se ejecuta cuando un personaje resucita
    void CharacterResurrected(GameObject character) {
        // Comprueba si el jugador resucitado es el seleccionado por el jugador
        if (character.Equals(CharacterManager.GetActiveCharacter())) {
            CharacterSelected(character);
        } else {
            // Actualiza el icono del jugador, dejándolo desactivado
            CharacterSelection(character, false, transparency);
            CharacterSelection(character, true, transparency);
        }
    }

    // Modifica la transparencia del personaje determinado
    void CharacterSelection(GameObject character, bool alive, float alphaSelection) {
        // Selecciona la interfaz relativa al personaje y modifica su apariencia
        Transform characterUI = GetCharacterUIRectTransform(character);
        if (alive) {
            characterUI.Find("Full").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
        } else {
            characterUI.Find("Empty").GetComponent<CanvasRenderer>().SetAlpha(alphaSelection);
        }
        // Selecciona la interfaz relativa a las habilidades del personaje y modifica su apariencia
        switch (character.GetComponent<CharacterStatus>().characterName) {
            case (CharacterName.Aoi):
                AbilitySelection("BigJumpAbility", alphaSelection);
                AbilitySelection("SprintAbility", alphaSelection);
                break;
            case (CharacterName.Akai):
                AbilitySelection("BreakAbility", alphaSelection);
                AbilitySelection("PushAbility", alphaSelection);
                break;
            case (CharacterName.Ki):
                AbilitySelection("TelekinesisAbility", alphaSelection);
                AbilitySelection("AstralProjectionAbility", alphaSelection);
                break;
        }
    }

    // Se ejecuta cuando se produce un cambio de personaje
    void CharacterSelected(GameObject character) {
        // Se muestra el icono correspondiente a la vida o no del personaje
        if (activeCharacter.GetComponent<CharacterStatus>().IsAlive()) {
            CharacterSelection(activeCharacter, true, transparency);
        }
        // Se modifica la interfaz del antiguo personaje seleccionado
        RectTransform trf = GetCharacterUIRectTransform(activeCharacter);
        foreach (RectTransform t1 in trf.parent) {
            foreach (RectTransform t2 in t1) {
                t2.sizeDelta /= (1 + modificationSelectedCharacter);
            }
        }
        trf.parent.GetComponent<RectTransform>().sizeDelta /= (1 + modificationSelectedCharacter);
        SetCharacterAsUnselected(activeCharacter, trf, character);
        // Se modifica la interfaz del personaje actualmente seleccionado
        activeCharacter = character;
        trf = GetCharacterUIRectTransform(activeCharacter);
        trf.parent.SetParent(selectedCharacter);
        trf.parent.GetComponent<RectTransform>().sizeDelta *= (1 + modificationSelectedCharacter);
        foreach (RectTransform t1 in trf.parent) {
            foreach (RectTransform t2 in t1) {
                t2.sizeDelta *= (1 + modificationSelectedCharacter);
            }
        }
        trf.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        CharacterSelection(activeCharacter, true, 1);
    }

    // Se ejecuta cuando se produce un cambio en la cantidad de energía de la habilidad determinada
    void EnergyModified(CharacterAbility ability) {
        // Selecciona la interfaz relativa a la habilidad y modifica el relleno de la imagen
        RectTransform abilityUI = GetAbilityUIRectTransform(ability.GetType());
        float amount = ability.GetAvailableEnergy() / ability.GetMaxEnergy();
        GameObject full = abilityUI.Find("Full").gameObject;
        full.GetComponent<Image>().fillAmount = amount;

        if (amount >= 1) {
            //Comprueba si se trata del jugador activo
            if (abilityUI.GetComponentInChildren<CanvasRenderer>().GetAlpha().Equals(1)) {

                //Lanzar Imagen segun habilidad
                cooldownNotification.GetComponent<CooldownNotification>().ShowNotification(full.GetComponent<Image>().sprite);
            }
        }
    }

    public static void FastTimer(float time) {
        instance.timerUI.GetComponentInChildren<Animator>().speed *= 3;
        iTween.ShakeRotation(instance.timerUI.gameObject, new Vector3(0, 0, 10), time);
        iTween.ShakeScale(instance.timerUI.gameObject, new Vector3(0.1f, 0.1f, 0), time);
    }

    // Devuelve el componente RectTransform de la habilidad indicada
    RectTransform GetAbilityCooldownRectTransform(System.Type abilityType) {
        RectTransform abilityUI = null;
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
            case "AstralProjectionAbility":
                abilityUI = astralProjectionAbilityUI;
                break;
        }

        return abilityUI;
    }

    // Devuelve el componente RectTransform de la habilidad indicada
    RectTransform GetAbilityUIRectTransform(string abilityType) {
        RectTransform abilityUI = null;
        switch (abilityType) {
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
            case "AstralProjectionAbility":
                abilityUI = astralProjectionAbilityUI;
                break;
        }

        return abilityUI;
    }

    // Devuelve el componente RectTransform de la habilidad indicada
    RectTransform GetAbilityUIRectTransform(System.Type abilityType) {
        RectTransform abilityUI = null;
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
            case "AstralProjectionAbility":
                abilityUI = astralProjectionAbilityUI;
                break;
        }

        return abilityUI;
    }

    // Devuelve el componente RectTransform del personaje indicado
    RectTransform GetCharacterUIRectTransform(GameObject character) {
        RectTransform characterUI = null;
        if (character.Equals(aoi)) {
            characterUI = aoiCharacterUI;
        } else if (character.Equals(akai)) {
            characterUI = akaiCharacterUI;
        } else if (character.Equals(ki)) {
            characterUI = kiCharacterUI;
        }

        return characterUI;
    }

    // Devuelve el componente RectTransform del objeto indicado
    RectTransform GetInventoryObjectUITransform(string obj) {
        RectTransform res = null;
        switch (obj) {
            case "SakeBottle":
                res = sakeUI;
                break;
        }

        return res;
    }

    // Se ejecuta cuando se añade un objeto al inventario 
    void ObjectAdded(string obj) {
        switch (obj) {
            case "SakeBottle":
                ShowInventoryObject(true, "SakeBottle");
                break;
        }
    }

    // Se ejecuta cuando se usa un objeto del inventario
    void ObjectRemoved(string obj) {
        switch (obj) {
            case "SakeBottle":
                ShowInventoryObject(false, "SakeBottle");
                break;
        }
    }

    // Se ejecuta cuando se le solicita al personaje un objeto de su inventario
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

    void OnDisable() {
        // Elimina suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent -= CharacterChanged;
        //AbilityController.SelectedAbilityEvent -= AbilitySelected;
        CharacterAbility.ModifiedAbilityEnergyEvent -= EnergyModified;
        CharacterAbility.ActivateAbilityEvent -= AbilityExecutionStart;
        CharacterAbility.DeactivateAbilityEvent -= AbilityExecutionEnd;
        CharacterInventory.ObjectAddedEvent -= ObjectAdded;
        CharacterInventory.ObjectRemovedEvent -= ObjectRemoved;
        CharacterInventory.ObjectRequestedEvent -= ObjectRequested;
        CharacterStatus.KillCharacterEvent -= CharacterKilled;
        CharacterStatus.ResurrectCharacterEvent -= CharacterResurrected;
    }

    void OnEnable() {
        // Comprueba si las variables están inicializadas para poder suscribirse a los eventos
        if ((activeCharacter != null) && (selectedAbility != null)) {
            // Suscripciones a eventos
            CharacterManager.ActiveCharacterChangedEvent += CharacterChanged;
            //AbilityController.SelectedAbilityEvent += AbilitySelected;
            CharacterAbility.ModifiedAbilityEnergyEvent += EnergyModified;
            CharacterAbility.ActivateAbilityEvent += AbilityExecutionStart;
            CharacterAbility.DeactivateAbilityEvent += AbilityExecutionEnd;
            CharacterInventory.ObjectAddedEvent += ObjectAdded;
            CharacterInventory.ObjectRemovedEvent += ObjectRemoved;
            CharacterInventory.ObjectRequestedEvent += ObjectRequested;
            CharacterStatus.KillCharacterEvent += CharacterKilled;
            CharacterStatus.ResurrectCharacterEvent += CharacterResurrected;
        }
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

    // Coloca el icono del anterior personaje seleccionado en su posición correcta dentro del listado de personajes no seleccionados
    private void SetCharacterAsUnselected(GameObject activeCharacter, RectTransform trfAC, GameObject newActiveCharacter) {
        // Añade el icono al listado
        trfAC.parent.SetParent(unselectedCharacters);
        // Lo coloca en el orden correcto
        switch (newActiveCharacter.GetComponent<CharacterStatus>().characterName) {
            case CharacterName.Aoi:
                if (activeCharacter.GetComponent<CharacterStatus>().characterName.Equals(CharacterName.Akai)) {
                    trfAC.parent.SetAsFirstSibling();
                } else {
                    trfAC.parent.SetAsLastSibling();
                }
                break;
            case CharacterName.Akai:
                if (activeCharacter.GetComponent<CharacterStatus>().characterName.Equals(CharacterName.Ki)) {
                    trfAC.parent.SetAsFirstSibling();
                } else {
                    trfAC.parent.SetAsLastSibling();
                }
                break;
            case CharacterName.Ki:
                if (activeCharacter.GetComponent<CharacterStatus>().characterName.Equals(CharacterName.Aoi)) {
                    trfAC.parent.SetAsFirstSibling();
                } else {
                    trfAC.parent.SetAsLastSibling();
                }
                break;
        }
    }

    // Inicialización de la transparencia de los distintos elementos del HUD
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
        if (ki != null) {
            CharacterSelection(ki, true, transparency);
            CharacterSelection(ki, false, transparency);
            AbilitySelection(typeof(TelekinesisAbility), transparency);
            AbilitySelection(typeof(AstralProjectionAbility), transparency);
        }
        // InventoryUI
        ShowInventoryObject(false, "SakeBottle");
        ShowEmptyInventoryObject(false, "SakeBottle");
    }

    // Actualiza el inventario del jugador seleccionado
    void UpdateInventory() {
        ShowEmptyInventoryObject(false, "SakeBottle");
        if (activeCharacter.GetComponent<CharacterInventory>().HasObject("SakeBottle")) {
            ShowInventoryObject(true, "SakeBottle");
        } else {
            ShowInventoryObject(false, "SakeBottle");
        }
    }

    public static void UpdateTimer(float value) {
        //if (instance.timerImage.enabled) {
        //    instance.timerImage.fillAmount = value;
        //}
        instance.timerText.text = Mathf.CeilToInt(value).ToString();
    }

    public static void StartTimer() {
        //instance.timerImage.enabled = true;
        //instance.timerImage.fillAmount = 1;
        instance.timerUI.gameObject.SetActive(true);
        instance.timerActive = true;

    }

    public static void StopTimer() {
        //instance.timerImage.enabled = false;
        instance.timerUI.gameObject.SetActive(false);
        instance.timerUI.GetComponentInChildren<Animator>().speed /= 3;
        iTween.Stop(instance.timerUI.gameObject);
        instance.timerActive = false;
    }
    public static bool TimerActive() {
        return instance.timerActive;
    }

    public static void  SetPuzzleName(string str) {
        instance.transform.Find("HUDCanvas/PuzzleName").GetComponent<Text>().text = str;
    }
}
