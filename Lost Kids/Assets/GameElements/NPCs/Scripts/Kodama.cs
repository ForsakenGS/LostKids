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

    
    // Use this for references
    void Awake() {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
        cutScene = GetComponent<CutScene>();
        basePosition = transform.position;
        
    }

    void OnEnable()
    {
        MoveUp();
    }

    void OnDisable()
    {
        transform.position = basePosition;
        iTween.Stop(gameObject);
    }

    /// <summary>
    /// Metodo que se activa al usar el objeto y muestra los mensajes en orden
    /// </summary>
    override public void Use() {
        // Muestra la conversación
        if (cutScene == null)
        {
            messageManager.ShowConversation(indexList);
        }
        else
        {
            cutScene.BeginCutScene(BeginConversation);
        }
    }

    private void BeginConversation()
    {
        messageManager.ShowConversation(indexList);
    }

    void MoveUp()
    {
        iTween.MoveTo(gameObject, iTween.Hash("y" ,transform.position.y+0.2f, "speed",floatSpeed,
            "easeType",iTween.EaseType.easeInOutSine,"oncomplete","MoveDown"));
    }

    void MoveDown()
    {
        iTween.MoveTo(gameObject, iTween.Hash("y", basePosition.y, "speed",floatSpeed,
            "easeType", iTween.EaseType.easeInOutSine, "oncomplete", "MoveUp"));
    }
}