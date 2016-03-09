using UnityEngine;
using System.Collections;

/* Class to define the ability of jumping more than one time */
public class MultipleJumpAbility : CharacterAbility {
	// Number of jumps and impulse modifier
	public int possibleJumps = 2;
	public float jumpImpulseModifier = 1.0f;

	public int jumpNumber;
	private CharacterMovement charMovement;

	// Use this for initialization
	void Start () {
		jumpNumber = 0;
		charMovement = GetComponent<CharacterMovement>();
	}

	// Finish the execution of the ability (in this case, it has no sense so return 'false')
	public override bool EndExecution () {
		return false;
	}

	// Start the execution of the ability (in this case, it has no sense so return 'false')
	public override bool StartExecution () {
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		// Jump action
		if ((active) && (Input.GetButtonDown("Jump"))) {
			if (jumpNumber == 0) {
				// Skip first jump
				jumpNumber += 1;
			} else if (jumpNumber < possibleJumps) {
				// Extra jump
				//charMovement.ExtraJump(jumpImpulseModifier);
				jumpNumber += 1;
			}
		}
	}
}