using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public bool locked;

	private CharacterMovement charMov;
	private AbilityController abilityControl;
	private PlayerUse playerUse;
    private MessageManager messageManager;
	private CharacterManager characterManager;

	// Use this for references
	void Awake () {
		characterManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<CharacterManager>();
		messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
	}

	// Use this for initialization
	void Start () {
		// Suscripciones a eventos
		CharacterManager.ActiveCharacterChangedEvent += CharacterComponentsUpdate;
	}

	// Update references to current character
	void CharacterComponentsUpdate () {
		GameObject player = characterManager.GetActiveCharacter();
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
	
	// Manage general inputs
	void Update () {
		CharacterComponentsUpdate();
		if (!locked) {
			// Switch Players Buttons
			if (Input.GetButtonDown("Player1")) {
				characterManager.ActivateCharacter(0);
			} else if (Input.GetButtonDown("Player2")) {
				characterManager.ActivateCharacter(1);
			} else if (Input.GetButtonDown("Player3")) {
				characterManager.ActivateCharacter(2);
			}
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
		} else {
            //Pasar mensajes
            if(Input.GetButtonDown("Jump")) {
                messageManager.SkipText();
            }
        }
	}
}