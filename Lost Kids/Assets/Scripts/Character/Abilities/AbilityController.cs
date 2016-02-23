using UnityEngine;
using System.Collections;

public class AbilityController : MonoBehaviour {
	private CharacterAbility ability1;
	private CharacterAbility ability2;
	private CharacterAbility activeAbility;

	// Use this for initialization
	void Start () {
		ability1 = GetComponent<MultipleJumpAbility>();
		ability2 = GetComponent<SprintAbility>();
		activeAbility = ability1;
		ability1.ActivateAbility();
	}

	public void ChangeAbility () {
		activeAbility.DeactivateAbility();
		if (activeAbility.Equals(ability1)) {
			activeAbility = ability2;
		} else {
			activeAbility = ability1;
		}
		activeAbility.ActivateAbility();
	}

	public void UseAbility () {
		if (!activeAbility.execution) {
			activeAbility.StartExecution();
		} else {
			activeAbility.EndExecution();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
