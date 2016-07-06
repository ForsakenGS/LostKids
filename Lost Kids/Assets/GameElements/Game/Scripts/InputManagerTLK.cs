using UnityEngine;
using InControl;

public class InputManagerTLK : MonoBehaviour {
    // Controles del juego
    InputControlType character1Control = InputControlType.DPadLeft;
    InputControlType character2Control = InputControlType.DPadUp;
    InputControlType character3Control = InputControlType.DPadRight;
    InputControlType nextCharacterControl = InputControlType.LeftTrigger;
    InputControlType prevCharacterControl = InputControlType.RightTrigger;
    InputControlType jumpControl = InputControlType.Action1;
    InputControlType Ability2Control = InputControlType.RightBumper;
    InputControlType useControl = InputControlType.Action3;
    InputControlType Ability1Control = InputControlType.LeftBumper;
    InputControlType menuControl = InputControlType.Command;
    InputControlType sacrificeControl = InputControlType.Action4;
    InputControlType crouchControl = InputControlType.Action2;

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
            case "Character1":
                res = character1Control;
                break;
            case "Character2":
                res = character2Control;
                break;
            case "Character3":
                res = character3Control;
                break;
            case "Ability2":
                res = Ability2Control;
                break;
            case "Ability1":
                res = Ability1Control;
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

        return control.WasPressed;
    }

    bool GetControlUp(InputControlType controlType) {
        InputControl control = InputManager.ActiveDevice.GetControl(controlType);

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
            if (ButtonDown("Character1")) {
                characterManager.ActivateCharacter(0);
            } else if (ButtonDown("Character2")) {
                characterManager.ActivateCharacter(1);
            } else if (ButtonDown("Character3")) {
                characterManager.ActivateCharacter(2);
            }
            // Abilities Buttons
            if (ButtonDown("Ability1")) {
                abilityControl.ActivateAbility1();
            } else if (ButtonDown("Ability2")) {
                abilityControl.ActivateAbility2();
            }
            // Use Button
            if (ButtonDown("Use")) {
                characterStatus.UseButton();
            }
            // Movement buttons
            horizontalButton = ButtonValue("Horizontal");
            verticalButton = ButtonValue("Vertical");
            // Jump button
            if (ButtonDown("Jump")) {
                characterStatus.JumpButtonDown();
            } else if (ButtonUp("Jump")) {
                characterStatus.JumpButtonUp();
            } else if (Button("Jump")) {
                jumpButton = true;
            }
            // Suicide button
            if (Button("Sacrifice")) {
                characterStatus.SacrificeButton();
            } else if (ButtonUp("Sacrifice")) {
                characterStatus.SacrificeButtonUp();
            }
        } else {
            //Pasar mensajes
            if (ButtonDown("Jump")) {
                if (messageManager.ShowingMessage())
                    messageManager.SkipText();
            }
        }
    }

    public void OnApplicationQuit() {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, 0, 0);
    }
}