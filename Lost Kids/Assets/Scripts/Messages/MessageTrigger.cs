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

    public float timeToShowAgain = 0;
    private float timeSinceShown = float.MaxValue;

    public int index;

    private bool messageShown;

    // Use this for initialization
    void Start () {
        messageManager = GameObject.FindGameObjectWithTag("MessageManager").GetComponent<MessageManager>();
        messageShown = false;
    }
	
    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            if(type.Equals(MessageChecker.Unique) && !messageShown) {
                messageShown = true;
                messageManager.ShowMessage(index);
            }else if(type.Equals(MessageChecker.ShowTime) && (Time.time - timeSinceShown >= timeToShowAgain) || !messageShown) {
                messageShown = true;
                timeSinceShown = Time.time;
                messageManager.ShowMessage(index);
            }else if(type.Equals(MessageChecker.Always)){
                messageManager.ShowMessage(index);
            }
        }
    }

}
