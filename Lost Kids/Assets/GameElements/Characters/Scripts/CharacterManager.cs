using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

    /// <summary>
    /// Evento para informar del cambio del personaje activo a otros scripts
    /// </summary>
    public delegate void CharacterChanged(GameObject character);
    public static event CharacterChanged ActiveCharacterChangedEvent;

    //lista de personajes
    public List<GameObject> characterList;

    //lista publica de personajes
    public static List<GameObject> characters;

    //Checkpoint inicial
    public GameObject initialCheckPoint;

    //Velocidad de movimiento hacia el checkpoint
    public float resurrectionSpeed = 15f;

    //Checkpoint activo
    private static CheckPoint activeCheckPoint;

    //Personaje activo
    private static GameObject activeCharacter;

    //Listado de estado de los personajes
    private static List<CharacterStatus> characterStatusList;

    private AudioLoader audioLoader;

    private AudioSource changeCharacterSound;

    public static CharacterManager instance = null;

    void Awake() {
        instance = this;
        characterStatusList = new List<CharacterStatus>();
        characters = new List<GameObject>();
        activeCheckPoint = initialCheckPoint.GetComponent<CheckPoint>();
        for (int i = 0 ; i < characterList.Count ; i++) {
            characterStatusList.Add(characterList[i].GetComponent<CharacterStatus>());
            characters.Add(characterList[i]);
        }
        activeCharacter = characterList[0];
    }

    // Use this for initialization
    void Start() {

        audioLoader = GetComponent<AudioLoader>();

        changeCharacterSound = audioLoader.GetSound("ChangeCharacter");

        activeCheckPoint.ActivateMuted();

        activeCharacter = characterList[0];
        activeCharacter.GetComponent<CharacterStatus>().playerParticles.IntensifyColor();
        if (ActiveCharacterChangedEvent != null) {
            ActiveCharacterChangedEvent(activeCharacter);
        }
    }

    /// <summary>
    /// Añade un nuevo personaje a la lista para su posterior control
    /// </summary>
    /// <param name="character"></param>
    public void addCharacter(GameObject character) {
        characterList.Add(character);
        characterStatusList.Add(character.GetComponent<CharacterStatus>());
    }

    /// <summary>
    /// Actualiza el checkpoint activo ,desactivando el que se encontraba activo hasta el momento
    /// </summary>
    /// <param name="cp"></param>
    public static void SetActiveCheckPoint(CheckPoint cp) {
        if (activeCheckPoint != cp) {
            activeCheckPoint.Deactivate();
            activeCheckPoint = cp;
        }
    }

    /// <summary>
    /// Evento de activacion de un checkpoint
    /// Resucita a los niños que esten muertos en ese momento.
    /// </summary>
    public static void CheckPointActivation() {
        for (int i = 0 ; i < characterStatusList.Count ; i++) {
            if (!characterStatusList[i].IsAlive()) {
                //characterStatusList[i].Ressurect();
            }
        }
    }

    /// <summary>
    /// Metodo que se lanza cuando se llega a un nuevo checkpoint por primera vez
    /// </summary>
    /// <param name="check"></param>
    public static void CheckPointReached(CheckPoint check) {
        if (activeCheckPoint != check) {
            activeCheckPoint.Deactivate();
        }
        activeCheckPoint = check;
        for (int i = 0 ; i < characterStatusList.Count ; i++) {
            if (characterStatusList[i].gameObject != activeCharacter) {
                instance.characterList[i].transform.position = activeCheckPoint.GetSpawnZone(i);
                characterStatusList[i].currentRoom = activeCheckPoint.room;
                if (!characterStatusList[i].IsAlive()) {
                    characterStatusList[i].Ressurect();
                } else {
                    characterStatusList[i].ResetCharacter();
                }
            }
        }
    }

    /// <summary>
    /// Comprueba si el niño correspondiente al indice esta disponible para seleciconarlo
    /// </summary>
    /// <param name="index">Indice del personaje a comprobar</param>
    /// <returns>true si el personaje esta disponible</returns>
    public bool IsAvailable(int index) {
        return ((index < characterStatusList.Count) && (characterStatusList[index].IsAvailable()));
    }

    /// <summary>
    /// Activa el personaje correspondiente al indice, siempre que este disponible
    /// Cambia la camara activa por la correspondiente a la posicion del jugador
    /// </summary>
    /// <param name="index">indice del personaje a activar</param>
    public void ActivateCharacter(int index) {
        if ((IsAvailable(index)) && (!activeCharacter.Equals(characterList[index]))) {
            CharacterStatus activeCS = activeCharacter.GetComponent<CharacterStatus>();
            activeCharacter.GetComponent<AudioListener>().enabled = false;
            activeCS.playerParticles.AttenuateColor();
            // Si se trata de Akai y tiene activada PushAbility, se desactiva la habilidad para evitar que otro personaje pueda empujar
            if ((activeCS.characterName.Equals(CharacterName.Akai)) && (activeCS.CurrentStateIs(CharacterStatus.State.Pushing))) {
                activeCharacter.GetComponent<AbilityController>().DeactivateActiveAbility(false);
            }
            // Nuevo personaje activo
            activeCharacter = characterList[index];
            activeCharacter.GetComponent<AudioListener>().enabled = true;
            activeCharacter.GetComponent<CharacterStatus>().playerParticles.IntensifyColor();
            //activeCharacter.GetComponent<CharacterStatus>().currentRoom = activeCheckPoint.room;

            if (ActiveCharacterChangedEvent != null) {

                ActiveCharacterChangedEvent(activeCharacter);

                AudioManager.Play(changeCharacterSound, false, 1);

            }

            if (activeCheckPoint.HasCharacterInside(activeCharacter)) {
                CheckPointActivation();
            }

        }
    }

    /// <summary>
    /// Notifica la muerte de uno de los personajes
    /// </summary>
    /// <param name="character">script del personaje muerto</param>
    public void CharacterKilled(CharacterStatus character) {

        int index = characterStatusList.IndexOf(character);
        float distanceToCheckPoint = Vector3.Distance(character.transform.position, activeCheckPoint.GetSpawnZone(index));

        iTween.MoveTo(character.gameObject, iTween.Hash("position", activeCheckPoint.GetSpawnZone(index), "time", distanceToCheckPoint / resurrectionSpeed,
            "oncomplete", "ReviveCharacter", "oncompletetarget", gameObject, "oncompleteparams", index));
        if (GetAvailableCharacter() == -1) {
            CutSceneManager.FadeIn();
        }
        //characterList[index].transform.position = activeCheckPoint.GetSpawnZone(index);
        characterStatusList[index].currentRoom = activeCheckPoint.room;
        //Invoke("ActivateAvailableCharacter", distanceToCheckPoint / resurrectionSpeed);

    }

    public void ReviveCharacter(int index) {
        int nextIndex = GetAvailableCharacter();
        if (nextIndex != -1) {
            //ActivateCharacter(nextIndex);
            characterStatusList[index].Ressurect();
        } else {
            ResetCheckPoint();
        }
    }


    ///FUNCION ANTIGUA PARA BORRAR
    /// <summary>,"
    /// Activa el siguiente personaje disponible tras la muerte.
    /// Si no hay ninguno, resucita a los 3 en el ultimo checkpoint
    /// </summary>
    /*
    public void ActivateAvailableCharacter() {
        int nextIndex = GetAvailableCharacter();
        if (nextIndex != -1) {
            //ActivateCharacter(nextIndex);
            activeCharacter.GetComponent<CharacterStatus>().Ressurect();
        } else {
            ResetCheckPoint();
        }
    }
    */

    /// <summary>
    /// Devuelve el indice del proximo personaje disponible
    /// Si no hay ninguno disponible, devuelve -1
    /// </summary>
    /// <returns>indice del proximo personaje disponible. -1 si no hay ninguno</returns>
    public int GetAvailableCharacter() {
        int newIndex = -1;

        for (int i = 0 ; i < characterStatusList.Count ; i++) {
            if (characterStatusList[i].IsAvailable()) {
                newIndex = i;
            }
        }

        return newIndex;
    }

    /// <summary>
    /// Activa el siguiente personaje siempre que se encuentre alguno disponible
    /// </summary>
    public void ActivateNextCharacter() {
        if (characterList.Count == 1) {
            return;
        }
        int actualIndex = characterList.IndexOf(activeCharacter);
        int nextIndex = actualIndex + 1;
        if (nextIndex >= characterList.Count) {
            nextIndex = 0;
        }
        bool found = false;
        while (nextIndex != actualIndex && !found) {

            if (nextIndex != actualIndex && characterStatusList[nextIndex].IsAvailable()) {
                ActivateCharacter(nextIndex);
                found = true;
            }
            nextIndex++;
            if (nextIndex >= characterList.Count) {
                nextIndex = 0;
            }
        }
    }


    /// <summary>
    /// Activa el anterior personaje siempre que se encuentre alguno disponible
    /// </summary>
    public void ActivatePreviousCharacter() {
        if (characterList.Count == 1) {
            return;
        }
        int actualIndex = characterList.IndexOf(activeCharacter);
        int nextIndex = actualIndex - 1;
        if (nextIndex < 0) {
            nextIndex = characterList.Count - 1;
        }

        bool found = false;
        while (nextIndex != actualIndex && !found) {

            if (characterStatusList[nextIndex].IsAvailable()) {
                ActivateCharacter(nextIndex);
                found = true;
            }
            nextIndex--;
            if (nextIndex < 0) {
                nextIndex = characterList.Count - 1;
            }
        }
    }

    /// <summary>
    /// Resetea los personajes en el checkpoint , resucitandolos y activando por defecto el primer personaje
    /// </summary>
    public void ResetCheckPoint() {
        Invoke("RessurectAll", 0.5f);

    }

    /// <summary>
    /// Resucita todos los personajes y activa el primero
    /// </summary>
    public void RessurectAll() {
        CutSceneManager.FadeOut();
        //Se resucita al primer personaje por separado para que no se active el checkpoint
        for (int i = 1 ; i < characterList.Count ; i++) {
            characterStatusList[i].Invoke("Ressurect", i);

        }
        //Se resucita al primer personaje por separado para que no se active el checkpoint
        characterStatusList[0].Ressurect();
        ActivateCharacter(0);
    }

    /// <summary>
    /// Devuelve el gameobject correspondiente al personaje activo
    /// </summary>
    /// <returns>gameobject correspondiente al personaje activo</returns>
    public static GameObject GetActiveCharacter() {
        return activeCharacter;
    }

    public static CheckPoint GetActiveCheckPoint() {
        return activeCheckPoint;
    }

    /// <summary>
    /// Comprueba si el objeto introducido por parametro es el personaje activo
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsActiveCharacter(GameObject obj) {
        return obj.Equals(activeCharacter);
    }

    public static List<GameObject> GetCharacterList() {
        return characters;
    }



}
