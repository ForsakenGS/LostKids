using UnityEngine;
using System.Collections;

/* Class to define the ability of jumping more than one time */
public class BigJumpAbility : CharacterAbility {
	// Number of jumps and impulse modifier
	public float jumpImpulseModifier = 2.0f;

	// Finish the execution of the ability (in this case, it has no sense so return 'false')
	public override bool EndExecution () {
        execution = false;

		return execution;
	}

	// Start the execution of the ability (in this case, it has no sense so return 'false')
	public override bool StartExecution () {
		bool started = !execution;
		if (!execution) {
			execution = true;
			characterMovement.Jump(jumpImpulseModifier * characterStatus.jumpImpulse);
		}

		return started;
	}
}