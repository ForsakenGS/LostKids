using UnityEngine;
using System.Collections;

/* Class to define the ability of sprint */
public class SprintAbility : CharacterAbility {
	// Speed modifier & execution time controls
	public float speedModifier = 2.0f;
	public float duration = 10;
	public float timeToRestore = 12;

	private float executionTimeLeft;
	private CharacterMovement charMovement;

	// Use this for initialization
	void Start () {
		executionTimeLeft = 0;
		charMovement = GetComponent<CharacterMovement>();
	}

	// Finish the execution of the ability, restoring the speed modifier
	public override bool EndExecution () {
		bool ended = execution;
		if (execution) {
			execution = false;
			charMovement.speedModifier = 1.0f;
		}

		return ended;
	}

	// Start the execution of the ability, setting the speed modifier
	public override bool StartExecution () {
		bool started = !execution;
		if (!execution) {
			execution = true;
			charMovement.speedModifier = speedModifier;
		}

		return started;
	}

	// Update is called once per frame
	void Update () {
		// Ability activation
		if (Input.GetButtonDown("ChangeAbility")) {
			if (active) {
				DeactivateAbility();
			} else {
				ActivateAbility();
			}
		}
		// Sprint execution
		if ((active) && (Input.GetButtonDown("UseAbility"))) {
			if (!execution) {
				StartExecution();
			} else {
				EndExecution();
			}
		}
		// Ability time control
		if (execution) {
			// Executing, so decrease time
			executionTimeLeft -= Time.deltaTime;
			if (executionTimeLeft <= 0) {
				EndExecution();
			}
		} else {
			// Not executing, so restore time
			executionTimeLeft += ((Time.deltaTime / timeToRestore) * duration);
			if (executionTimeLeft > duration) {
				executionTimeLeft = duration;
			}
		}
	}
}