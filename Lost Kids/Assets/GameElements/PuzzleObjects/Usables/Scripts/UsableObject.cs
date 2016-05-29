using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tipo de objeto:
/// Instant: Se activa una unica vez y no se vuelve a desactivar
/// Hold: Se mantiene activo mientras el personaje lo este usando
/// Una vez activo, se desactiva cuando pase el tiempo introducido
/// </summary>
public enum UsableTypes { Instant, Hold, Timed }

/// <summary>
/// Clase abstracta que representa un objeto usable
/// Contiene variables y metodos comunes a cualquier objeto activable ( botones, palancas...)
/// Permite definir un tipo de comportamiento y referencias al objeto que activa 
/// o al puzzle manager, cuando forme parte de un puzzle mas complejo.
/// </summary>
public abstract class UsableObject : MonoBehaviour {

    //Objeto al que apunta el boton
    public List<GameObject> targets;


    public UsableTypes type;

    //Tiempo que se mantiene activo el objeto
    public float activeTime = 0;

    //Flag para saber si esta activado
    [HideInInspector]
    public bool onUse
    {
        get; set;
    }


    //Referencia al objeto que activa
    [HideInInspector]
    public List<IActivable> activables;

    //Indicador de si forma o no parte de un puzzle
    //Cuando el objeto forma parte de un puzzle, ignora su activable, 
    //y su funcion consiste en notificar al puzzle manager
    [HideInInspector]
    public bool inPuzzle
    {
        get; set;
    }

    //Indica cuando el jugador puede usar el objeto. Se debe poner a true
    //Cuando el jugador se encuentra dentro de la zona de activacion ( tooltipdetector)
    [HideInInspector]
    public bool canUse=false;

    //Referencia al manager del puzzle
    [HideInInspector]
    public PuzzleManagerBase puzzleManager
    {
        get; set;
    }
    
    /// <summary>
    /// Es necesario llamar a esta funcion desde los scripts que heredan mediante base.Start
    /// </summary>
    public void Start () {

        //Se guarda la referencia al script Activable
        foreach(GameObject target in targets)
        {
            activables.Add(target.GetComponent<IActivable>());
        }

        //Si no tiene detector de uso, se asume que se puede usar desde cualquier posicion
        if(GetComponentInChildren<TooltipDetector>()==null)
        {
            canUse = true;
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Activacion del objeto
    /// </summary>
    public virtual void  Use()
    {
        if (!onUse)
        {

            /*
            if (!type.Equals(UsableTypes.Instant))
            {
                onUse = true;
            }
            */
            onUse = true;

            //Si no forma parte de un puzzle, activa su objetivo
            if (!inPuzzle) {
                foreach(IActivable activable in activables) {
                    activable.Activate();
                }
            }
            //Si forma parte de un puzzle, notifica su activacion al manager
            else
            {
                puzzleManager.NotifyChange(this, true);
            }

            if (type.Equals(UsableTypes.Timed))
            {
                Invoke("CancelUse", activeTime);
            }


        }

    }

    /// <summary>
    /// Cancela la activacion del objeto
    /// </summary>
    public virtual void CancelUse()
    {
        if (onUse)
        {
            Debug.Log("Boton Desactivado");
            onUse = false;
            //Si no forma parte de un puzzle, desactiva su objetivo
            if (!inPuzzle)
            {
                foreach(IActivable activable in activables) {
                    activable.Activate();
                }
            }
            //Si forma parte de un puzzle, notifica su desactivacion al manager
            else
            {
                puzzleManager.NotifyChange(this, false);
            }
        }
    }

}
