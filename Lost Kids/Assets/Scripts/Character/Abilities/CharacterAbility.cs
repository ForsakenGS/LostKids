using UnityEngine;
using System.Collections;

/* Abstract class to define an ability for the character */
public abstract class CharacterAbility : MonoBehaviour {
	public bool active = false;
	public bool execution = false;

	// Activate the ability for the character, deactivating the rest of them
	public void ActivateAbility() {
		active = true;
	}

	// Deactivate the ability for the character
	public void DeactivateAbility() {
		active = false;
		if (execution) {
			EndExecution();
		}
	}

	public abstract bool EndExecution ();

	public abstract bool StartExecution ();

}