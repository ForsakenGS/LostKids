using UnityEngine;
using System.Collections;

public class CharacterStatus : MonoBehaviour {

    //Referencia al manager de personajes
    public GameObject characterManagerPrefab;
    private CharacterManager characterManager;

    //Habitacion en la que se encuentra el personaje
    public int currentRoom = 0;

    //Estado del personaje
    private bool isAlive;


    // Use this for initialization
    void Awake()
    {
        if (characterManagerPrefab == null)
        {
            characterManagerPrefab = GameObject.FindGameObjectWithTag("CharacterManager");
        }
        characterManager = characterManagerPrefab.GetComponent<CharacterManager>();
    }

	// Use this for initialization
	void Start () {
        isAlive = true;


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Mata al personaje y notifica su nuevo estado al manager, que desactivara su control y lo movera al checkpoint
    /// </summary>
    public void Kill()
    {
        Debug.Log("Muerto");
        isAlive = false;
        GetComponent<Renderer>().enabled = false;
        //Animacion, Efectos, Cambio de imagen.....
        GetComponent<Renderer>().enabled = false; //Temporal
        characterManager.CharacterKilled(this);    
    }

    /// <summary>
    /// Resucita al personaje
    /// </summary>
    public void Ressurect()
    {
        isAlive = true;
        GetComponent<Renderer>().enabled = true;
        //Animacion, Efectos, Cambio de imagen.....
    }

    /// <summary>
    /// Devuelve true si el personaje esta disponible para su manejo
    /// Puede encontrarse indisponible si esta muerto, o asustado
    /// </summary>
    /// <returns></returns>
    public bool IsAvailable()
    {
        return isAlive;
    }

    /// <summary>
    /// Devuelve true si el personaje esta vivo
    /// </summary>
    /// <returns></returns>
    public bool IsAlive()
    {
        return isAlive;
    }

}
