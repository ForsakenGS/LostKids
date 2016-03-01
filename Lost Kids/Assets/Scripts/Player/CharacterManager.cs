﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

    /// <summary>
    /// Evento para informar del cambio del personaje activo a otros scripts
    /// </summary>
    public delegate void CharacterChanged();
    public static event CharacterChanged ActiveCharacterChangedEvent;

    //lista de personajes
    public List<GameObject> characterList;

    //Checkpoint inicial
    public GameObject initialCheckPoint;

    //Checkpoint activo
    private CheckPoint activeCheckPoint;

    //Personaje activo
    private static GameObject activeCharacter;

    //Listado de estado de los personajes
    private List<CharacterStatus> characterStatusList;



    void Awake()
    {
        characterStatusList = new List<CharacterStatus>();
        activeCheckPoint = initialCheckPoint.GetComponent<CheckPoint>();
        for (int i = 0; i < characterList.Count; i++)
        {
            characterStatusList.Add(characterList[i].GetComponent<CharacterStatus>());
        }
    }
	// Use this for initialization
	void Start () {
       
        activeCheckPoint.Activate();
       
        ActivateCharacter(0);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Añade un nuevo personaje a la lista para su posterior control
    /// </summary>
    /// <param name="character"></param>
    public void addCharacter(GameObject character)
    {
        characterList.Add(character);
        characterStatusList.Add(character.GetComponent<CharacterStatus>());
    }

    /// <summary>
    /// Actualiza el checkpoint activo ,desactivando el que se encontraba activo hasta el momento
    /// </summary>
    /// <param name="cp"></param>
    public void SetActiveCheckPoint(CheckPoint cp)
    {
        activeCheckPoint.Deactivate();
        activeCheckPoint = cp;
    }

    /// <summary>
    /// Evento de activacion de un checkpoint
    /// Resucita a los niños que esten muertos en ese momento.
    /// </summary>
    public void CheckPointActivation()
    {
        for(int i=0;i<characterStatusList.Count;i++)
        {
            if(!characterStatusList[i].IsAlive())
            {
                characterStatusList[i].Ressurect();
            }
        }
    }

    /// <summary>
    /// Comprueba si el niño correspondiente al indice esta disponible para seleciconarlo
    /// </summary>
    /// <param name="index">Indice del personaje a comprobar</param>
    /// <returns>true si el personaje esta disponible</returns>
    public bool IsAvailable(int index)
    {
        return characterStatusList[index].IsAvailable();
    }

    /// <summary>
    /// Activa el personaje correspondiente al indice, siempre que este disponible
    /// Cambia la camara activa por la correspondiente a la posicion del jugador
    /// </summary>
    /// <param name="index">indice del personaje a activar</param>
    public void ActivateCharacter(int index)
    {
        if (IsAvailable(index))
        {
            activeCharacter = characterList[index];
            if (ActiveCharacterChangedEvent != null)
            {
                ActiveCharacterChangedEvent();
            }

            
        }
    }

    /// <summary>
    /// Notifica la muerte de uno de los personajes
    /// </summary>
    /// <param name="character">script del personaje muerto</param>
    public void CharacterKilled(CharacterStatus character)
    {
        int index = characterStatusList.IndexOf(character);
        characterList[index].transform.position = activeCheckPoint.GetSpawnZone(index);

        int nextIndex = NextAvailableCharacter();
        if(nextIndex!=-1)
        {
            ActivateCharacter(nextIndex);
        }
        else
        {
            ResetCheckPoint();
        }
    }

    /// <summary>
    /// Devuelve el indice del proximo personaje disponible
    /// Si no hay ninguno disponible, devuelve -1
    /// </summary>
    /// <returns>indice del proximo personaje disponible. -1 si no hay ninguno</returns>
    public int NextAvailableCharacter()
    {
        int newIndex = -1;

        for(int i=0;i<characterStatusList.Count;i++)
        {
            if(characterStatusList[i].IsAvailable())
            {
                newIndex = i;
            }
        }

        return newIndex;
    }

    /// <summary>
    /// Resetea los personajes en el checkpoint , resucitandolos y activando por defecto el primer personaje
    /// </summary>
    public void ResetCheckPoint()
    {
        for(int i=0; i<characterList.Count;i++)
        {
            characterStatusList[i].Ressurect();
            ActivateCharacter(0);
        }
    }

    /// <summary>
    /// Devuelve el gameobject correspondiente al personaje activo
    /// </summary>
    /// <returns>gameobject correspondiente al personaje activo</returns>
    public GameObject GetActiveCharacter()
    {
        return activeCharacter;
    }

    public static bool IsActiveCharacter(GameObject obj)
    {
        return obj.Equals(activeCharacter);
    }



}