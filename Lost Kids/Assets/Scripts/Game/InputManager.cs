using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public bool locked;
	private CharacterStatus characterStatus;
	private AbilityController abilityControl;
    private MessageManager messageManager;
	private CharacterManager characterManager;

	// Use this for references
	void Awake () {
		characterManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<CharacterManager>();
		messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
	}

	// Use this for initialization
	void Start () {
        // Inicialización variables
        locked = false;
        CharacterComponentsUpdate();
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent += CharacterComponentsUpdate;
	}

	// Update references to current character
	void CharacterComponentsUpdate () {
		GameObject player = CharacterManager.GetActiveCharacter();
		abilityControl = player.GetComponent<AbilityController>();
		characterStatus = player.GetComponent<CharacterStatus>();
	}

	// Manage inputs that produce physics
	void FixedUpdate () {
		if (!locked) {
			// Character movement
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");
			if ((horizontal != 0) || (vertical != 0f)) {
				characterStatus.MovementButtons(horizontal, vertical);
			}
			// Jump button
			if (Input.GetButtonDown("Jump")) {
				characterStatus.JumpButton();
			}
		}
	}
    /// <summary>
    /// Función para bloquear/desbloquear la lectura de 
    /// </summary>
    /// <param name="lockVar"></param>
    public void SetLock(bool lockVar) {
        locked = lockVar;
    }
	
	// Manage general inputs
	void Update () {
		//CharacterComponentsUpdate();	// CAMBIAR!! Quitar esto de aquí y ponerlo en otro sitio para el problema de referencias
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
				characterStatus.CrouchButton();
			}
			// Abilities Buttons
			if (Input.GetButtonDown("ChangeAbility")) {
				abilityControl.ChangeAbility();
			}
			if (Input.GetButtonDown("UseAbility")) {
				abilityControl.UseAbility();
			}
			// Use Button
			if (Input.GetButtonDown("Use")) {
				characterStatus.UseButton();
			}
		} else {
            //Pasar mensajes
            if(Input.GetButtonDown("Jump")) {
                messageManager.SkipText();
            }
        }
	}
}