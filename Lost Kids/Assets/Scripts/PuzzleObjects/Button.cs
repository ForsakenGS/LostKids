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
    public float pushDeph;
    
    //Velocidad a la que se pulsa el boton
    public float pushSpeed;

    //Posicion de inicio del boton
    private Vector3 startPosition;
    //Posicion final cuando este pulsado
    private Vector3 endPosition;

    //Posicion auxiliar de comienzo cuando se activa/desactiva
    private Vector3 beginPosition;

    private bool isMoving = false;

    private bool inUse= false;

    private AudioLoader audioLoader;


    // Use this for initialization
    new void Start()
    {
        //Funcionalidad base para los usables
        base.Start();

        //Almacena posiciones iniciales y finales ( activado y desactivado )
        startPosition = this.transform.position;
        endPosition = this.transform.position - new Vector3(0, pushDeph, 0);

        audioLoader = GetComponent<AudioLoader>();

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
        if(CharacterManager.IsActiveCharacter(col.gameObject) || col.gameObject.tag.Equals("Pushable"))
        {
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
        if (CharacterManager.IsActiveCharacter(col.gameObject) || col.gameObject.tag.Equals("Pushable"))
        {
            if(isMoving || type.Equals(Usables.Hold))
            {
                CancelUse();
            }
        }
    }

    /// <summary>
    /// Metodo que se activa al usar el objeto. Incluye un comportamiento base generico
    /// y comportamiendo especifico para el objeto
    /// </summary>
    new public void Use()
    {
        if(!inUse) {
            
            inUse = true;

            //Comportamiento generico de un usable. (Activar objeto o notificar al puzzle segun situacion)
            base.Use();

            //Es necesario añadir funcionalidad adicional como Sonido o animaciones
            AudioManager.Play(audioLoader.GetSound("ButtonDown"), false, 1);
        }
    }


    /// <summary>
    /// Metodo que se activa al usar el objeto. Incluye un comportamiento base generico
    /// y comportamiendo especifico para el objeto
    /// </summary>
    new public void CancelUse()
    {
        inUse = false;

        AudioManager.Play(audioLoader.GetSound("ButtonUp"), false, 1);

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
        if(transform.position.Equals(endPosition))
        {
            Use();
        }
        yield return 0;
    }


}
