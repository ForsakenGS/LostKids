using UnityEngine;
using UnityEngine.EventSystems;
using InControl;

public class InputManagerTLKMenu : MonoBehaviour {
    /// <summary>
    /// EventSystem sobre el que ejecutar los eventos del mando 
    /// </summary>
    public EventSystem eventSystem;

    // Controles del menú
    InputControlType downControl = InputControlType.DPadDown;
    InputControlType upControl = InputControlType.DPadUp;
    InputControlType leftControl = InputControlType.DPadLeft;
    InputControlType rightControl = InputControlType.DPadRight;
    InputControlType submitControl = InputControlType.Action1;
    InputControlType menuControl = InputControlType.Command;

    bool ButtonDown(string button) {
        // Entrada por mando
        bool res = GetControlDown(GetButtonControl(button));

        return res;
    }

    InputControlType GetButtonControl(string button) {
        // Selecciona el InputControlType al que se referencia
        InputControlType res = InputControlType.None;
        switch (button) {
            case "Submit":
                res = submitControl;
                break;
            case "Menu":
                res = menuControl;
                break;
            case "MenuDown":
                res = downControl;
                break;
            case "MenuUp":
                res = upControl;
                break;
            case "MenuLeft":
                res = leftControl;
                break;
            case "MenuRight":
                res = rightControl;
                break;
        }

        return res;
    }

    bool GetControlDown(InputControlType controlType) {
        InputControl control = InputManager.ActiveDevice.GetControl(controlType);

        return control.WasPressed;
    }

    void OnDisable() {
        if (eventSystem != null) {
            eventSystem.gameObject.SetActive(false);
        }
    }

    void OnEnable() {
        eventSystem.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        // Submit button
        if (ButtonDown("Submit")) {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }
        // Cancel button
        if (ButtonDown("Menu")) {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, new BaseEventData(eventSystem), ExecuteEvents.cancelHandler);
        }
        // Direction buttons
        if (ButtonDown("MenuUp")) {
            AxisEventData axisEventData = new AxisEventData(eventSystem);
            axisEventData.moveDir = MoveDirection.Up;
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
        } else if (ButtonDown("MenuDown")) {
            AxisEventData axisEventData = new AxisEventData(eventSystem);
            axisEventData.moveDir = MoveDirection.Down;
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
        } else if (ButtonDown("MenuLeft")) {
            AxisEventData axisEventData = new AxisEventData(eventSystem);
            axisEventData.moveDir = MoveDirection.Left;
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
        } else if (ButtonDown("MenuRight")) {
            AxisEventData axisEventData = new AxisEventData(eventSystem);
            axisEventData.moveDir = MoveDirection.Right;
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
        }
    }
}
