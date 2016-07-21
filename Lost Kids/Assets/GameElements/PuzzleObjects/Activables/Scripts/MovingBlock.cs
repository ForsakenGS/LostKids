using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// Script para definir el comportamiento de una puerta activable mediante un mecanismo
/// Permite definir la direccion y velocidad de apertura
/// </summary>
public class MovingBlock : MonoBehaviour, IActivable {

    //Direccion de apertura de la puerta
    public enum Direction { Left, Right, Up, Down }
    public Direction openDirection;

    //Velocidad de apertura
    public float moveSpeed;

    public float moveDistance;

    //Variable que almacena el estado del bloque
    private bool onPosition = false;

    //Posicion inicial y final del bloque
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 offset = new Vector3(0, 0, 0);
    private bool initialized = false;

    private CutScene cutScene;
    private AudioSource moveSound;

    //Variable de control sobre el movimiento del bloque
    private bool isMoving
    {
        get; set;
    }

    //Variable auxiliar al movimiento del bloque
    private Vector3 beginPosition;

    // Use this for initialization
    void Start()
    {
        cutScene = GetComponent<CutScene>();
        moveSound = GetComponent<AudioSource>();
        //Se guarda la posicion inicial del bloque
        //startPosition = this.transform.position;
        //Se calcula un offset ( distancia a la que se movera) en funcion al tamaño y direccion de apertura
        
        
        switch (openDirection)
        {
            case Direction.Down:
                offset = new Vector3(0, -moveDistance, 0);
                break;
            case Direction.Up:
                offset = new Vector3(0, moveDistance, 0);
                break;
            case Direction.Left:
                offset = new Vector3(-moveDistance, 0, 0);
                break;
            case Direction.Right:
                offset = new Vector3(moveDistance, 0, 0);
                break;

        }

        //Se guara la posicion final del bloque 
        //endPosition = startPosition + offset;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginMove()
    {

        if (!onPosition)
        {
            if(moveSound!=null)
            {
                AudioManager.Play(moveSound, false, 1);
            }
            //Se cancela un movimiento previo y se mueve el bloque a su posicion final
            StopAllCoroutines();
            StartCoroutine(MoveBlock(endPosition));
        }

    }

    public void ResetBlock()
    {
        //Se cancela un movimiento previo y se mueve el bloque a su posicion inicial
        onPosition = false;
        StopAllCoroutines();
        StartCoroutine(MoveBlock(startPosition));

    }

    /// <summary>
    /// Corutina que mueve la puerta hasta una determinada posicion
    /// </summary>
    /// <param name="pos">Posicion final del objeto al terminar el movimiento</param>
    /// <returns></returns>
    public IEnumerator MoveBlock(Vector3 pos)
    {
        isMoving = true;
        beginPosition = transform.position;
        float t = 0;
        while (t < 1f) // Hasta que no acabe el frame no permite otro movimiento
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(beginPosition, pos, t); // interpola el movimiento entre dos puntos
            yield return null;
        }
        isMoving = false;
        if (transform.position.Equals(endPosition))
        {
            onPosition = true;
        }
        else if(transform.position.Equals(startPosition))
        {
            onPosition = false;
        }
        yield return 0;

    }

    /// <summary>
    /// Funcion de activacion general de objetos
    /// </summary>
    public void Activate()
    {
        //Si se activa pro primera vez, guarda su posicion original
        if (!initialized)
        {
            initialized = true;
            startPosition = transform.position;
            endPosition = startPosition + offset;
        }

        if (cutScene != null && (!cutScene.shown || cutScene.alwaysShow))
        {
            cutScene.BeginCutScene(BeginMove);

        }
        else {
            //Es necesario incluir el metodo dentro dentro de activate, para poder referenciar de manera generica al script
            BeginMove();
        }
        
    }

    /// <summary>
    /// Funcion de activacion general de objetos
    /// </summary>
    public void CancelActivation()
    {
        //Si se activa pro primera vez, guarda su posicion original
        if (!initialized)
        {
            initialized = true;
            startPosition = transform.position;
            endPosition = startPosition + offset;
        }
        //Es necesario incluir el metodo dentro dentro de activate, para poder referenciar de manera generica al script
        ResetBlock();
    }

    /// <summary>
    /// Cuando detecta al jugador, lo añade como hijo para que siga su movimiento
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.parent = this.transform;
        }
    }

    /// <summary>
    /// Cuando el jugadr abandona el bloque, deja de ser hijo del bloque
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.transform.parent = null;
        }
    }


}
