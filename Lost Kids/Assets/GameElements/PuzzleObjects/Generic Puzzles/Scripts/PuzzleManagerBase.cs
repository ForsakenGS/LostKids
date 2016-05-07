using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Tipo de puzzle respectoa su activacion:
/// Instant: Se activa una vez permanentemente
/// Hold: Se mantiene activo mientras se cumpla una condicion
/// Timed: Se mantiene activo durante un tiempo.
/// </summary>
public enum puzzleType { Instant, Hold, Timed }

/// <summary>
/// Script base para la creaccion de puzzles que involucren varios objetos usables
/// Permite definir la lista de objetos que participan, el objeto que se activa como recompensa, 
/// y el comportamiento del puzzle una vez resuelto
/// CADA PUZZLE DEBE IMPLEMENTAR SU PROPIA LOGICA!!
/// </summary>
public abstract class PuzzleManagerBase : MonoBehaviour {

    //Lista de objetos que participan en el puzzle
    public List<GameObject> objectList;

    //Objeto que se activa como resultado
    public GameObject result;


    public puzzleType type;

    //Tiempo que se mantiene activo
    public float activeTime = 0;


    //Referencia a los scripts Usables
    [HideInInspector]
    public  List<UsableObject> usables;

    //Referencia al script Activable
    [HideInInspector]
    public IActivable targetActivable;

    //Estado del puzzle
    [HideInInspector]
    public bool solved;

    // Use this for initialization
    public void Start () {

        //Inicializacion del puzzle
        //Guarda las referencias a los scripts de los objetos
        //Y marca los objetos como parte del puzzle para que adapten su comportamiento
        UsableObject aux;
        foreach(GameObject o in objectList)
        {
            aux = o.GetComponent<UsableObject>();
            aux.puzzleManager = this;
            aux.inPuzzle = true;
            usables.Add(aux);           
        }
        targetActivable = result.GetComponent<IActivable>();

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Recibe la notificacion de un objeto con su estado de activacion 
    /// </summary>
    /// <param name="sender">Objeto notificador</param>
    /// <param name="status">estado de activacion del objeto</param>
    public abstract void NotifyChange(UsableObject sender, bool status);

    /// <summary>
    /// Activa la resolucion del puzzle. Cada caso implementa su propia logica
    /// </summary>
    public abstract void Solve();

    /// <summary>
    /// Resetea el estado del puzzle y sus elementos. Cada caso implementa su propia logica
    /// </summary>
    public abstract void Reset();

}
