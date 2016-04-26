using UnityEngine;
using System.Collections.Generic;

public class Kodama : UsableObject {
    // Listado con índices de los mensajes a mostrar
    public List<int> indexList;

    // Referencia a MessageManager
    private MessageManager messageManager;

    // Use this for references
    void Awake() {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
    }

    /// <summary>
    /// Metodo que se activa al usar el objeto y muestra los mensajes en orden
    /// </summary>
    override public void Use() {
        // Muestra la conversación
        messageManager.ShowConversation(indexList);
    }
}