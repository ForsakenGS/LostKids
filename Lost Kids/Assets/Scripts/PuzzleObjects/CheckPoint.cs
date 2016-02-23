using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckPoint : MonoBehaviour {

    public GameObject characterStatusManager;
    private CharacterStatusManager statusManager;
    public List<GameObject> spawnZones;
    private Neko neko;

    public int room;

    bool isActive
    {
        set; get;
    }

    // Use this for initialization
    void Start()
    {
        isActive = false;
        if (characterStatusManager == null)
        {
            characterStatusManager = GameObject.FindGameObjectWithTag("CharacterStatusManager");
        }
        statusManager = characterStatusManager.GetComponent<CharacterStatusManager>();
        neko = GetComponentInChildren<Neko>();
        

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
                statusManager.CheckPointActivation();
            }

        }
        
    }

    /// <summary>
    /// Desactiva el checkpoint y notifica el cambio al manager
    /// </summary>
    public void Activate()
    {
        isActive = true;
        statusManager.SetActiveCheckPoint(this);
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
