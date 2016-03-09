/* Class to define the ability of sprint */
public class SprintAbility : CharacterAbility {
	// Speed modifier
	public float speedModifier = 2.0f;

	// Finish the execution of the ability, restoring the speed modifier
	public override bool EndExecution () {
		bool ended = execution;
		if (execution) {
			execution = false;
			characterStatus.standingSpeed /= speedModifier;
		}

		return ended;
	}

	// Start the execution of the ability, setting the speed modifier
	public override bool StartExecution () {
		bool started = false;
		if (!execution) {
			execution = true;
            started = true;
            energy -= initialConsumption;
			characterStatus.standingSpeed *= speedModifier;
		}

		return started;
	}
}