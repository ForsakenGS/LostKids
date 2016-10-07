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
    private List<InventoryObject> objects;

    // Use this for initialization
    void Start() {
        objects = new List<InventoryObject>();
    }

    /// <summary>
    /// Añade un objeto al inventario si aún no está lleno
    /// </summary>
    /// <param name="obj">Objeto a añadir</param>
    /// <returns></returns>
    public bool AddObject(InventoryObject obj) {
        bool res = (objects.Count < size);
        if (res) {
            objects.Add(obj);
            if (ObjectAddedEvent != null) {
                ObjectAddedEvent(obj.objectName);
            }
        }

        return res;
    }

    InventoryObject GetInventoryObject(string objName) {
        InventoryObject obj = null;
        // Busca el objeto en el listado
        int i = 0;
        while ((obj == null) && (i < objects.Count)) {
            if (objects[i].objectName.Equals(objName)) {
                obj = objects[i];
            }
            ++i;
        }

        return obj;
    }
    /// <summary>
    /// Obtiene del inventario el objeto solicitado
    /// </summary>
    /// <param name="obj">Objeto a retirar</param>
    /// <returns></returns>
    public bool GetObject(string objName) {
        InventoryObject invObject = GetInventoryObject(objName);
        bool res = (invObject != null);
        if (res) {
            objects.Remove(invObject);
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

    public bool HasObject(string objName) {
        return ((objects != null) && (GetInventoryObject(objName) != null));
    }

    public void SetEmptyToPosition(Vector3 pos) {
        bool res = false;
        // Mueve cada objeto a posición indicada
        foreach (InventoryObject obj in objects) {
            obj.transform.position = pos;
            res = true;
        }
        if (res) {
            SetEmpty();
        }
    }

    public void SetEmpty() {
        // Muestra cada objeto en su posición inicial
        foreach (InventoryObject obj in objects) {
            obj.gameObject.SetActive(true);
            obj.GetComponent<Rigidbody>().isKinematic = false;
            obj.GetComponent<Rigidbody>().useGravity = true;
            ObjectRemovedEvent(obj.objectName);
        }
        // Vacía el inventario
        objects.Clear();
    }
}
