using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckPoint : MonoBehaviour {

    //Lista de zonas donde aparecen los personajes al resucitar
    public List<GameObject> spawnZones;

    //Referencia al gato sobre el checkpoint
    private Neko neko;

    private VortexActivator vortex;

    //Habitacion en la que se encuentra
    public int room;

    //Flag de Checkpoint activo
    bool isActive;

    private AudioLoader audioLoader;

    private AudioSource checkPointSound;

    void Awake()
    {
        isActive = false;
        
        neko = GetComponentInChildren<Neko>();
        vortex = GetComponentInChildren<VortexActivator>();
    }

    // Use this for initialization
    void Start()
    {

        audioLoader = GetComponent<AudioLoader>();

        checkPointSound = audioLoader.GetSound("CheckPoint");


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
        //col.gameObject.CompareTag("Player")
        
        if (CharacterManager.IsActiveCharacter(col.gameObject))
        {
            if (col.gameObject.GetComponent<CharacterStatus>().IsAvailable())
            {
                if (!isActive)
                {
                    Activate();
                }
                CharacterManager.CheckPointActivation();
            }

        }
        
    }

    /// <summary>
    /// Activa el checkpoint y notifica el cambio al manager
    /// </summary>
    public void Activate()
    {
        if (!isActive) {
            isActive = true;

            if(checkPointSound) {
                AudioManager.Play(checkPointSound,false,1);
            }

            CharacterManager.SetActiveCheckPoint(this);
            neko.Show();
            vortex.Show();
        }
    }

/// <summary>
/// Activa el checkpoint sin que suene y notifica el cambio al manager
/// </summary>
public void ActivateMuted()
{
    if (!isActive)
    {
        isActive = true;

        CharacterManager.SetActiveCheckPoint(this);
        neko.Show();
        vortex.Show();
    }
}

    /// <summary>
    /// Desactiva el checkpoint y oculta al gato
    /// </summary>
    public void Deactivate()
    {
        isActive = false;
        neko.Hide();
        vortex.Hide();
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
