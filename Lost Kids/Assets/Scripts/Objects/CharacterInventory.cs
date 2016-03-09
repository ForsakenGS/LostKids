using UnityEngine;
using System.Collections.Generic;

public class CharacterInventory : MonoBehaviour {
    // Tamaño del inventario
    public int size = 1;

    // Objetos del inventario
    private List<string> objects;

	// Use this for initialization
	void Start () {
        objects = new List<string>();
	}
	
    /// <summary>
    /// Añade un objeto al inventario si aún no está lleno
    /// </summary>
    /// <param name="obj">Objeto a añadir</param>
    /// <returns></returns>
    public bool AddObject (InventoryObject obj) {
        bool res = (objects.Count < size);
        if (res) {
            objects.Add(obj.objectName);
        }

        return res;
    }

    /// <summary>
    /// Obtiene del inventario el objeto solicitado
    /// </summary>
    /// <param name="obj">Objeto a retirar</param>
    /// <returns></returns>
    public bool GetObject (string objName) {
        bool res = objects.Contains(objName);
        if (res) {
            objects.Remove(objName);
        }

        return res;
    }
}
