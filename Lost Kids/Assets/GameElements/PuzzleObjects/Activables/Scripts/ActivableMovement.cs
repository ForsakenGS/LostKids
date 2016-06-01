﻿using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// Script para definir el comportamiento de una puerta activable mediante un mecanismo
/// Permite definir la direccion y velocidad de apertura
/// </summary>
public class ActivableMovement : MonoBehaviour, IActivable {

    //Direccion de apertura de la puerta
    public enum Direction { Left, Right, Up, Down }
    public Direction openDirection;

    //Velocidad de apertura
    public float openSpeed;

    //Variable que alamcena el estado de la puerta
    private bool isOpen = false;

    //Distancia que se mueve la puerta al abrirse ( no se usa de momento)
    //private float openDistance = 0;

    //Posicion inicial y final de la puerta ( cerrada / abierta )
    private Vector3 startPosition;
    private Vector3 endPosition;

    //Variable de control sobre el movimiento de la puerta
    private bool isMoving
    {
        get; set;
    }

    //Variable auxiliar al movimiento de la puerta
    private Vector3 beginPosition;


    private AudioLoader audioLoader;

    private AudioSource activationSound;
    private AudioSource deActivationSound;

    public bool hasFadeCutScene;
    private ActivableFade activableFade;
    public GameObject cutSceneCamera;

    public void OnEnable() {
        if(hasFadeCutScene) {
            ActivableFade.FadeInOutEvent += OpenDoorCutScene;
        }
        
    }

    public void OnDisable() {
        if(hasFadeCutScene) {
            ActivableFade.FadeInOutEvent -= OpenDoorCutScene;
        }
        
    }

    // Use this for initialization
    void Start()
    {
        //Si se va a mostrar un cutscene
        if(hasFadeCutScene) {
            activableFade = GetComponent<ActivableFade>();
        }

        audioLoader = GetComponent<AudioLoader>();
        if(audioLoader) {
            activationSound = audioLoader.GetSound("Activation");
            deActivationSound = audioLoader.GetSound("Deactivation");

        }

        //Se guarda la posicion inicial de la puerta
        startPosition = this.transform.position;
        //Se calcula un offset ( distancia a la que se movera) en funcion al tamaño y direccion de apertura
        Vector3 size = transform.localScale;//GetComponent<Renderer>().bounds.size;
        Vector3 offset = new Vector3(0, 0, 0);
        switch (openDirection)
        {
            case Direction.Down:
                offset = new Vector3(0, -size.y, 0);
                break;
            case Direction.Up:
                offset = new Vector3(0, size.y, 0);
                break;
            case Direction.Left:
                offset = new Vector3(-size.z, 0, 0);
                break;
            case Direction.Right:
                offset = new Vector3(size.z, 0, 0);
                break;

        }

        //Se guara la posicion final de la puerta cuando este abierta
        endPosition = startPosition + offset;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveObject()
    {

        if (!isOpen)
        {
            AudioManager.Play(activationSound, false, 1);

            //Se cancela un movimiento previo y se mueve la puerta a su posicion de apertura
            StopAllCoroutines();
            StartCoroutine(DoingMovement(endPosition));

        }

    }

    public void DisableMovement()
    {
        if(activationSound.isPlaying) {
            AudioManager.Stop(activationSound);
        }
        AudioManager.Play(activationSound, false, 1);

        //Se cancela un movimiento previo y se mueve la puerta a su posicion de cierre
        isOpen = false;
        StopAllCoroutines();
        StartCoroutine(DoingMovement(startPosition));

    }

    /// <summary>
    /// Corutina que mueve la puerta hasta una determinada posicion
    /// </summary>
    /// <param name="pos">Posicion final del objeto al terminar el movimiento</param>
    /// <returns></returns>
    public IEnumerator DoingMovement(Vector3 pos)
    {
        isMoving = true;
        beginPosition = transform.position;
        
        float t = 0;
        while (t < 1f) // Hasta que no acabe el frame no permite otro movimiento
        {
            t += Time.deltaTime * openSpeed;
            transform.position = Vector3.Lerp(beginPosition, pos, t); // interpola el movimiento entre dos puntos
            yield return null;
        }
        //Al terminar de moverse actualizamos el estado de la puerta, comprobando en que posicion esta
        //Final = abierta , Inicial = cerrada
        isMoving = false;
        if (transform.position.Equals(endPosition))
        {
            isOpen = true;
        }
        else if(transform.position.Equals(startPosition))
        {
            isOpen = false;
        }

        
        yield return 0;

    }

    /// <summary>
    /// Funcion de activacion general de objetos
    /// </summary>
    public void Activate()
    {
        if(hasFadeCutScene) {

            activableFade.StartCutScene(cutSceneCamera);

        }else {
            //Es necesario incluir el metodo dentro dentro de activate, para poder referenciar de manera generica al script
            MoveObject();
        }
        
    }

    public void OpenDoorCutScene() {
        MoveObject();
    }

    /// <summary>
    /// Funcion de activacion general de objetos
    /// </summary>
    public void CancelActivation()
    {
        //Es necesario incluir el metodo dentro dentro de activate, para poder referenciar de manera generica al script
        DisableMovement();
    }


}