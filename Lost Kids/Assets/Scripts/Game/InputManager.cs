using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour, ILockeable {
	private bool locked;
	private CharacterMovement charMov;
	private AbilityController abilityControl;
	private PlayerUse playerUse;

	// Use this for initialization
	void Start () {
		locked = false;
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		charMov = player.GetComponent<CharacterMovement>();
		abilityControl = player.GetComponent<AbilityController>();
		playerUse = player.GetComponent<PlayerUse>();
	}

	// Manage inputs that produce physics
	void FixedUpdate () {
		if (!locked) {
			// Character movement
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");
			// Jump button
			bool jump = Input.GetButtonDown("Jump");
			if (jump) {
				charMov.JumpButton();
			}
			// Apply the movement
			if ((jump) || (horizontal != 0) || (vertical != 0f)) {
				charMov.MoveCharacter(horizontal, vertical);
			}
		}
	}

	public void Lock () {
		locked = true;
	}

	public void Unlock () {
		locked = false;
	}
	
	// Manage general inputs
	void Update () {
		if (!locked) {
			// Crouch Button
			if (Input.GetButtonDown("Crouch")) {
				charMov.CrouchButton();
			}
			// Abilities Buttons
			if (Input.GetButtonDown("ChangeAbility")) {
				abilityControl.ChangeAbility();
			}
			if (Input.GetButtonDown("UseAbility")) {
				abilityControl.UseAbility();
			}
			// Use Button
			if (Input.GetButton("Use")) {
				playerUse.Use();
			}
		}
	}
}
