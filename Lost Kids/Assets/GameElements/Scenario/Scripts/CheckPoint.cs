using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckPoint : MonoBehaviour {

    //Lista de zonas donde aparecen los personajes al resucitar
    public List<GameObject> spawnZones;

    //Referencia al gato sobre el checkpoint
    private Neko neko;

    private ParticlesActivator vortex;

    //Habitacion en la que se encuentra
    public int room;

    public bool reached = false;
    //Flag de Checkpoint activo
    bool isActive;

    private AudioLoader audioLoader;

    private AudioSource checkPointSound;

    private HashSet<GameObject> charactersInside;


    void OnDisable()
    {
        CharacterStatus.KillCharacterEvent -= CharacterDied;
    }

    void Awake() {
        isActive = false;
        charactersInside = new HashSet<GameObject>();
        neko = GetComponentInChildren<Neko>();
        vortex = GetComponentInChildren<ParticlesActivator>();

    }

    // Use this for initialization
    void Start() {

        audioLoader = GetComponent<AudioLoader>();

        checkPointSound = audioLoader.GetSound("CheckPoint");
        if (!isActive)
        {
            vortex.Hide();
        }

    }

    /// <summary>
    /// Detecta al jugador para activar el checkpoint, notificando al manager y resucitando a los niños correspondientes
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col) {


        if (col.gameObject.CompareTag("Player")) {
            if (col.gameObject.GetComponent<CharacterStatus>().IsAvailable()) {
                //Se guarda el personaje en la lista de niños en el checkpoint y se subscribe al evento por si muere sin salir del trigger
                charactersInside.Add(col.gameObject);
                if(charactersInside.Count==1)
                {
                    CharacterStatus.KillCharacterEvent += CharacterDied;
                }

                if (reached) {
                    if (CharacterManager.GetActiveCheckPoint() == this)
                    {
                        CharacterManager.CheckPointActivation();
                    }
                } else {
                    reached = true;
                    Activate();
                    CharacterManager.CheckPointReached(this);
                }
            }

        }

    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //Si ya no quedan niños dentro del trigger, se elimina la subscripcion al evento
            charactersInside.Remove(col.gameObject);
            if(charactersInside.Count==0)
            {
                CharacterStatus.KillCharacterEvent -= CharacterDied;
            }
        }
    }

    /// <summary>
    /// Notificacion de que el alma del niño ha llegado al checkpoint
    /// Si  hay algun niño en vivo en el checkpoint, resucita al nuevo
    /// </summary>
    /// <param name="character"></param>
    void SoulReached(GameObject character)
    {
        if(charactersInside.Count>0)
        {
            character.GetComponent<CharacterStatus>().Ressurect();
            charactersInside.Add(character);
        }
    }

    void CharacterDied(GameObject character)
    {
        charactersInside.Remove(character);
    }

    public bool HasCharacterInside(GameObject character)
    {
        return charactersInside.Contains(character);
    }

    /// <summary>
    /// Activa el checkpoint y notifica el cambio al manager
    /// </summary>
    public void Activate() {
        if (!isActive) {
            isActive = true;

            if (checkPointSound) {
                AudioManager.Play(checkPointSound, false, 1);
            }

            //CharacterManager.SetActiveCheckPoint(this);
            neko.Show();
            neko.MIAU();
            vortex.Show();
        }
    }

    /// <summary>
    /// Activa el checkpoint sin que suene y notifica el cambio al manager
    /// </summary>
    public void ActivateMuted() {
        if (!isActive) {
            isActive = true;
            reached = true;
            CharacterManager.SetActiveCheckPoint(this);
            neko.Show();
            vortex.Show();
        }
    }

    /// <summary>
    /// Desactiva el checkpoint y oculta al gato
    /// </summary>
    public void Deactivate() {
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
    public Vector3 GetSpawnZone(int index) {
        return spawnZones[index].transform.position;
    }


}
