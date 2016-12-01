using UnityEngine;
using UnityEngine.EventSystems;
using InControl;
using System.Collections;

public class InputManagerTLK : MonoBehaviour {
    /// <summary>
    /// Valor mínimo para considerar movimiento del jugador
    /// </summary>
    public float minValueMovButton = 0f;
    /// <summary>
    /// EventSystem sobre el que ejecutar los eventos del mando 
    /// </summary>
    public EventSystem eventSystem;
    /// <summary>
    /// Menú de pausa
    /// </summary>
    public GameObject pauseMenu;

    // Controles del juego
    InputControlType character1Control = InputControlType.DPadLeft;
    InputControlType character2Control = InputControlType.DPadUp;
    InputControlType character3Control = InputControlType.DPadRight;
    InputControlType nextCharacterControl = InputControlType.RightBumper;
    InputControlType prevCharacterControl = InputControlType.LeftBumper;
    InputControlType jumpControl = InputControlType.Action1;
    InputControlType ability2Control = InputControlType.Action2;
    InputControlType useControl = InputControlType.Action3;
    InputControlType ability1Control = InputControlType.Action4;
    InputControlType menuControl = InputControlType.Command;
    InputControlType sacrifice1Control = InputControlType.LeftTrigger;
    InputControlType sacrifice2Control = InputControlType.RightTrigger;
    InputControlType menuDownControl = InputControlType.DPadDown;
    InputControlType menuUpControl = InputControlType.DPadUp;
    InputControlType menuLeftControl = InputControlType.DPadLeft;
    InputControlType menuRightControl = InputControlType.DPadRight;

    private int locked;
    private CharacterStatus characterStatus;
    private AbilityController abilityControl;
    private MessageManager messageManager;
    private CharacterManager characterManager;
    private float horizontalButton;
    private float verticalButton;
    private int jumpButton;
    private bool menuMode;

    private static InputManagerTLK instance = null;

    // Use this for references
    void Awake() {
        characterManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<CharacterManager>();
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
        instance = this;
    }

    // Use this for initialization
    void Start() {
        // Inicialización variables
        locked = 0;
        horizontalButton = 0.0f;
        verticalButton = 0.0f;
        jumpButton = 0;
        CharacterComponentsUpdate(CharacterManager.GetActiveCharacter());
        // Suscripciones a eventos
        //CharacterManager.ActiveCharacterChangedEvent += CharacterComponentsUpdate;
    }

    void AddLock() {
        locked += 1;
    }

    void RemoveLock() {
        if (locked > 0) {
            locked -= 1;
        }
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
    void CharacterComponentsUpdate(GameObject character) {
        GameObject player = character;
        abilityControl = player.GetComponent<AbilityController>();
        characterStatus = player.GetComponent<CharacterStatus>();
        AddLock();
        Invoke("RemoveLock", 0.25f);
    }

    // Manage inputs that produce physics
    void FixedUpdate() {
        if (locked == 0) {
            // Character movement
            if ((Mathf.Abs(horizontalButton) >= minValueMovButton) || (Mathf.Abs(verticalButton) >= minValueMovButton)) {
                // Slow movement?
                if (Input.GetButton("Slow")) {
                    horizontalButton /= 4;
                    verticalButton /= 4;
                }
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
                res = ability2Control;
                break;
            case "Ability1":
                res = ability1Control;
                break;
            case "Sacrifice1":
                res = sacrifice1Control;
                break;
            case "Sacrifice2":
                res = sacrifice2Control;
                break;
            case "Menu":
                res = menuControl;
                break;
            case "MenuDown":
                res = menuDownControl;
                break;
            case "MenuUp":
                res = menuUpControl;
                break;
            case "MenuLeft":
                res = menuLeftControl;
                break;
            case "MenuRight":
                res = menuRightControl;
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
        instance.AddLock();
    }

    void OnDisable() {
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent -= CharacterComponentsUpdate;
        EndVibration();
    }

    void OnEnable() {
        // Suscripciones a eventos
        CharacterManager.ActiveCharacterChangedEvent += CharacterComponentsUpdate;
    }

    public void ResumeGame() {
        if (menuMode) {
            menuMode = false;
            GameManager.ResumeGame();
            PausePanel.HidePanel();
            Unlock();
        }
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
        instance.RemoveLock();
    }

    // Manage general inputs
    void Update() {
        if (ButtonDown("Menu")) {
            if (!GameManager.paused) {
                Lock();
                PausePanel.ShowPanel();
                GameManager.PauseGame();
                menuMode = true;
            } else {
                ResumeGame();
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
            } else
            // Abilities Buttons
            if (ButtonDown("Ability1")) {
                abilityControl.ActivateAbility1();
            } else if (ButtonDown("Ability2")) {
                abilityControl.ActivateAbility2();
            } else
            // Use Button
            if (ButtonDown("Use")) {
                characterStatus.UseButton();
            } else
            // Jump button
            if (ButtonDown("Jump")) {
                jumpButton = 2;
            } else if (ButtonUp("Jump")) {
                characterStatus.JumpButtonUp();
            } else if (Button("Jump")) {
                if (jumpButton != 2) {
                    jumpButton = 1;
                }
            } else
            // Suicide button
            if ((Button("Sacrifice1")) && (Button("Sacrifice2"))) {
                characterStatus.SacrificeButtons();
            } else if ((ButtonUp("Sacrifice1")) || (ButtonUp("Sacrifice2"))) {
                characterStatus.SacrificeButtonsUp();
            }
            // Movement buttons
            horizontalButton = ButtonValue("Horizontal");
            verticalButton = ButtonValue("Vertical");
        } else if (menuMode) {
            // Controla únicamente la entrada por mando, ya que el EventSystem no lo hace
            // Submit button
            if (GetControlDown(GetButtonControl("Jump"))) {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
            }
            // Direction buttons
            if (GetControlDown(GetButtonControl("MenuUp"))) {
                AxisEventData axisEventData = new AxisEventData(eventSystem);
                axisEventData.moveDir = MoveDirection.Up;
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            } else if (GetControlDown(GetButtonControl("MenuDown"))) {
                AxisEventData axisEventData = new AxisEventData(eventSystem);
                axisEventData.moveDir = MoveDirection.Down;
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            } else if (GetControlDown(GetButtonControl("MenuLeft"))) {
                AxisEventData axisEventData = new AxisEventData(eventSystem);
                axisEventData.moveDir = MoveDirection.Left;
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            } else if (GetControlDown(GetButtonControl("MenuRight"))) {
                AxisEventData axisEventData = new AxisEventData(eventSystem);
                axisEventData.moveDir = MoveDirection.Right;
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            }
        } else {
            //Pasar mensajes
            if (ButtonDown("Use")) {
                if (messageManager.ShowingMessage()) {
                    messageManager.SkipText();
                }
            }
        }
    }

    public static void BeginVibration(float leftAmount, float rightAmount) {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, leftAmount, rightAmount);
    }

    public static void BeginVibration(float amount) {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, amount, amount);
    }

    public static void BeginVibrationTimed(float amount, float duration) {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, amount, amount);
        Instance().Invoke("endVibration", duration);
    }

    public static void BeginVibrationTimed(float leftAmount, float rightAmount, float duration) {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, leftAmount, rightAmount);
        Instance().Invoke("endVibration", duration);
    }

    public static void BeginVibrationTimed(float amount, float duration, bool fading) {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, amount, amount);
        if (!fading) {
            Instance().Invoke("endVibration", duration);
        } else {
            instance.StartCoroutine(instance.FadeVibration(amount, duration));
        }
    }

    public static void BeginVibrationTimed(float leftAmount, float rightAmount, float duration, bool fading) {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, leftAmount, rightAmount);
        if (!fading) {
            Instance().Invoke("endVibration", duration);
        } else {
            instance.StartCoroutine(instance.FadeVibration(leftAmount, rightAmount, duration));
        }
    }

    private IEnumerator FadeVibration(float initialVibration, float time) {
        float vibration = initialVibration;
        float t = 0;
        while (t < time) {
            vibration = Mathf.Lerp(vibration, 0, t / time);
            t += Time.deltaTime;
            BeginVibration(vibration);
            yield return null;
        }
        EndVibration();
        yield return null;
    }

    private IEnumerator FadeVibration(float initialLeftVibration, float initialRightVibration, float time) {
        float leftVibration = initialLeftVibration;
        float rightVibration = initialRightVibration;
        float t = 0;
        while (t < time) {
            leftVibration = Mathf.Lerp(leftVibration, 0, t / time);
            rightVibration = Mathf.Lerp(rightVibration, 0, t / time);
            t += Time.deltaTime;
            BeginVibration(leftVibration, rightVibration);
            yield return null;
        }
        EndVibration();
        yield return null;
    }

    public static void EndVibration() {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, 0, 0);
    }

    private void endVibration() {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, 0, 0);
    }

    public static InputManagerTLK Instance() {
        return instance;
    }

    public void OnApplicationQuit() {
        EndVibration();
    }

    public static void SetMenuMode(bool menuMode) {
        instance.menuMode = menuMode;
        if (menuMode) {
            Lock();
        } else {
            Unlock();
        }
    }
}