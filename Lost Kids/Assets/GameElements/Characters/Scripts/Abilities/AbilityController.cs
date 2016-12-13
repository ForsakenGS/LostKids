using UnityEngine;
using System.Collections;

/// <summary>
/// Sencillo controlador de habilidades para el personaje, con el que controlar el paso de una habilidad a otra y su uso. Durante 
/// el cambio de habilidad, desactiva la habilidad que estaba seleccionada y activa la otra. Para la acción de usar/dejar de usar, 
/// comprueba si se encuentra en ejecución o no y, además de modificar el propio estado de la habilidad, también hace lo propio 
/// con el personaje.
/// </summary>
public class AbilityController : MonoBehaviour {
    public delegate void AbilityChanged(CharacterAbility abilityaffected);

    private CharacterAbility ability1;
    private CharacterAbility ability2;
    private CharacterAbility activeAbility;
    private CharacterStatus characterStatus;

    // Use this for references
    void Awake() {
        CharacterAbility[] abilities = GetComponents<CharacterAbility>();
        ability1 = abilities[0];
        ability2 = abilities[1];
        characterStatus = GetComponent<CharacterStatus>();
    }

    // Use this for initialization
    void Start() {
        // Inicialización
        activeAbility = null;
    }

    // Activa la habilidad pasada como parámetro
    bool ActivateAbility(CharacterAbility ability) {
        // Comprueba si la habilidad puede activarse
        bool res = ability.CanBeStarted();
        if (res) {
            res = characterStatus.StartAbility(ability);
            if (res) {
                activeAbility = ability;
            }
        } else {
            characterStatus.NegationAnimation();
        }

        return res;
    }

    /// <summary>
    /// Activa o desactiva la habilidad nº 1 del personaje
    /// </summary>
    public void ActivateAbility1() {
        if (activeAbility == null) {
            // No hay habilidad activa, luego se activa la habilidad 1
            ActivateAbility(ability1);
        } else if (activeAbility.Equals(ability1)) {
            // Se desactiva la habilidad 1
            DeactivateAbility(ability1, false);
        }
    }

    /// <summary>
    /// Activa o desactiva la habilidad nº 2 del personaje
    /// </summary>
    public void ActivateAbility2() {
        if (activeAbility == null) {
            // No hay habilidad activa, luego se activa la habilidad 2
            ActivateAbility(ability2);
        } else if (activeAbility.Equals(ability2)) {
            // Se desactiva la habilidad 2
            DeactivateAbility(ability2, false);
        }
    }

    // Desactiva la habilidad pasada como parámetro
    bool DeactivateAbility(CharacterAbility ability, bool force) {
        bool res = (ability != null);
        if (res) {
            res = ability.DeactivateAbility(force);
            if (res) {
                characterStatus.EndAbility(ability);
                if (ability.Equals(activeAbility)) {
                    activeAbility = null;
                }
            }
        }

        return res;
    }

    /// <summary>
    /// Función para desactivar la habilidad activa en el momento de la llamada
    /// </summary>
    /// <returns></returns>
    public bool DeactivateActiveAbility(bool force) {
        return DeactivateAbility(activeAbility, force);
    }

    /// <summary>
    /// Permite conocer la habilidad activa del personaje
    /// </summary>
    /// <returns>El componente <c>CharacterAbility</c> referente a la habilidad activa</returns>
    public CharacterAbility GetActiveAbility() {
        return activeAbility;
    }

    void OnDisable() {
        CutSceneManager.CutSceneActivation -= StopAbilitiesTime;
        CutSceneManager.CutSceneDeactivation -= ResumeAbilitiesTime;
    }

    void OnEnable() {
        CutSceneManager.CutSceneActivation += StopAbilitiesTime;
        CutSceneManager.CutSceneDeactivation += ResumeAbilitiesTime;
    }

    /// <summary>
    /// Resetea el estado de las dos habilidades del personaje
    /// </summary>
    public void ResetAbilities() {
        ability1.Reset();
        ability2.Reset();
        activeAbility = null;
    }

    void ResumeAbilitiesTime() {
        ability1.ResumeTime();
        ability2.ResumeTime();
    }

    void StopAbilitiesTime() {
        ability1.StopTime();
        ability2.StopTime();
    }
}