using UnityEngine;
using System.Collections;

public class AbilityController : MonoBehaviour {
	private CharacterAbility ability1;
	private CharacterAbility ability2;
	private CharacterAbility activeAbility;
	private CharacterStatus characterStatus;

	void Awake() {
		CharacterAbility[] abilities = GetComponents<CharacterAbility>();
		ability1 = abilities[0];
		ability2 = abilities[1];
		characterStatus = GetComponent<CharacterStatus>();
	}

	// Use this for initialization
	void Start () {
		activeAbility = ability1;
		ability1.ActivateAbility();
	}

	/// <summary>
	/// Cambia la habilidad del personaje seleccionada
	/// </summary>
	public void ChangeAbility () {
		activeAbility.DeactivateAbility();
		if (activeAbility.Equals(ability1)) {
			activeAbility = ability2;
		} else {
			activeAbility = ability1;
		}
		activeAbility.ActivateAbility();
		Debug.Log("ActiveAbility:"+activeAbility.GetType());
	}

	/// <summary>
	/// Usa la habilidad del personaje seleccionada
	/// </summary>
	public void UseAbility () {
		if (!activeAbility.execution) {
			if (characterStatus.CanStartAbility(activeAbility)) {
				activeAbility.StartExecution();
			}
		} else {
			if (activeAbility.EndExecution()) {
				characterStatus.EndAbility(activeAbility);
			}
		}
	}
}
