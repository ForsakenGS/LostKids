using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Tamaños de habitación
public enum RoomSizes { Small, Medium, Big };
// Etiquetas para la habitación
public enum PuzzleTags {
    BigJump, Sprint, Push, Break, Telekinesis, Astral, Platforms,
    Sequence, Levers, Buttons, CanDie, DarkFear, WaterFear, TreeFear, Collectibles, Secret
};
/// <summary>
/// Script para controlar la configuración general de una habitación
/// </summary>
public class RoomSettings : MonoBehaviour {
    [Header("Room Configuration")]
    /// <summary>
    /// Tamaño de la habitación
    /// </summary>
    public RoomSizes size;
    /// <summary>
    /// Dificultad estimada de la habitación
    /// </summary>
    [Range(1, 10)]
    public int difficulty = 1;
    /// <summary>
    /// Etiquetas para definir las características de los puzzles que contiene la habitación
    /// </summary>
    public List<PuzzleTags> tags;
    public List<CharacterName> requiredCharacters;
    [Header("References")]
    public Transform decoration;
    public Transform exit;
    public Transform puzzleElements;
    public Transform walls;

    private GameObject frontWall;
    private int objectsToPrepare;
    private bool prepared;

    // Use this for references & content generation
    void Awake() {
        objectsToPrepare = 0;
        // Referencias a componentes de la habitación
        decoration = transform.Find("Decoration");
        exit = transform.Find("Exit");
        puzzleElements = transform.Find("PuzzleElements");
        walls = transform.Find("Walls");

        frontWall = walls.Find("FrontWall").gameObject;
        
    }

    // Use this for initialization
    void Start() {
        prepared = false;
    }

    // Imita el efecto de caída libre sobre un elemento
    IEnumerator FreeFallEffect(Transform trf) {
        // Incrementa el número de elementos en preparación
        objectsToPrepare += 1;
        //ChangeValue(1);
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
        //ChangeValue(-1);

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
        return exit.position;
    }

    /// <summary>
    /// Función a ejecutar cada vez que el personaje activo salga de la habitación
    /// </summary>
    public void HideRoom() {
        // Modifica la transparencia de la pared horizontal frontal
        //frontWall.GetComponent<Renderer>().enabled = true;
        foreach(Renderer r in frontWall.GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
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
            foreach (Transform wall in walls) {
                //objectsToPrepare += 1;
                StartCoroutine("FreeFallEffect", wall);
            }
            // Elementos del puzzle
            foreach (Transform wall in puzzleElements) {
                //objectsToPrepare += 1;
                StartCoroutine("FreeFallEffect", wall);
            }
            // Elementos decorativos
            foreach (Transform wall in decoration) {
                //objectsToPrepare += 1;
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
        //frontWall.GetComponent<Renderer>().enabled = false;
        foreach (Renderer r in frontWall.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
    }

    IEnumerator WaitEndOfPreparation() {
        yield return new WaitUntil(() => objectsToPrepare == 0);
        // Desbloqueo del jugador
        InputManager.SetLock(false);
        // Muestra la habitación
        ShowRoom();
    }

    private readonly object locker = new object();
    void ChangeValue(int val) {
        lock (locker) {
            objectsToPrepare += val;
        }
    }
}