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
    /// <summary>
    /// Evento para informar del cambio de habilidad activa a otros scripts
    /// </summary>
    public static event AbilityChanged SelectedAbilityEvent;

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
        bool res = (ability.CanBeStarted()) && (characterStatus.CanStartAbility(ability));
        if (res) {
            res = ability.ActivateAbility();
            if (res) {
                activeAbility = ability;
            }
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
            DeactivateAbility(ability1);
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
            DeactivateAbility(ability2);
        }
    }

    // Desactiva la habilidad pasada como parámetro
    bool DeactivateAbility(CharacterAbility ability) {
        bool res = (ability != null);
        if (res) {
            res = ability.DeactivateAbility();
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
    public bool DeactivateActiveAbility() {
        return DeactivateAbility(activeAbility);
    }

    /// <summary>
    /// Permite conocer la habilidad activa del personaje
    /// </summary>
    /// <returns>El componente <c>CharacterAbility</c> referente a la habilidad activa</returns>
    public CharacterAbility GetActiveAbility() {
        return activeAbility;
    }

    public void ResetAbilities() {
        ability1.Reset();
        ability2.Reset();
        activeAbility = null;
    }
}