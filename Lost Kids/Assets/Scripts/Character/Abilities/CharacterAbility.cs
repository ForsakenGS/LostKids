using UnityEngine;
using System.Collections;

/* Abstract class to define an ability for the character */
public abstract class CharacterAbility : MonoBehaviour {
	public bool active = false;
	public bool execution = false;
	public float duration;
	public float timeToRestore;

	protected CharacterStatus characterStatus;
	protected CharacterMovement characterMovement;
	protected float executionTimeLeft;

	void Awake() {
		characterStatus = GetComponent<CharacterStatus>();
		characterMovement = GetComponent<CharacterMovement>();
	}

	void Start() {
		executionTimeLeft = 0.0f;
	}

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

	void Update() {
		// Control del tiempo de habilidad
		if (duration > 0) {
			if (execution) {
				// En ejecución, luego se decrementa el tiempo restante
				executionTimeLeft -= Time.deltaTime;
				if (executionTimeLeft <= 0.0) {
					// La habilidad debe terminar su ejecución
					Debug.Log("c");
					GetComponent<AbilityController>().UseAbility();
				}
			} else if (executionTimeLeft < duration) {
				// No en ejecución y el tiempo restante no está completo, luego se va recuperando
				if (timeToRestore == 0) {
					executionTimeLeft = duration;
				} else {
					executionTimeLeft += ((Time.deltaTime / timeToRestore) * duration);
				}
				if (executionTimeLeft > duration) {
					executionTimeLeft = duration;
				}
			}
		}
	}
}