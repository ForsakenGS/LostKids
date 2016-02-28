using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckPoint : MonoBehaviour {

    //Referencia al character manager
    public GameObject characterManagerPrefab;
    private CharacterManager characterManager;

    //Lista de zonas donde aparecen los personajes al resucitar
    public List<GameObject> spawnZones;

    //Referencia al gato sobre el checkpoint
    private Neko neko;

    //Habitacion en la que se encuentra
    public int room;

    //Flag de Checkpoint activo
    bool isActive;

    void Awake()
    {
        isActive = false;
        
        characterManager = characterManagerPrefab.GetComponent<CharacterManager>();
        neko = GetComponentInChildren<Neko>();
    }

    // Use this for initialization
    void Start()
    {
        if (characterManagerPrefab == null)
        {
            characterManagerPrefab = GameObject.FindGameObjectWithTag("CharacterManager");
            characterManager = characterManagerPrefab.GetComponent<CharacterManager>();
        }


    }

    // Update is called once per frame
    void Update () {
	
	}

    /// <summary>
    /// Detecta al jugador para activar el checkpoint, notificando al manager y resucitando a los niños correspondientes
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
        
        if (col.gameObject.CompareTag("Player"))
        {
            if (col.gameObject.GetComponent<CharacterStatus>().IsAvailable())
            {
                if (!isActive)
                {
                    Activate();
                }
                characterManager.CheckPointActivation();
            }

        }
        
    }

    /// <summary>
    /// Desactiva el checkpoint y notifica el cambio al manager
    /// </summary>
    public void Activate()
    {
        isActive = true;
        characterManager.SetActiveCheckPoint(this);
        neko.Show();
    }

    /// <summary>
    /// Desactiva el checkpoint y oculta al gato
    /// </summary>
    public void Deactivate()
    {
        isActive = false;
        neko.Hide();
    }

    /// <summary>
    /// 
    /// Devuelve la posicion del spawner del checkpoint correspondiente al index
    /// </summary>
    /// <param name="index">index del spawner dentro del checkpoint</param>
    /// <returns>posicion del spawner</returns>
    public Vector3 GetSpawnZone(int index)
    {
        return spawnZones[index].transform.position;
    }


}
