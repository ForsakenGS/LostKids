using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Componente que detecta al jugador en la zona activa de un objeto, permitiendo su uso
/// y mostrando un tooltip sobre el jugador
/// </summary>
public class TooltipDetector : MonoBehaviour {
    // Icono asociado al objeto
    public Sprite tooltipImage;
    // Habilidad requerida para poder usar el objeto
    public bool requireAbility;
    public AbilityName requiredAbilityName;

    // Referencia al controlador del canvas del jugador
    private CharacterIcon icon;
    // Referencia a la habilidad con la que se relaciona el objeto
    private CharacterAbility requiredAbility;
    // Listado de personajes en trigger
    private List<CharacterStatus> charactersList;
    // Referencia al UsableObject relacionado
    private UsableObject usableObj;

    // Use this for references
    void Awake() {
        charactersList = new List<CharacterStatus>();
        usableObj = GetComponentInParent<UsableObject>();
    }

    void ActiveCharacterChanged(GameObject character) {
        if (charactersList.Contains(character.GetComponent<CharacterStatus>())) {
            ShowCharacterTooltip(character);
        }
    }

    // Muestra/oculta el tooltip
    void ChangeTooltipStatus(bool status) {
        if (status) {
            ShowCharacterTooltip(CharacterManager.GetActiveCharacter());
        } else {
            HideCharacterTooltip();
        }
    }

    CharacterAbility GetRequiredAbility(GameObject go) {
        CharacterAbility ability = null;
        if (requireAbility) {
            switch (requiredAbilityName) {
                case AbilityName.AstralProjection:
                    ability = go.GetComponent<AstralProjectionAbility>();
                    break;
                case AbilityName.Telekinesis:
                    ability = go.GetComponent<TelekinesisAbility>();
                    break;
                case AbilityName.Break:
                    ability = go.GetComponent<BreakAbility>();
                    break;
                case AbilityName.Push:
                    ability = go.GetComponent<PushAbility>();
                    break;
                case AbilityName.BigJump:
                    ability = go.GetComponent<BigJumpAbility>();
                    break;
                case AbilityName.Sprint:
                    ability = go.GetComponent<SprintAbility>();
                    break;
            }
        }

        return ability;
    }

    private void HideCharacterTooltip() {
        // Desactiva el tooltip si se está mostrando
        if (icon != null) {
            icon.ActiveCanvas(false);
            icon = null;
        }
        if (requiredAbility != null) {
            // Informa a la habilidad que el objeto ya no se encuentra disponible
            requiredAbility.SetReady(false);
        } else if (usableObj != null) {
            // Deshabilita el objeto usable
            usableObj.canUse = false;
        }

    }

    void OnDestroy() {
        HideCharacterTooltip();
    }

    void OnDisable() {
        // Suscripción a eventos
        CharacterManager.ActiveCharacterChangedEvent -= ActiveCharacterChanged;
        TooltipManager.TooltipOnOff -= ChangeTooltipStatus;
    }

    void OnEnable() {
        // Suscripción a eventos
        CharacterManager.ActiveCharacterChangedEvent += ActiveCharacterChanged;
        TooltipManager.TooltipOnOff += ChangeTooltipStatus;
    }

    void OnTriggerEnter(Collider other) {
        // Si se trata de un personaje, lo añade al listado
        CharacterStatus cs = other.gameObject.GetComponent<CharacterStatus>();
        if (cs != null) {
            charactersList.Add(cs);
            // Asocia la habilidad relacionada con el objeto, si la requiere
            if ((requireAbility) && (requiredAbility == null)) {
                requiredAbility = GetRequiredAbility(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        // Si se trata de un personaje, lo elimina del listado y esconde su tooltip
        CharacterStatus cs = other.gameObject.GetComponent<CharacterStatus>();
        if (cs != null) {
            charactersList.Remove(cs);
            HideCharacterTooltip();
        }
    }

    /// <summary>
    /// Cuando el jugador entra en el detector, si dispone de las habilidades necesarias
    /// muestra el icono correspondiente y permite el uso del objeto
    /// </summary>
    /// <param name="other"></param>
	void OnTriggerStay(Collider other) {
        ShowCharacterTooltip(other.gameObject);
    }

    void ShowCharacterTooltip(GameObject character) {
        if (CharacterManager.IsActiveCharacter(character)) {
            if (TooltipHasToBeShown(character)) {
                if ((TooltipManager.On) && (tooltipImage != null)) {
                    icon = character.GetComponentInChildren<CharacterIcon>();
                    icon.ActiveCanvas(true);
                    icon.SetImage(tooltipImage);
                }
            } else {
                HideCharacterTooltip();
            }
        }
    }

    bool TooltipHasToBeShown(GameObject character) {
        bool res = (!requireAbility);
        if (!res) {
            // Requiere habilidad, luego se comprueba si el tooltip se muestra o no
            if ((requiredAbilityName.Equals(AbilityName.AstralProjection)) || (requiredAbilityName.Equals(AbilityName.Telekinesis))) {
                // Habilidades de Ki
                if (character.GetComponent<CharacterStatus>().characterName.Equals(CharacterName.Ki)) {
                    // Se trata del personaje correcto, Ki
                    res = requiredAbility.SetReady(true, transform.parent.gameObject);
                }
            } else if ((requiredAbilityName.Equals(AbilityName.Break)) || (requiredAbilityName.Equals(AbilityName.Push))) {
                // Habilidades de Akai
                if (character.GetComponent<CharacterStatus>().characterName.Equals(CharacterName.Akai)) {
                    // Se trata del personaje correcto, Akai
                    Ray usingRay = new Ray(character.transform.position + Vector3.up, character.transform.forward);
                    RaycastHit hit;
                    // Comprueba la orientación del personaje
                    if (Physics.Raycast(usingRay, out hit, 2)) {
                        res = requiredAbility.SetReady(true, null, hit);
                    }
                }
            }
        } else {
            // Se trata de un objeto usable por todos los personajes
            Ray usingRay = new Ray(character.transform.position + Vector3.up, character.transform.forward);
            RaycastHit hit;
            // Comprueba la orientación del personaje
            if (Physics.Raycast(usingRay, out hit, 2)) {
                res = ((hit.collider.gameObject.Equals(usableObj.gameObject)) && (!usableObj.onUse));
                if (res) {
                    usableObj.canUse = true;
                }
            } else {
                res = false;
            }
        }

        return res;
    }
}