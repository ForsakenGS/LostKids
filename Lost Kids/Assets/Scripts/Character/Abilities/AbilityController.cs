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
        // Activa la habilidad 1
        activeAbility = ability1;
        ability1.ActivateAbility();
        // Lanza el evento de habilidad seleccionada
        if (SelectedAbilityEvent != null) {
            SelectedAbilityEvent(ability1);
        }
    }

    /// <summary>
    /// Cambia la habilidad del personaje que está seleccionada
    /// </summary>
    public void ChangeAbility() {
		// Intenta desactivar la habilidad actual
		if (activeAbility.DeactivateAbility ()) {
			// Elige la habilidad a activar y procede con su activación
			if (activeAbility.Equals (ability1)) {
				activeAbility = ability2;
			} else {
				activeAbility = ability1;
			}
			activeAbility.ActivateAbility();
			// Lanza el evento de habilidad seleccionada
			if (SelectedAbilityEvent != null) {
				SelectedAbilityEvent(activeAbility);
			}
		}
    }

    /// <summary>
    /// Permite conocer la habilidad activa del personaje
    /// </summary>
    /// <returns>El componente <c>CharacterAbility</c> referente a la habilidad activa</returns>
    public CharacterAbility GetActiveAbility() {
        return activeAbility;
    }

    /// <summary>
    /// Usa o deja de usar la habilidad del personaje que está seleccionada
    /// </summary>
    public void UseAbility() {
        if (!activeAbility.IsExecuting()) {
            // Empieza a usar si es posible
            if ((activeAbility.CanBeStarted()) && (characterStatus.CanStartAbility(activeAbility))) {
                activeAbility.StartExecution();
            }
        } else {
            // Deja de usar
            if (activeAbility.EndExecution()) {
                characterStatus.EndAbility(activeAbility);
            }
        }
    }
}
