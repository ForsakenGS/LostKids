using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// Script que representa el comportamiento de un boton
/// Hereda gran parte de sus variables y funciones de la clase UsableObject
/// El boton detecta al jugador (MARCADO CON TAG 'Player') sobre el y se hunde a una determinada distancia, donde se considera activado
/// Cuando el jugador sale, si el tipo del boton es el adecuado, desactiva y devuelve el boton a su posicion inicial
/// </summary>
public class Button : UsableObject {


    //Profundidad a la quese hunde el boton
    public float pushDeph = 0.5f;
    
    //Velocidad a la que se pulsa el boton
    public float pushSpeed;

    //Posicion de inicio del boton
    private Vector3 startPosition;
    //Posicion final cuando este pulsado
    private Vector3 endPosition;
    private bool initialized = false;

    //Posicion auxiliar de comienzo cuando se activa/desactiva
    private Vector3 beginPosition;

    //Variable para estado de moviemiento
    private bool isMoving = false;

    private AudioSource buttonUpSound;
    private AudioSource buttonDownSound;

    void OnDisable()
    {
        initialized = false;
    }


    // Use this for initialization
    new void Start()
    {
        //Funcionalidad base para los usables
        base.Start();

        buttonUpSound = audioLoader.GetSound("ButtonUp");
        buttonDownSound = audioLoader.GetSound("ButtonDown");

    }

    // Update is called once per frame
    void Update () {
	
	}

    
    /// <summary>
    /// Detecta al jugador cuando se situa encima del boton para comenzar a moverlo y llegar a su activacion
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {


        if (!onUse && col.gameObject.tag.Equals("Player") && col.gameObject.transform.position.y > transform.position.y + 0.3f)
        {
            //Si se activa pro primera vez, guarda su posicion original
            if (!initialized)
            {
                initialized = true;
                startPosition = transform.position;
                endPosition = transform.position - new Vector3(0, pushDeph, 0);
            }

            StopAllCoroutines();

            StartCoroutine(Move(endPosition));

        }
    }

    /// <summary>
    /// Detecta al jugador al dejar la parte superior del objeto, y devuelve este a su posicion inicial
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerExit(Collider col)
    {
        if (CharacterManager.IsActiveCharacter(col.gameObject))
        {
            if(isMoving || type.Equals(UsableTypes.Hold))
            {
                CancelUse();
            }
        }
    }

    /// <summary>
    /// Detecta al jugador cuando se situa encima del boton para comenzar a moverlo y llegar a su activacion
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {

        if (!onUse && col.gameObject.tag.Equals("Player") && col.gameObject.transform.position.y > transform.position.y+0.3f)
        {
            //Si se activa pro primera vez, guarda su posicion original
            if (!initialized)
            {
                initialized = true;
                startPosition = transform.position;
                endPosition = transform.position - new Vector3(0, pushDeph, 0);
            }

            col.transform.parent = transform;
            StopAllCoroutines();

            StartCoroutine(Move(endPosition));

        }
    }

    /// <summary>
    /// Detecta al jugador al dejar la parte superior del objeto, y devuelve este a su posicion inicial
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            col.transform.parent = null;
            if (isMoving || type.Equals(UsableTypes.Hold))
            {
                CancelUse();
            }
        }
    }

    /// <summary>
    /// Metodo que se activa al usar el objeto. Incluye un comportamiento base generico
    /// y comportamiendo especifico para el objeto
    /// </summary>
    override public void Use()
    {

        if (!onUse) {
            
            //Comportamiento generico de un usable. (Activar objeto o notificar al puzzle segun situacion)
            base.Use();

            //Es necesario añadir funcionalidad adicional como Sonido o animaciones
            AudioManager.Play(buttonDownSound, false, 1);
        }
    }


    /// <summary>
    /// Metodo que se activa al usar el objeto. Incluye un comportamiento base generico
    /// y comportamiendo especifico para el objeto
    /// </summary>
    override public void CancelUse()
    {

        //Si se activa pro primera vez, guarda su posicion original
        if (!initialized)
        {
            initialized = true;
            startPosition = transform.position;
            endPosition = transform.position - new Vector3(0, pushDeph, 0);
        }
        AudioManager.Play(buttonUpSound, false, 1);

            //Comportamiento base generico para todos los objetos usables
            base.CancelUse();

            //Comportamiento especifico para cada objeto. Incluir ademas animaciones, sonidos...
            StopAllCoroutines();
            StartCoroutine(Move(startPosition));
        
    }

    /// <summary>
    /// Mueve el objeto hacia la posicion indicada. Si llega a la posicion final, se activa la funcion del boton
    /// </summary>
    /// <param name="pos">Posicion a la que se mueve el boton</param>
    /// <returns></returns>
    public IEnumerator Move(Vector3 pos)
    {
        isMoving = true;
        beginPosition = transform.position;
        float t = 0;

        while (t < 1f) // Hasta que no acabe el frame no permite otro movimiento
        {
            t += Time.deltaTime * pushSpeed;
            transform.position = Vector3.Lerp(beginPosition, pos, t); // interpola el movimiento entre dos puntos

            yield return null;
        }
        isMoving = false;
        if(transform.position.y<=endPosition.y+0.1f)
        {
            Use();
        }
        yield return 0;
    }


}
