using UnityEngine;
using System.Collections.Generic;

public class RoomGeneration : MonoBehaviour {
    [Header("Decoration")]
    /// <summary>
    /// Listado de posiciones en las que se puede colocar elementos decorativos
    /// </summary>
    public List<Transform> positions;
    /// <summary>
    /// Listado de elementos decorativos a distribuir por la habitación
    /// </summary>
    public List<GameObject> randomProps;
    /// <summary>
    /// Indica si puede haber elementos repetidos
    /// </summary>
    public bool repeatedProps;
    [Header("References")]
    public Transform decoration;

    // Use this for content generation
    void Awake() {
        // Generación automática de los muros
        WallGeneration();
        // Generación automática de los elementos de decoración
        DecoGeneration();
        // Ordenación jerarquía de la escena
        //HierarchySorting();
    }

    // Use this for initialization
    void Start() {
        // Se desactiva la habitación una vez generada
        gameObject.SetActive(false);
    }

    void DecoGeneration() {
        // Distribución aleatoria de elementos
        foreach (Transform pos in positions) {
            // Elemento aleatorio
            int propIndex = Random.Range(0, randomProps.Count);
            GameObject prop = (GameObject) Instantiate(randomProps[propIndex], pos.position, pos.rotation);
            // Añadido a jerarquía de decoración
            prop.transform.parent = decoration;
            // Actualiza lista de elementos a distribuir
            if (!repeatedProps) {
                randomProps.RemoveAt(propIndex);
            }
        }
    }

    void HierarchySorting() {
        // Cambio de jerarquía de la decoración fija
        Transform fixedDecoration = decoration.Find("Fixed");
        foreach (Transform trf in fixedDecoration) {
            trf.parent = decoration;
        }
        // Eliminación jerarquías
        Destroy(fixedDecoration.gameObject);
        Destroy(decoration.Find("RandomPositions").gameObject);
    }

    void WallGeneration() {

    }
}