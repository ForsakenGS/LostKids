using UnityEngine;
using System.Collections;

public abstract class CharacterAbility : MonoBehaviour {
	public bool active = false;
	public bool execution = false;

	public void ActivateAbility() {
		active = true;
	}

	public void DeactivateAbility() {
		active = false;
		if (execution) {
			EndExecution();
		}
	}

	public abstract bool EndExecution ();

	public abstract bool StartExecution ();

}