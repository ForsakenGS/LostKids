using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Kodama : UsableObject {
    // Listado con índices de los mensajes a mostrar
    public List<int> indexList;

    public float floatSpeed = 0.1f;
    private Vector3 basePosition;
    // Referencia a MessageManager
    private MessageManager messageManager;

    private CutScene cutScene;
    private GameObject canvas;

    // Use this for references
    void Awake() {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
        cutScene = GetComponent<CutScene>();
        basePosition = transform.position;
        if (GetComponentInChildren<Canvas>() != null)
        {
            canvas = GetComponentInChildren<Canvas>().gameObject;
        }
        ChangeTooltipStatus(TooltipManager.On);
    }

    // Muestra/oculta el tooltip del Kodama
    void ChangeTooltipStatus(bool status) {
        canvas.SetActive(status);
    }

    void OnEnable() {
        TooltipManager.TooltipOnOff += ChangeTooltipStatus;
        MoveUp();
    }

    void OnDisable() {
        TooltipManager.TooltipOnOff -= ChangeTooltipStatus;
        transform.position = basePosition;
        iTween.Stop(gameObject);
    }

    /// <summary>
    /// Metodo que se activa al usar el objeto y muestra los mensajes en orden
    /// </summary>
    override public void Use() {
        // Muestra la conversación
        if (cutScene == null) {
            messageManager.ShowConversation(indexList);
        } else {
            cutScene.BeginCutScene(BeginConversation);
            Invoke("MoveCharacterToFront", 0.5f);
        }
    }

    private void MoveCharacterToFront() {
        GameObject character = CharacterManager.GetActiveCharacter();
        Vector3 newCharacterPos = character.transform.position;
        newCharacterPos.z = transform.position.z;
        newCharacterPos.x = transform.position.x;
        newCharacterPos += transform.forward * 1.8f;
        character.transform.position = newCharacterPos;
        Vector3 lookPosition = transform.position;
        lookPosition.y = newCharacterPos.y;
        character.transform.LookAt(lookPosition);


    }
    private void BeginConversation() {
        messageManager.ShowConversation(indexList);
    }

    void MoveUp() {
        iTween.MoveTo(gameObject, iTween.Hash("y", transform.position.y + 0.2f, "speed", floatSpeed,
            "easeType", iTween.EaseType.easeInOutSine, "oncomplete", "MoveDown"));
    }

    void MoveDown() {
        iTween.MoveTo(gameObject, iTween.Hash("y", basePosition.y, "speed", floatSpeed,
            "easeType", iTween.EaseType.easeInOutSine, "oncomplete", "MoveUp"));
    }
}