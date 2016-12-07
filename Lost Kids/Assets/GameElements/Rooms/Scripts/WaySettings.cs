using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script para controlar la configuración general de un pasillo entre dos habitaciones
/// </summary>
public class WaySettings : MonoBehaviour {
    /// <summary>
    /// Indica si en el camino debe aparecer un checkpoint o no
    /// </summary>
    public bool hasCheckpoint;
    /// <summary>
    /// Prefab del objeto <c>Checkpoint</c> a instanciar en el pasillo
    /// </summary>
    public GameObject checkpointPrefab;
    /// <summary>
    /// Indica si en el camino debe aparecer Kodama o no
    /// </summary>
    public bool hasKodama;
    /// <summary>
    /// Prefab del personaje <c>Kodama</c> a instanciar en el pasillo
    /// </summary>
    public GameObject kodamaPrefab;
    /// <summary>
    /// Indica los índices de la conversación con Kodama
    /// </summary>
    public List<int> kodamaConversation;
    /// <summary>
    /// Referencia a la <c>room</c> que precede 
    /// </summary>
    public RoomSettings prevRoom;
    /// <summary>
    /// Referencia a la <c>room</c> que se encuentra a continuación
    /// </summary>
    public RoomSettings nextRoom;

    private GameObject exit;

    // Use this for references & content generation
    void Awake() {
        // CheckPoint
        Transform trf = transform.FindChild("Checkpoint");
        if (hasCheckpoint) {
            // Escoge al azar posición donde instanciar
            Transform checkpointPos = trf.GetChild(Random.Range(0,trf.childCount));
            GameObject checkpoint = (GameObject) Instantiate(checkpointPrefab, checkpointPos.position, checkpointPos.rotation);
            // Separa el objeto instanciado de resto de posiciones posibles
            checkpoint.transform.parent = null;
        }
        if(trf) Destroy(trf.gameObject);
        // Kodama
        trf = transform.FindChild("Kodama");
        if (hasKodama) {
            // Escoge al azar posición donde instanciar
            Transform kodamaPos = trf.GetChild(Random.Range(0, trf.childCount));
            GameObject kodama = (GameObject) Instantiate(kodamaPrefab, kodamaPos.position, kodamaPos.rotation);
            // Separa el objeto instanciado de resto de posiciones posibles
            kodama.transform.parent = null;
            // Asigna la conversación a Kodama
            kodama.GetComponent<Kodama>().indexList = kodamaConversation;
        }

        if(trf) Destroy(trf.gameObject);        
        // Elementos de decoración

        // Referencias a componentes de la habitación
        exit = transform.FindChild("Exit").gameObject;
    }

    // Use this for initialization
    void Start () {
	
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
}
