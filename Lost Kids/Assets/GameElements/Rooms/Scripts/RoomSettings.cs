using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script para controlar la configuración general de una habitación
/// </summary>
public class RoomSettings : MonoBehaviour {
    /// <summary>
    /// Tamaño de la habitación
    /// </summary>
    public enum RoomSizes { Small, Medium, Big };
    public RoomSizes size;
    /// <summary>
    /// Dificultad estimada de la habitación
    /// </summary>
    [Range(1, 10)]
    public int difficulty = 1;
    /// <summary>
    /// Etiquetas para definir las características de los puzzles que contiene la habitación
    /// </summary>
    public enum PuzzleTags {
        BigJump, Sprint, Push, Break, Telekinesis, Astral, Platforms,
        Sequence, Levers, Buttons, CanDie, DarkFear, WaterFear, TreeFear, Collectibles, Secret
    };
    public List<PuzzleTags> tags;

    private GameObject exit;
    private GameObject frontWall;
    private int objectsToPrepare;
    private bool prepared;

    // Use this for references & content generation
    void Awake() {
        // Generación automática de los muros

        // Generación automática de los elementos de decoración

        // Referencias a componentes de la habitación
        exit = transform.FindChild("Exit").gameObject;
        frontWall = transform.FindChild("Walls").FindChild("FrontWall").gameObject;
    }

    // Use this for initialization
    void Start() {
        objectsToPrepare = 0;
        prepared = false;
    }

    // Imita el efecto de caída libre sobre un elemento
    IEnumerator FreeFallEffect(Transform trf) {
        // Incrementa el número de elementos en preparación
        objectsToPrepare += 1;
        // Desactiva física para el objeto
        Rigidbody rb = trf.GetComponent<Rigidbody>();
        if ((rb != null) && (!rb.isKinematic)) {
            rb.isKinematic = true;
        } else {
            rb = null;
        }
        // Posición inicial
        Vector3 initialPos = trf.position;
        // Altura y velocidad de caída
        trf.Translate(new Vector3(0, Random.Range(15.0f, 25.0f), 0));
        float fallSpeed = 0.1f * Random.Range(1.0f, 2.0f);
        // Caída libre
        do {
            trf.Translate(new Vector3(0, -fallSpeed, 0));
            yield return new WaitForSeconds(0.01f);
        } while (Vector3.Distance(trf.position, initialPos) > 0.2f);
        trf.position = initialPos;
        // Reactiva física si procede
        if (rb != null) {
            rb.isKinematic = false;
        }
        // Decrementa el número de elementos que quedan por preparar
        objectsToPrepare -= 1;

        yield return 0;
    }

    /// <summary>
    /// Función para conocer la entrada a la habitación
    /// </summary>
    /// <returns><c>Vector3</c> con la posición de entrada</returns>
    public Vector3 GetEntry() {
        return transform.position;
    }

    /// <summary>
    /// Función para conocer la salida de la habitación
    /// </summary>
    /// <returns><c>Vector3</c> con la posición de salida</returns>
    public Vector3 GetExit() {
        return exit.transform.position;
    }

    /// <summary>
    /// Función a ejecutar cada vez que el personaje activo salga de la habitación
    /// </summary>
    public void HideRoom() {
        // Modifica la transparencia de la pared horizontal frontal
        frontWall.GetComponent<Renderer>().enabled = true;
    }

    // Se ejecuta al habilitar el script
    void OnEnable() {
        // Efecto de preparación de la habitación
        PrepareRoom();
    }

    /// <summary>
    /// Función a ejecutar la primera vez que se vea la habitación en pantalla, que crea un efecto visual para colocar 
    /// todos los objetos del puzzle
    /// </summary>
    public bool PrepareRoom() {
        bool res = !prepared;
        if (!prepared) {
            prepared = true;
            // Bloqueo al jugador
            InputManager.SetLock(true);
            // Bloques de las paredes
            foreach (Transform wall in transform.FindChild("Walls")) {
                StartCoroutine("FreeFallEffect", wall);
            }
            // Elementos del puzzle
            foreach (Transform wall in transform.FindChild("PuzzleElements")) {
                StartCoroutine("FreeFallEffect", wall);
            }
            // Elementos decorativos
            foreach (Transform wall in transform.FindChild("Decoration")) {
                StartCoroutine("FreeFallEffect", wall);
            }
            // Espera para que la habitación termine de estar preparada
            StartCoroutine("WaitEndOfPreparation");
        }

        return res;
    }

    /// <summary>
    /// Función a ejecutar cada vez que el personaje activo entre en la habitación
    /// </summary>
    public void ShowRoom() {
        // Modifica la transparencia de la pared horizontal frontal
        frontWall.GetComponent<Renderer>().enabled = false;
    }

    IEnumerator WaitEndOfPreparation() {
        yield return new WaitUntil(() => objectsToPrepare == 0);
        Debug.Log(objectsToPrepare);
        // Desbloqueo del jugador
        InputManager.SetLock(false);
        // Muestra la habitación
        ShowRoom();
    }
}