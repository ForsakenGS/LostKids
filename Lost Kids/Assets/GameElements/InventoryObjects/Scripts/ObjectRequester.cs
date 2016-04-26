using UnityEngine;
using System.Collections;

public class ObjectRequester : UsableObject {
    // Objeto requerido
    public InventoryObject requested;

    // Nombre del objeto requerido
    private string requestedObjectName;

    //public GameObject fearSource;

	// Use this for initialization
	void Start () {
        base.Start();
        type = Usables.Instant;
        requestedObjectName = requested.GetComponent<InventoryObject>().objectName;
	}


    override public void Use()
    {
        if(CharacterManager.GetActiveCharacter().GetComponent< CharacterInventory>().GetObject(requestedObjectName))
        {
            base.Use();
            //Sonido o lo que sea
        }
        else
        {
            //Activar mensaje pidiendo botella o algo asi?
        }
    }

    /*
    // Se lanza cuando el objeto entra en contacto con otro objeto
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Player")) {
            // Se trata del jugador, luego si tiene el objeto deseado, lo coge del inventario y realiza la acción determinada
            if (col.gameObject.GetComponent<CharacterInventory>().GetObject(requestedObjectName)) {
                // Realizar acción
                ObjectDelivered();
                Debug.Log("Objeto entregado");
            }
        }
    }
    */
    /*
    private void ObjectDelivered()
    {
        fearSource.GetComponent<IActivable>().Activate();
    }
    */
}
