using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Puzzle base de ejemplo
/// Hereda  variables y funcionalidades de la clase PuzzleManagerBase
/// Define la logica de puzzle basico, que requiere todos sus elementos activos al mismo tiempo
/// </summary>
public class PuzzleAllActive : PuzzleManagerBase {

    //Mapa del estado de los objetos ( activo/inactivo)
    Dictionary<UsableObject, bool> usablesState;

    private AudioLoader audioLoader;

    private AudioSource successSound;
    private AudioSource failSound;

    // Use this for initialization
    new void Start () {
        //Inicializacion basica del manager
        base.Start();

        audioLoader = GetComponent<AudioLoader>();
        successSound = audioLoader.GetSound("Success");
        failSound = audioLoader.GetSound("Fail");

        //Inicializacion del mapa de estado de objetos a false
        usablesState = new Dictionary<UsableObject, bool>();
        foreach(UsableObject o in usables)
        {
            usablesState.Add(o, o.onUse);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Recibe la notificacion de un objeto con su estado de activacion 
    /// </summary>
    /// <param name="sender">Objeto notificador</param>
    /// <param name="status">estado de activacion del objeto</param>
    public override void NotifyChange(UsableObject sender, bool status)
    {
        usablesState[sender] = status;

        //Si todos los elementos estan activos, se resuelve el puzzle
        if (AllActive())
        {
            Solve();
        }
        //Si el puzzle estaba resuelto, y algun elemento ha dejado de estar activo, se resetea
        else if(solved && type.Equals(puzzleType.Hold))
        {
            Reset();
        }

    }

    /// <summary>
    /// Comprueba si toda la lista de objetos estan activados
    /// </summary>
    /// <returns>true cuando todos los objetos estan activos</returns>
    bool AllActive()
    {
       
        bool active = true;
        foreach(bool value in usablesState.Values)
        {
            if(!value)
            {
                active = false;
            }
        }
        return active;
    }


    /// <summary>
    /// Metodo activa la resolucion del puzzle
    /// Activando el objeto resultado
    /// </summary>
    public override void Solve()
    {
        
        if (!solved)
        {
            AudioManager.Play(successSound, false, 1);

            solved = true;
            targetActivable.Activate();
            //Si se trata de un puzzle Timed, se resetea en un tiempo
            if (type.Equals(puzzleType.Timed))
            {
                Invoke("Reset", activeTime);
            }
        }
        
    }

    /// <summary>
    /// Reseteo del estado del puzzle
    /// </summary>
    public override void Reset()
    {
        //Si esta resuelto, cancela la activacion del resultado
        if(solved)
        {

            if(successSound.isPlaying) {
                AudioManager.Stop(successSound);
            }

            AudioManager.Play(failSound, false, 1);

            targetActivable.CancelActivation();
        }
        solved = false;

        //Resetea el estado de los objetos participantes
        foreach(UsableObject o in usables)
        {
            o.CancelUse();
        }

    }

}
