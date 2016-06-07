using UnityEngine;
using InControl;

public class InputManagerTLK : MonoBehaviour {
    // Controles del juego
    InputControlType player1Control = InputControlType.DPadUp;
    InputControlType player2Control = InputControlType.DPadRight;
    InputControlType player3Control = InputControlType.DPadDown;
    InputControlType jumpControl = InputControlType.Action1;
    InputControlType useAbilityControl = InputControlType.Action2;
    InputControlType useControl = InputControlType.Action3;
    InputControlType changeAbilityControl = InputControlType.Action4;
    InputControlType menuControl = InputControlType.Command;
    InputControlType sacrificeControl = InputControlType.LeftBumper;
    InputControlType crouchControl = InputControlType.RightBumper;

    private static bool locked;
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

    bool Button(string button) {
        // Entrada por teclado
        bool res = Input.GetButton(button);
        if (!res) {
            // Entrada por mando
            res = GetControl(GetButtonControl(button));
        }

        return res;
    }

    bool ButtonDown(string button) {
        // Entrada por teclado
        bool res = Input.GetButtonDown(button);
        if (!res) {
            // Entrada por mando
            res = GetControlDown(GetButtonControl(button));
        }

        return res;
    }

    bool ButtonUp(string button) {
        // Entrada por teclado
        bool res = Input.GetButtonUp(button);
        if (!res) {
            // Entrada por mando
            res = GetControlUp(GetButtonControl(button));
        }

        return res;
    }

    float ButtonValue(string button) {
        // Entrada por teclado
        float res = Input.GetAxis(button);
        if (res == 0.0f) {
            // Entrada por mando
            if (button.Equals("Horizontal")) {
                res = InputManager.ActiveDevice.LeftStickX.Value;
            } else if (button.Equals("Vertical")) {
                res = InputManager.ActiveDevice.LeftStickY.Value;
            }
        }

        return res;
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

    InputControlType GetButtonControl(string button) {
        // Selecciona el InputControlType al que se referencia
        InputControlType res = InputControlType.None;
        switch (button) {
            case "Jump":
                res = jumpControl;
                break;
            case "Use":
                res = useControl;
                break;
            case "Player1":
                res = player1Control;
                break;
            case "Player2":
                res = player2Control;
                break;
            case "Player3":
                res = player3Control;
                break;
            case "UseAbility":
                res = useAbilityControl;
                break;
            case "ChangeAbility":
                res = changeAbilityControl;
                break;
            case "Crouch":
                res = crouchControl;
                break;
            case "Sacrifice":
                res = sacrificeControl;
                break;
            case "Menu":
                res = menuControl;
                break;
        }

        return res;
    }

    bool GetControl(InputControlType controlType) {
        return (InputManager.ActiveDevice.GetControl(controlType).IsPressed);
    }

    bool GetControlDown(InputControlType controlType) {
        InputControl control = InputManager.ActiveDevice.GetControl(controlType);
        //return (!control.WasPressed && control.IsPressed);
        return control.WasPressed;
    }

    bool GetControlUp(InputControlType controlType) {
        InputControl control = InputManager.ActiveDevice.GetControl(controlType);
        //return (control.WasPressed && !control.IsPressed);
        return control.WasReleased;
    }

    void Lock() {
        locked = true;
    }

    public void LockTime(float time) {
        locked = true;
        Invoke("Unlock", time);
    }

    /// <summary>
    /// Función para bloquear/desbloquear el paso de instrucciones al personaje 
    /// </summary>
    /// <param name="lockVar"></param>
    public static void SetLock(bool lockVar) {
        locked = lockVar;
    }

    void Unlock() {
        locked = false;
    }

    public void UnlockTime(float time) {
        locked = false;
        Invoke("Lock", time);
    }

    // Manage general inputs
    void Update() {
        if (ButtonDown("Menu")) {
            if (!GameManager.paused) {
                PausePanel.ShowPanel();
                GameManager.PauseGame();
            }
        }

        if (!locked) {
            // Switch Players Buttons
            if (ButtonDown("Player1")) {
                characterManager.ActivateCharacter(0);
            } else if (ButtonDown("Player2")) {
                characterManager.ActivateCharacter(1);
            } else if (ButtonDown("Player3")) {
                characterManager.ActivateCharacter(2);
            }
            // Crouch Button
            if (ButtonDown("Crouch")) {
                characterStatus.CrouchButton();
            }
            // Abilities Buttons
            if (ButtonDown("ChangeAbility")) {
                abilityControl.ChangeAbility();
            }
            if (ButtonDown("UseAbility")) {
                abilityControl.UseAbility();
            }
            // Use Button
            if (ButtonDown("Use")) {
                characterStatus.UseButton();
            }
            // Movement buttons
            horizontalButton = ButtonValue("Horizontal");
            verticalButton = ButtonValue("Vertical");
            // Jump button
            if (ButtonUp("Jump")) {
                characterStatus.JumpButtonUp();
            } else if (Button("Jump")) {
                jumpButton = true;
            }
            // Suicide button
            if (ButtonDown("Sacrifice")) {
                characterStatus.SacrificeButton();
            }
        } else {
            //Pasar mensajes
            if (ButtonDown("Jump")) {
                if(messageManager.ShowingMessage())
                messageManager.SkipText();
            }
        }
    }
}