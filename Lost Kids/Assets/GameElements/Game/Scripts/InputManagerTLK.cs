﻿using UnityEngine;
using InControl;

public class InputManagerTLK : MonoBehaviour {
    /// <summary>
    /// Valor mínimo para considerar movimiento del jugador
    /// </summary>
    public float minValueMovButton = 0f;

    // Controles del juego
    InputControlType character1Control = InputControlType.DPadLeft;
    InputControlType character2Control = InputControlType.DPadUp;
    InputControlType character3Control = InputControlType.DPadRight;
    InputControlType nextCharacterControl = InputControlType.RightTrigger;
    InputControlType prevCharacterControl = InputControlType.LeftTrigger;
    InputControlType jumpControl = InputControlType.Action1;
    InputControlType Ability2Control = InputControlType.RightBumper;
    InputControlType useControl = InputControlType.Action3;
    InputControlType Ability1Control = InputControlType.LeftBumper;
    InputControlType menuControl = InputControlType.Command;
    InputControlType sacrificeControl = InputControlType.Action4;
    InputControlType crouchControl = InputControlType.Action2;

    private static int locked;
    private CharacterStatus characterStatus;
    private AbilityController abilityControl;
    private MessageManager messageManager;
    private CharacterManager characterManager;
    private float horizontalButton;
    private float verticalButton;
    private int jumpButton;

    // Use this for references
    void Awake() {
        characterManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<CharacterManager>();
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
    }

    // Use this for initialization
    void Start() {
        // Inicialización variables
        locked = 0;
        horizontalButton = 0.0f;
        verticalButton = 0.0f;
        jumpButton = 0;
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
        if (locked == 0) {
            // Character movement
            if ((Mathf.Abs(horizontalButton) >= minValueMovButton) || (Mathf.Abs(verticalButton) >= minValueMovButton)) {
                characterStatus.MovementButtons(horizontalButton, verticalButton);
                horizontalButton = 0.0f;
                verticalButton = 0.0f;
            }
            // Jump button
            if (jumpButton == 1) {
                characterStatus.JumpButton();
                jumpButton = 0;
            } else if (jumpButton == 2) {
                characterStatus.JumpButtonDown();
                jumpButton = 0;
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
            case "NextCharacter":
                res = nextCharacterControl;
                break;
            case "PrevCharacter":
                res = prevCharacterControl;
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

    static void Lock() {
        locked += 1;
    }

    //public void LockTime(float time) {
    //    Lock();
    //    Invoke("Unlock", time);
    //}

    void OnDisable() {
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent -= CharacterComponentsUpdate;
    }

    void OnEnable() {
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent += CharacterComponentsUpdate;
    }

    /// <summary>
    /// Función para bloquear/desbloquear el paso de instrucciones al personaje 
    /// </summary>
    /// <param name="lockVar"></param>
    public static void SetLock(bool lockVar) {
        if (lockVar) {
            Lock();
        } else {
            Unlock();
        }
    }

    static void Unlock() {
        //if (!hardLocked) {
        if (locked > 0) {
            locked -= 1;
        }
        //}
    }

    //public void UnlockTime(float time) {
    //    Unlock();
    //    Invoke("Lock", time);
    //}

    // Manage general inputs
    void Update() {
        if (ButtonDown("Menu")) {
            if (!GameManager.paused) {
                PausePanel.ShowPanel();
                GameManager.PauseGame();
                Lock();
            } else {
                Unlock();
            }
        } else if (locked == 0) {
            // Switch Players Buttons
            if (ButtonDown("Character1")) {
                characterManager.ActivateCharacter(0);
            } else if (ButtonDown("Character2")) {
                characterManager.ActivateCharacter(1);
            } else if (ButtonDown("Character3")) {
                characterManager.ActivateCharacter(2);
            } else if (ButtonDown("NextCharacter")) {
                characterManager.ActivateNextCharacter();
            } else if (ButtonDown("PrevCharacter")) {
                characterManager.ActivatePreviousCharacter();
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
                jumpButton = 2;
            } else if (ButtonUp("Jump")) {
                characterStatus.JumpButtonUp();
            } else if (Button("Jump")) {
                if (jumpButton != 2) {
                    jumpButton = 1;
                }
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
                if (messageManager.ShowingMessage()) {
                    messageManager.SkipText();
                }
            }
        }
        //if (hardLocked) {
        //    Debug.Log("hard");
        //}
        //if (locked) {
        //    Debug.Log("looo");
        //}
    }

    public void OnApplicationQuit() {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, 0, 0);
    }
}