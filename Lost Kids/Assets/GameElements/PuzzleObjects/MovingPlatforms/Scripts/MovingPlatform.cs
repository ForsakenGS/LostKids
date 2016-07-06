using UnityEngine;
using System.Collections;
using System;

public class MovingPlatform : MonoBehaviour,IActivable {

    public enum speedModes {Constant,Custom};

   

    //public enum delayModes { Continuous,Extremes,Custom};

    public enum delayModes { Continuous, Custom }



    //Puntos que marcan el camino de la plataforma
    public GameObject[] points;

    //Variable que marca si el camino es un ciclo, o vuelve por el mismo camino
    public bool cyclePath;

    //Modo de velocidades entre puntos
    public speedModes speedMode;
    //Velocidad personalizada para cada punto
    public float[] pointSpeed;

    //Velocidad de movimiento unico
    public float moveSpeed;

    //Modo de parada entre puntos
    public delayModes delayMode;
    //Tiempo de parada en los puntos extremos
    //public float extremesDelay;

    //Tiempo de parada en cada punto
    public float[] pointDelay;

    //Variable que indica si la plataforma debe resetearse a su posicion inicial cuando se cancele su activacion
    public bool resetOnCancelation;

    //Posiciones de los puntos a seguir
    private Vector3[] path;
    //Posicion objetivo
    private Vector3 target;

    //Variable de control sobre el movimiento
    private bool isMoving;

    //Variable para activar/desactivar el movimiento
    public bool isActive;
 
    //Indice del punto objetivo
    private int currentNode;


    //Marca si la plataforma esta en el camino de vuelta
    private  bool returning;

    //Variable auxiliar para el movimiento
    private Vector3 beginPosition;
    
    //variable de estado de reseteo a su posicion inicial
    private bool resetting;

    // Use this for initialization
    void OnEnable()
    {

        //Se genera el camino de puntos
        path = new Vector3[points.Length];

        //Se guardan las posiciones de los puntos del camino
        for (int i = 0; i < points.Length; i++)
        {
            path[i] = points[i].transform.position;
        }

        //Se genera el array de velocidades
        if (speedMode.Equals(speedModes.Constant))
        {
            for (int i = 0; i < pointSpeed.Length; i++)
            {
                pointSpeed[i] = moveSpeed;
            }
        }

        //Se genera el array de paradas
        if (delayMode.Equals(delayModes.Continuous))
        {
            for (int i = 0; i < pointDelay.Length; i++)
            {
                pointDelay[i] = 0;
            }
        }

        if (points.Length > 0)
        { 
            currentNode = 1;
            target = path[currentNode];
            if (isActive || resetting)
            {
                StartCoroutine(Move(target));
            }
        }


    }
	
	// Update is called once per frame
	void Update () {

    }

    /// <summary>
    /// Corutina que mueve la plataforma hasta la siguiente posicion
    /// </summary>
    /// <param name="pos">Posicion final del objeto al terminar el movimiento</param>
    /// <returns></returns>
    public IEnumerator Move(Vector3 pos)
    {
        
        isMoving = true;
        beginPosition = transform.position;
        float t = 0;
        while (t < 1f) // Hasta que no acabe el frame no permite otro movimiento
        {
            t += Time.deltaTime * pointSpeed[currentNode];
            transform.position = Vector3.Lerp(beginPosition, pos, t); // interpola el movimiento entre dos puntos
            yield return null;
        }

        //Cuando llega a la posicion, se actualiza el objetivo con el siguiente nodo y se vuelve a lanzar la rutina
        isMoving = false;
        if (!resetting && pointDelay[currentNode] > 0)
        {
            yield return new WaitForSeconds(pointDelay[currentNode]);
        }

        UpdateTarget();
        if (isActive)
        {
            StartCoroutine(Move(target));
        }

    }

    /// <summary>
    /// Actualiza el objetivo y su posicion con el siguiente dependiendo de la direccion
    /// y modo de la plataforma
    /// </summary>
    void UpdateTarget()
    {
        //Avanzamos al siguiente nodo segun la direccion
        if(returning || resetting)
        {
            currentNode--;
        }
        else
        {
            currentNode++;
        }

        //Se corrige el nodo cuando el indice se salga del array
        if(currentNode>=path.Length)
        {
            if(cyclePath)
            {
                currentNode = 0;
            }
            else
            {
                returning = true;
                currentNode = path.Length - 2; //Si queremos que se detenga un momento en la posicion, poner -1
            }
        }
        else if(currentNode<0)
        {
            returning = false;
            currentNode = 1; //Si queremos que se detenga un momento en la posicion, poner a 0
            if(resetting)
            {
                resetting = false;
                StopAllCoroutines();
            }
        }

        //Se actualiza el objetivo con el nodo calculado
        target = path[currentNode];

    }

    /// <summary>
    /// Detecta al jugador y lo coloca como hijo de la plataforma para que siga su movimiento
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag=="Player")
        {
            if (col.transform.parent == null || col.transform.parent.GetComponent<MovingPlatform>()!=null)
            {
                col.transform.parent = this.transform;
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.parent = null;
        }
    }

    public void Activate()
    {
        isActive = true;
        resetting = false;
        if (!isMoving)
        {
            StartCoroutine(Move(target));
        }
    }

    /// <summary>
    /// 
    /// Cancela la activacion de la plataforma y la detiene. Si la plataforma debe resetearse,
    /// La vuelve a mover hasta su posicion inicial
    /// </summary>
    public void CancelActivation()
    {
        isActive = false;
        if (resetOnCancelation)
        {
            resetting = true;
            if (!isMoving)
            {
                UpdateTarget();
                StartCoroutine(Move(target));
            }
        }

    }

    /// <summary>
    /// Metodo que se llama desde el inspector cuando se modifica algun valor
    /// Se utiliza para mantener el mismo tamaño entre los puntos, sus velocidades, y sus delays
    /// </summary>
    void OnValidate()
    {
        if(pointDelay.Length!=points.Length)
        {
            pointDelay=copyAndResize(pointDelay, points.Length);
        }
        if(pointSpeed.Length!=points.Length)
        {
            pointSpeed=copyAndResize(pointSpeed, points.Length);
        }
    }


    /// <summary>
    /// Metodo auxiliar para redimensionar un array manteniendo su contenido
    /// </summary>
    /// <param name="array">array a dimensionar</param>
    /// <param name="size">nuevo tamaño</param>
    /// <returns></returns>
    private float[] copyAndResize(float[] array,int size)
    {
        float[] temp = new float[size];
        Array.Copy(array, temp,Math.Min(array.Length,size));
        array = temp;
        return array;

    }
}
