using UnityEngine;
using System.Collections.Generic;

public class Kodama : UsableObject {
    // Listado con índices de los mensajes a mostrar
    public List<int> indexList;

    // Referencia a MessageManager
    private MessageManager messageManager;

    private CutScene cutScene;
    // Use this for references
    void Awake() {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
        cutScene = GetComponent<CutScene>();
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
}