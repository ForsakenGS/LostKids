using UnityEngine;
using System.Collections;

/// <summary>
/// Sencillo controlador de habilidades para el personaje, con el que controlar el paso de una habilidad a otra y su uso. Durante 
/// el cambio de habilidad, desactiva la habilidad que estaba seleccionada y activa la otra. Para la acción de usar/dejar de usar, 
/// comprueba si se encuentra en ejecución o no y, además de modificar el propio estado de la habilidad, también hace lo propio 
/// con el personaje.
/// </summary>
public class AbilityController : MonoBehaviour {
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
        activeAbility = ability1;
        ability1.ActivateAbility();
    }

    /// <summary>
    /// Cambia la habilidad del personaje que está seleccionada
    /// </summary>
    public void ChangeAbility() {
        activeAbility.DeactivateAbility();
        if (activeAbility.Equals(ability1)) {
            activeAbility = ability2;
        } else {
            activeAbility = ability1;
        }
        activeAbility.ActivateAbility();
        Debug.Log("ActiveAbility:" + activeAbility.GetType());
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
