using UnityEngine;
using System.Collections;

public class ObjectRequester : MonoBehaviour {
    // Objeto requerido
    public InventoryObject requested;

    // Nombre del objeto requerido
    private string requestedObjectName;

	// Use this for initialization
	void Start () {
        requestedObjectName = requested.GetComponent<InventoryObject>().objectName;
	}

    // Se lanza cuando el objeto entra en contacto con otro objeto
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Player")) {
            // Se trata del jugador, luego si tiene el objeto deseado, lo coge del inventario y realiza la acción determinada
            if (col.gameObject.GetComponent<CharacterInventory>().GetObject(requestedObjectName)) {
                // Realizar acción
                Debug.Log("Objeto entregado");
            }
        }
    }
}
