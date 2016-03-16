using UnityEngine;
using System.Collections.Generic;

public class CharacterInventory : MonoBehaviour {
    /// <summary>
    /// Evento para informar del cambio en el inventorio
    /// </summary>
    public delegate void InventoryChanged(string obj);
    public static event InventoryChanged ObjectAddedEvent;
    public static event InventoryChanged ObjectRemovedEvent;
    public static event InventoryChanged ObjectRequestedEvent;

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
            if (ObjectAddedEvent != null) {
                ObjectAddedEvent(obj.objectName);
            }
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
            if (ObjectRemovedEvent != null) {
                ObjectRemovedEvent(objName);
            }
        } else {
            if (ObjectRequestedEvent != null) {
                ObjectRequestedEvent(objName);
            }
        }

        return res;
    }

    public bool HasObject (string objName) {
        return objects.Contains(objName);
    }
}
