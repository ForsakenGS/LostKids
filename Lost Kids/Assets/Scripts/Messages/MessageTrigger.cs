using UnityEngine;
using System.Collections;

public class MessageTrigger : MonoBehaviour {

    private MessageManager messageManager;

    /// <summary>
    /// Tipo de salto de mensajes:
    /// Unique: Se muestra una vez y no se vuelve a mostrar
    /// Always: Se muestra todas las veces
    /// ShowTime: Una vez se muestre, se desactiva hasta que pase el tiempo introducido
    /// </summary>
    public enum MessageChecker { Unique, Always, ShowTime }
    public MessageChecker type;

    //Tiempo para que vuelva a activarse el trigger
    public float timeToShowAgain = 0;

    //Tiempo desde que se ha mostrado el último mensaje del trigger
    private float timeSinceShown = float.MaxValue;

    //Indice del trigger para acceder al mensaje
    public int index;

    //Indica si se ha mostrado ya el mensaje
    private bool messageShown;

    // Use this for initialization
    void Start () {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
        messageShown = false;
    }
	
    void OnTriggerEnter(Collider other) {
        //Si se trata del jugador activo
        if(CharacterManager.IsActiveCharacter(other.gameObject)) {
            //Si el tipo de trigger es único y no se ha mostrado el mensaje se llama a mostrar mensaje
            if(type.Equals(MessageChecker.Unique) && !messageShown) {
                messageShown = true;
                messageManager.ShowMessage(index);
            //Si no, si el tipo de trigger es por tiempo y el tiempo del trigger ha pasado se llama a mostrar mensaje y se actualiza el tiempo
            }else if(type.Equals(MessageChecker.ShowTime) && (Time.time - timeSinceShown >= timeToShowAgain) || !messageShown) {
                messageShown = true;
                timeSinceShown = Time.time;
                messageManager.ShowMessage(index);
            //Si no, si el tipo de trigger es infinito, se muestra el mensaje
            }else if(type.Equals(MessageChecker.Always)){
                messageManager.ShowMessage(index);
            }
        }
    }

}
