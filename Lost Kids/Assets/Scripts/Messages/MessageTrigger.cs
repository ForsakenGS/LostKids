using UnityEngine;
using System.Collections.Generic;

public class MessageTrigger : MonoBehaviour {
    /// <summary>
    /// Tipo de salto de mensajes:
    /// Unique: Se muestra una vez y no se vuelve a mostrar
    /// Always: Se muestra todas las veces
    /// ShowTime: Una vez se muestre, se desactiva hasta que pase el tiempo introducido
    /// </summary>
    public enum MessageChecker { Unique, Always, ShowTime }
    public MessageChecker type;

    /// <summary>
    /// Listado con índices de los mensajes a mostrar
    /// </summary>
    public List<int> indexList;
    /// <summary>
    /// Indica si se cumplen las condiciones para que los mensajes puedan ser mostrados
    /// </summary>
    public bool messagesCanBeShown = true;
    /// <summary>
    /// Tiempo hasta que vuelva a activarse el trigger
    /// </summary>
    public float timeToShowAgain = 0;

    
    // Referencia a MessageManager
    private MessageManager messageManager;
    // Indica si se ha mostrado ya el mensaje
    private bool messageShown;
    // Tiempo desde que se ha mostrado el último mensaje del trigger
    private float timeSinceShown;

    // Use this for references
    void Awake() {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
    }

    // Use this for initialization
    void Start() {
        messageShown = false;
        timeSinceShown = float.MaxValue;
    }

    void OnTriggerEnter(Collider other) {
        // Muestra el mensaje si no requiere el botón de acción y se trata del jugador activo
        if ((messagesCanBeShown) && (CharacterManager.IsActiveCharacter(other.gameObject))) {
            ShowMessages();
        }
    }

    void ShowMessages() {
        // Si el tipo de trigger es único y no se ha mostrado el mensaje se llama a mostrar mensaje
        if (type.Equals(MessageChecker.Unique) && !messageShown) {
            messageShown = true;
            messageManager.ShowConversation(indexList);
            // Si no, si el tipo de trigger es por tiempo y el tiempo del trigger ha pasado se llama a mostrar mensaje y se actualiza el tiempo
        } else if (type.Equals(MessageChecker.ShowTime) && (Time.time - timeSinceShown >= timeToShowAgain) || !messageShown) {
            messageShown = true;
            timeSinceShown = Time.time;
            messageManager.ShowConversation(indexList);
            // Si no, si el tipo de trigger es infinito, se muestra el mensaje
        } else if (type.Equals(MessageChecker.Always)) {
            messageManager.ShowConversation(indexList);
        }
    }
}