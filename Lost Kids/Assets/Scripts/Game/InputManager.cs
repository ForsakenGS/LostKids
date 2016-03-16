using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    private bool locked;
    private CharacterStatus characterStatus;
    private AbilityController abilityControl;
    private MessageManager messageManager;
    private CharacterManager characterManager;
    private float horizontalButton;
    private float verticalButton;
    private bool jumpButton;

    // Use this for references
    void Awake() {
        characterManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<CharacterManager>();
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
    }

    // Use this for initialization
    void Start() {
        // Inicialización variables
        locked = false;
        horizontalButton = 0.0f;
        verticalButton = 0.0f;
        jumpButton = false;
        CharacterComponentsUpdate();
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent += CharacterComponentsUpdate;
    }

    // Update references to current character
    void CharacterComponentsUpdate() {
        GameObject player = CharacterManager.GetActiveCharacter();
        abilityControl = player.GetComponent<AbilityController>();
        characterStatus = player.GetComponent<CharacterStatus>();
    }

    // Manage inputs that produce physics
    void FixedUpdate() {
        if (!locked) {
            // Character movement
            if ((horizontalButton != 0) || (verticalButton != 0f)) {
                characterStatus.MovementButtons(horizontalButton, verticalButton);
                horizontalButton = 0.0f;
                verticalButton = 0.0f;
            }
            // Jump button
            if (jumpButton) {
                characterStatus.JumpButton();
                jumpButton = false;
            }
        }
    }
    /// <summary>
    /// Función para bloquear/desbloquear el paso de instrucciones al personaje 
    /// </summary>
    /// <param name="lockVar"></param>
    public void SetLock(bool lockVar) {
        locked = lockVar;
    }

    // Manage general inputs
    void Update() {
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
            // Movement buttons
            horizontalButton = Input.GetAxis("Horizontal");
            verticalButton = Input.GetAxis("Vertical");
            // Jump button
            if (Input.GetButtonDown("Jump")) {
                jumpButton = true;
            }
        } else {
            //Pasar mensajes
            if (Input.GetButtonDown("Jump")) {
                messageManager.SkipText();
            }
        }
    }
}