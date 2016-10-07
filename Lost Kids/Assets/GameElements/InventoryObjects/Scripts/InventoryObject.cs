using UnityEngine;

public class InventoryObject : MonoBehaviour {
    // Nombre para identificar al objeto
    public string objectName;

	// Se lanza cuando el objeto entra en contacto con otro objeto
    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Player")) {
            // Se trata del jugador, luego se añade al inventario y, si ha sido posible, queda eliminado
            if (col.gameObject.GetComponent<CharacterInventory>().AddObject(this)) {
                gameObject.SetActive(false);
            }
        }
    }
}
