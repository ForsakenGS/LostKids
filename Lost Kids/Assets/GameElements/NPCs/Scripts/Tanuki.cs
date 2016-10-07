﻿using UnityEngine;
using System.Collections.Generic;

public class Tanuki : UsableObject {
    // Listado con índices de los mensajes a mostrar
    public List<int> askList;
    public List<int> thanksList;

    // Objeto requerido
    public InventoryObject requested;

    // Nombre del objeto requerido
    private string requestedObjectName;

    // Referencias
    private CutScene cutScene;
    private CutScene fearCutScene;
    private GameObject canvas;
    private MessageManager messageManager;

    // Use this for references
    void Awake() {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
        cutScene = GetComponent<CutScene>();
        fearCutScene = targets[0].GetComponent<CutScene>();
        canvas = GetComponentInChildren<Canvas>().gameObject;
        ChangeTooltipStatus(TooltipManager.On);
    }

    // Use this for initialization
    new void Start() {
        base.Start();
        type = UsableTypes.Instant;
        requestedObjectName = requested.GetComponent<InventoryObject>().objectName;
    }

    void BeginAskConversation() {
        BeginConversation(askList);
    }

    void BeginConversation(List<int> indexList) {
        messageManager.ShowConversation(indexList);
    }

    void BeginThanksConversation() {
        BeginConversation(thanksList);
    }

    // Muestra/oculta el tooltip del Tanuki
    void ChangeTooltipStatus(bool status) {
        canvas.SetActive(status);
    }

    void OnEnable() {
        TooltipManager.TooltipOnOff += ChangeTooltipStatus;
    }

    void OnDisable() {
        TooltipManager.TooltipOnOff -= ChangeTooltipStatus;
    }

    override public void Use() {
        // Comprueba si el personaje posee el objeto solicitado
        if (CharacterManager.GetActiveCharacter().GetComponent<CharacterInventory>().GetObject(requestedObjectName)) {
            // Eliminación del miedo
            if (fearCutScene == null) {
                base.Use();
            } else {
                fearCutScene.BeginCutScene(base.Use);
            }
            Invoke("ShowThanksConversation", fearCutScene.cutSceneTime + 0.5f);
        } else {
            // Muestra la conversación para pedir el objeto
            if (cutScene == null) {
                messageManager.ShowConversation(askList);
            } else {
                cutScene.BeginCutScene(BeginAskConversation);
                Invoke("MoveCharacterToFront", 0.5f);
            }
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

    void ShowThanksConversation() {
        // Muestra la conversación de agradecimiento
        if (cutScene == null) {
            messageManager.ShowConversation(thanksList);
        } else {
            cutScene.BeginCutScene(BeginThanksConversation);
            Invoke("MoveCharacterToFront", 0.5f);
        }
    }
}
