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
        // Lanza el evento de habilidad seleccionada
        //if (SelectedAbilityEvent != null) {
        //SelectedAbilityEvent(ability1);
        //}
    }

    bool ActivateAbility(CharacterAbility ability) {
        bool res = (ability.CanBeStarted()) && (characterStatus.CanStartAbility(ability));
        if (res) {
            res = ability.StartExecution();
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
        } else {
            // Se desactiva la habilidad 2, que está activa
            if (DeactivateAbility(ability2)) {
                // Se activa la habilidad 1 porque se ha podido desactivar la habilidad 2
                ActivateAbility(ability1);
            }
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
        } else {
            // Se desactiva la habilidad 1, que está activa
            if (DeactivateAbility(ability1)) {
                // Se activa la habilidad 2 porque se ha podido desactivar la habilidad 1
                ActivateAbility(ability2);
            }
        }
    }

    bool DeactivateAbility(CharacterAbility ability) {
        bool res = activeAbility.EndExecution();
        if (res) {
            characterStatus.EndAbility(activeAbility);
            activeAbility = null;
        }

        return res;
    }

    public bool DeactivateActiveAbility() {
        return DeactivateAbility(activeAbility);
    }

    ///// <summary>
    ///// Usa o deja de usar la habilidad del personaje indicada
    ///// </summary>
    //void UseAbility(CharacterAbility ability) {

    //    if (!activeAbility.IsExecuting()) {
    //        // Empieza a usar si es posible
    //        if ((activeAbility.CanBeStarted()) && (characterStatus.CanStartAbility(activeAbility))) {
    //            activeAbility.StartExecution();
    //        }
    //    } else {
    //        // Deja de usar
    //        if (activeAbility.EndExecution()) {
    //            characterStatus.EndAbility(activeAbility);
    //        }
    //    }
    //}

    ///// <summary>
    ///// Cambia la habilidad del personaje que está seleccionada
    ///// </summary>
    //public void ChangeAbility() {
    //    // Intenta desactivar la habilidad actual
    //    if (activeAbility.DeactivateAbility()) {
    //        // Elige la habilidad a activar y procede con su activación
    //        if (activeAbility.Equals(ability1)) {
    //            activeAbility = ability2;
    //        } else {
    //            activeAbility = ability1;
    //        }
    //        activeAbility.ActivateAbility();
    //        // Lanza el evento de habilidad seleccionada
    //        if (SelectedAbilityEvent != null) {
    //            SelectedAbilityEvent(activeAbility);
    //        }
    //    }
    //}

    /// <summary>
    /// Permite conocer la habilidad activa del personaje
    /// </summary>
    /// <returns>El componente <c>CharacterAbility</c> referente a la habilidad activa</returns>
    public CharacterAbility GetActiveAbility() {
        return activeAbility;
    }

    ///// <summary>
    ///// Usa o deja de usar la habilidad del personaje que está seleccionada
    ///// </summary>
    //public void UseAbility() {

    //}
}
