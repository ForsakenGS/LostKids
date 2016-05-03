﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Clase con la que que se realiza un control general sobre el comportamiento de cada personaje. Básicamente, implementa una
/// máquina de estados, las acciones a llevar a cabo durante los estados y las diferentes posibles transferencias.
/// </summary>
public class CharacterStatus : MonoBehaviour {
    /// <summary>
    /// Evento para informar del cambio en la vida del personaje
    /// </summary>
    public delegate void CharacterLifeChanged(GameObject character);
    public static event CharacterLifeChanged KillCharacterEvent;
    public static event CharacterLifeChanged ResurrectCharacterEvent;


    /// <summary>
    /// Clase para definir los diferentes estados en los que se puede encontrar un personaje
    /// </summary>
    public enum State { AstralProjection, Breaking, Crouching, Dead, BigJumping, Jumping, Pushing, Scared, Sprint, Standing, Telekinesis, Using }
    /// <summary>
    /// Clase para definir los diferentes personajes que pueden implementar una máquina de estados
    /// </summary>
    public enum CharacterName { Aoi, Akai, Murasaki };

    //Referencia al manager de personajes
    public GameObject characterManagerPrefab;
    private CharacterManager characterManager;
    private CharacterMovement characterMovement;
    private PlayerUse playerUse;

    public float astralSpeed = 0.0f;
    public float standingSpeed = 8000f;
    public float jumpImpulse = 200000f;
    public float jumpingSpeed = 5000f;
    public float crouchingSpeed = 4000f;
    public float pushingSpeed = 4000f;

    //Habitacion en la que se encuentra el personaje
    public int currentRoom = 0;

    // Personaje
    private State characterState;
    public State initialCharacterState;
    public CharacterName characterName;

    private AudioLoader audioLoader;

    private AudioSource resurrectSound;
    private AudioSource dieSound;

    // Use this for initialization
    void Awake() {
        if (characterManagerPrefab == null) {
            characterManagerPrefab = GameObject.FindGameObjectWithTag("CharacterManager");
        }
        characterManager = characterManagerPrefab.GetComponent<CharacterManager>();
        characterMovement = GetComponent<CharacterMovement>();
        playerUse = GetComponent<PlayerUse>();

    }

    // Use this for initialization
    void Start() {
        characterState = initialCharacterState;
        audioLoader = GetComponent<AudioLoader>();
        resurrectSound = audioLoader.GetSound("Resurrect");
        dieSound = audioLoader.GetSound("Die");

        //Se saca el objeto del padre para poder añadirlo como hijo a nuevos elementos
        transform.parent = null;
    }

    /// <summary>
    /// Comprueba si es posible comenzar la ejecución de una habilidad concreta, modificando el estado en que se encuentra el personaje
    /// </summary>
    /// <param name="ability">Habilidad que se desea iniciar a ejecutar</param>
    /// <returns><c>true</c> si es posible iniciar la habilidad, en cuyo caso modifica además el estado del personaje; <c>false</c> en otro caso</returns>
    public bool CanStartAbility(CharacterAbility ability) {
        bool res = characterState.Equals(State.Standing);
        if (res) {
            switch (ability.GetType().ToString()) {
                case "AstralProjectionAbility":
                    characterState = State.AstralProjection;
                    break;
                case "BigJumpAbility":
                    characterState = State.BigJumping;
                    break;
                case "BreakAbility":
                    characterState = State.Breaking;
                    break;
                case "PushAbility":
                    characterState = State.Pushing;
                    break;
                case "SprintAbility":
                    characterState = State.Sprint;
                    break;
                case "TelekinesisAbility":
                    characterState = State.Telekinesis;
                    break;
            }
        }

        return res;
    }

    // Update is called once per frame
    void Update() {
        switch (characterState) {
            case State.Jumping:
                if (characterMovement.CharacterIsGrounded()) {
                    characterState = State.Standing;
                }
                break;
            case State.BigJumping:
                if (characterMovement.CharacterIsGrounded()) {
                    GetComponent<AbilityController>().UseAbility();
                    characterState = State.Standing;
                }
                break;
            case State.Pushing: //TEMPORAL!! SUelta el objeto si el personaje se cae
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Jumping;
                    GetComponent<PushAbility>().ReleaseObject();
                }
                break;
            case State.Crouching:
                if (!characterMovement.CharacterIsGrounded()) {
                    characterMovement.Stand();
                    characterState = State.Jumping;
                }
                break;
            case State.Dead:
            case State.AstralProjection:
                break;
            default:
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Jumping;
                }
                break;
        }
    }

    void FixedUpdate() {
        // Aplica gravedad extra sobre el personaje si se encuentra en el aire
        if (characterState.Equals(State.BigJumping) || characterState.Equals(State.Jumping)) {
            characterMovement.ExtraGravity();
        }
    }

    /// <summary>
    /// Mata al personaje y notifica su nuevo estado al manager, que desactivara su control y lo movera al checkpoint
    /// </summary>
    public void Kill() {

        switch (characterState) {
            case State.AstralProjection:
                // La habilidad debe terminar su ejecución
                GetComponent<AbilityController>().UseAbility();
                break;
            case State.Dead:
                break;
            default:
                characterState = State.Dead;
                //Animacion, Efectos, Cambio de imagen.....
                AudioManager.Play(dieSound, false, 1);
                //Reinicia el transform en caso de morir estando subido a una plataforma
                transform.parent = null;
                //Si muere mientras empuja un objeto, lo debe soltar
                if (GetComponent<PushAbility>() != null) {
                    GetComponent<PushAbility>().ReleaseObject();
                }
                GetComponent<Renderer>().enabled = false; //Temporal
                GetComponent<Rigidbody>().isKinematic = true;
                if (KillCharacterEvent != null) {
                    KillCharacterEvent(gameObject);
                }
                characterManager.CharacterKilled(this);
                break;

        }
    }

    /// <summary>
    /// Función para indicar que el botón de salto ha sido pulsado, cambiando el estado del personaje a 'Jumping' en caso de ser necesario.
    /// </summary>
	public void JumpButton() {
        switch (characterState) {
            case State.Standing:
                characterState = State.Jumping;
                characterMovement.Jump(jumpImpulse);
                break;
        }
    }

    /// <summary>
    /// Función para indicar que el botón de uso ha sido pulsado, cambiando el estado del personaje a 'Standing' si el objeto usado es instantáneo, o a 'Using' en caso de que el objeto requiera ser mantenido para su uso.
    /// </summary>
	public void UseButton() {
        switch (characterState) {
            case State.Standing:
                if (playerUse.Use()) {
                    characterState = State.Using;
                }
                break;
            case State.AstralProjection:
                playerUse.Use();
                break;
            case State.Using:
                playerUse.StopUsing();
                characterState = State.Standing;
                break;
        }
    }

    /// <summary>
    /// Función para indicar que el botón de agacharse ha sido pulsado, cambiando el estado del personaje a 'Crouching' en caso de ser necesario.
    /// </summary>
	public void CrouchButton() {
        switch (characterState) {
            case State.Crouching:
                characterMovement.Stand();
                characterState = State.Standing;
                break;
            case State.Standing:
                characterMovement.Crouch();
                characterState = State.Crouching;
                break;
        }
    }

    /// <summary>
    /// Función para indicar que alguno de los botones de movimiento ha sido pulsado, realizando el correspondiende movimiento y 
    /// variando la velocidad en función del estado en que se encuentre el personaje.
    /// </summary>
    /// <param name="horizontal">Movimiento sobre el eje X a aplicar sobre el personaje</param>
    /// <param name="vertical">Movimiento sobre el eje Z a aplicar sobre el personaje</param>
    public void MovementButtons(float horizontal, float vertical) {
        switch (characterState) {
            case State.Standing:
            case State.Sprint:
                characterMovement.MoveCharacterNormal(horizontal, vertical, standingSpeed);
                break;
            case State.Jumping:
            case State.BigJumping:
                characterMovement.MoveCharacterNormal(horizontal, vertical, jumpingSpeed);
                break;
            case State.AstralProjection:
                characterMovement.MoveCharacterNormal(horizontal, vertical, astralSpeed);
                break;
            case State.Crouching:
                characterMovement.MoveCharacterNormal(horizontal, vertical, crouchingSpeed);
                break;
            case State.Pushing:
                characterMovement.MoveCharacterAxes(horizontal, vertical, pushingSpeed, GetComponent<PushAbility>().GetPushNormal());
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ability">Habilidad cuya ejecución se desea finalizar</param>
    public void EndAbility(CharacterAbility ability) {
        switch (ability.GetType().ToString()) {
            case "AstralProjectionAbility":
            case "BreakAbility":
            case "PushAbility":
            case "SprintAbility":
            case "TelekinesisAbility":
                characterState = State.Standing;
                break;
        }
    }

    /// <summary>
    /// Resucita al personaje
    /// </summary>
    public void Ressurect() {
        characterState = State.Standing;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        if (ResurrectCharacterEvent != null) {
            ResurrectCharacterEvent(gameObject);
        }
        //Animacion, Efectos, Cambio de imagen.....
        AudioManager.Play(resurrectSound, false, 1);
    }

    /// <summary>
    /// Devuelve true si el personaje esta disponible para su manejo. Puede encontrarse indisponible si esta muerto, o asustado
    /// </summary>
    /// <returns></returns>
    public bool IsAvailable() {
        return !(characterState.Equals(State.Dead) || characterState.Equals(State.Scared));
    }

    /// <summary>
    /// Devuelve true si el personaje esta vivo
    /// </summary>
    /// <returns></returns>
    public bool IsAlive() {
        return !characterState.Equals(State.Dead);
    }

    /// <summary>
    /// Cambia el estado asustado del personaje
    /// </summary>
    /// <param name="scared">true para ponerlo asustado, false para que deje de estarlo</param>
    public void SetScared(bool scared)
    {
        if(scared)
        {
            characterState = State.Scared;
        }
        else if(characterState.Equals(State.Scared))
        {
            characterState = State.Standing;
        }
    }
}