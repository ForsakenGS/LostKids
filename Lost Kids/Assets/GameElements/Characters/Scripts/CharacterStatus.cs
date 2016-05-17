﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Clase para definir los diferentes personajes que pueden implementar una máquina de estados
/// </summary>
public enum CharacterName { Aoi, Akai, Ki }

/// <summary>
/// Clase con la que que se realiza un control general sobre el comportamiento de cada personaje. Básicamente, implementa una
/// máquina de estados, las acciones a llevar a cabo durante los estados y las diferentes posibles transferencias.
/// </summary>
public class CharacterStatus : MonoBehaviour {
    /// <summary>
    /// Evento para informar del cambio en el nivel de vida del personaje
    /// </summary>
    public delegate void CharacterLifeChanged(GameObject character);
    public static event CharacterLifeChanged KillCharacterEvent;
    public static event CharacterLifeChanged ResurrectCharacterEvent;

    /// <summary>
    /// Clase para definir los diferentes estados en los que se puede encontrar un personaje
    /// </summary>
    public enum State { AstralProjection, Breaking, BigJumping, Crouching, Dead, Falling, Idle, Jumping, Pushing, Scared, Sprint, Walking, Telekinesis, Using }


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
    private float totalJumpImpulse;
    public float maxJumpImpulse;
    public State initialCharacterState;
    public CharacterName characterName;
    // Audio variables
    private AudioLoader audioLoader;
    private AudioSource resurrectSound;
    private AudioSource dieSound;
    private AudioSource sacrificeSound;
    private AudioSource stepSound;

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
        // Inicialización
        characterState = initialCharacterState;
        //Se saca el objeto del padre para poder añadirlo como hijo a nuevos elementos
        transform.parent = null;
        // Obtención sonidos
        audioLoader = GetComponent<AudioLoader>();
        resurrectSound = audioLoader.GetSound("Resurrect");
        dieSound = audioLoader.GetSound("Die");
        sacrificeSound = audioLoader.GetSound("Die");
        stepSound = audioLoader.GetSound("Steps");
    }

    /// <summary>
    /// Comprueba si es posible comenzar la ejecución de una habilidad concreta, modificando el estado en que se encuentra el personaje
    /// </summary>
    /// <param name="ability">Habilidad que se desea iniciar a ejecutar</param>
    /// <returns><c>true</c> si es posible iniciar la habilidad, en cuyo caso modifica además el estado del personaje; <c>false</c> en otro caso</returns>
    public bool CanStartAbility(CharacterAbility ability) {
        // Estados desde los que se puede iniciar cualquier habilidad: Walking, Idle
        bool res = (characterState.Equals(State.Walking) || characterState.Equals(State.Idle));
        if (res) {
            // Detiene el sonido de andar, que es el único que puede estar reproduciéndose
            AudioManager.Stop(stepSound);
            // Actualiza el estado del personaje
            switch (ability.abilityName) {
                case AbilityName.AstralProjection:
                    characterState = State.AstralProjection;
                    break;
                case AbilityName.BigJump:
                    characterState = State.BigJumping;
                    break;
                case AbilityName.Break:
                    characterState = State.Breaking;
                    break;
                case AbilityName.Push:
                    characterState = State.Pushing;
                    break;
                case AbilityName.Sprint:
                    characterState = State.Sprint;
                    break;
                case AbilityName.Telekinesis:
                    characterState = State.Telekinesis;
                    break;
            }
        }

        return res;
    }

    void FixedUpdate() {
        // Aplica gravedad extra sobre el personaje si está cayendo
        if (characterState.Equals(State.Falling)) {
            characterMovement.ExtraGravity();
        }
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
    /// Mata al personaje y notifica su nuevo estado al manager, que desactivara su control y lo movera al checkpoint
    /// </summary>
    public void Kill() {
        bool kill = true;
        switch (characterState) {
            case State.AstralProjection:
                // La habilidad debe terminar su ejecución
                GetComponent<AbilityController>().UseAbility();
                // El personaje no debe morir
                kill = false;
                break;
            case State.Pushing:
                //Si muere mientras empuja un objeto, lo debe soltar
                if (GetComponent<PushAbility>() != null) {
                    GetComponent<PushAbility>().ReleaseObject();
                }
                break;
            case State.BigJumping:
            case State.Breaking:
            case State.Telekinesis:
            case State.Sprint:
                // La habilidad debe terminar su ejecución
                GetComponent<AbilityController>().UseAbility();
                break;
            case State.Dead:
                kill = false;
                break;
        }
        if (kill) {
            characterState = State.Dead;
            //Animacion, Efectos, Cambio de imagen.....
            GetComponent<Renderer>().enabled = false; //Temporal
            GetComponent<Rigidbody>().isKinematic = true;
            AudioManager.Play(dieSound, false, 1);
            //Reinicia el transform en caso de morir estando subido a una plataforma
            transform.parent = null;
            if (KillCharacterEvent != null) {
                KillCharacterEvent(gameObject);
            }
            characterManager.CharacterKilled(this);
        }
    }

    /// <summary>
    /// Función para indicar que el botón de salto ha sido pulsado, cambiando el estado del personaje a 'Jumping' en caso de ser necesario.
    /// </summary>
	public void JumpButton() {
        switch (characterState) {
            case State.Jumping:
                // Comprueba si ha alcanzado el impulso de salto máximo
                if (totalJumpImpulse < maxJumpImpulse) {
                    characterMovement.Jump(jumpImpulse, false);
                    totalJumpImpulse += jumpImpulse;
                }
                break;
            case State.Walking:
            case State.Idle:
                // Comienza la acción de saltar
                characterMovement.Jump(jumpImpulse, true);
                totalJumpImpulse = jumpImpulse;
                characterState = State.Jumping;
                break;
        }
    }

    /// <summary>
    /// Función para indicar que el botón de uso ha sido pulsado, cambiando el estado del personaje a 'Idle' si el objeto usado es instantáneo, o a 'Using' en caso de que el objeto requiera ser mantenido para su uso.
    /// </summary>
	public void UseButton() {
        switch (characterState) {
            case State.Idle:
            case State.Walking:
                if (playerUse.Use()) {
                    characterState = State.Using;
                } else {
                    characterState = State.Idle;
                }
                break;
            case State.AstralProjection:
                playerUse.Use();
                break;
            case State.Using:
                playerUse.StopUsing();
                characterState = State.Idle;
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
                characterState = State.Walking;
                break;
            case State.Idle:
            case State.Walking:
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
            case State.Idle:
                characterMovement.MoveCharacterNormal(horizontal, vertical, standingSpeed / 2);
                characterState = State.Walking;
                break;
            case State.Walking:
            case State.Sprint:
                characterMovement.MoveCharacterNormal(horizontal, vertical, standingSpeed);
                break;
            case State.Falling:
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
        switch (ability.abilityName) {
            case AbilityName.AstralProjection:
            case AbilityName.Break:
            case AbilityName.Push:
            case AbilityName.Sprint:
            case AbilityName.Telekinesis:
                characterState = State.Idle;
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        // Comprueba si el jugador está apoyado sobre alguna superficie 
        switch (characterState) {
            case State.Walking:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Falling;
                } else if (characterMovement.PlayerIsStopped()) { // Comprueba si el jugador está en movimiento
                    characterMovement.PlayerHasStopped();
                    characterState = State.Idle;
                }
                break;
            case State.Idle:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Falling;
                }
                break;
            case State.Falling:
                // Comprueba si el jugador está apoyado en alguna superficie 
                if (characterMovement.CharacterIsGrounded()) {
                    characterState = State.Idle;
                }
                break;
            case State.Jumping:
                // Si empieza a caer, cambia de estado
                if (characterMovement.CharacterIsFalling()) {
                    characterState = State.Falling;
                }
                break;
            case State.BigJumping:
                // Si empieza a caer, cambia de estado
                if (characterMovement.CharacterIsFalling()) {
                    GetComponent<AbilityController>().UseAbility();
                    characterState = State.Falling;
                }
                break;
            case State.Pushing:
                //TEMPORAL!! Suelta el objeto si el personaje se cae
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Falling;
                    GetComponent<PushAbility>().ReleaseObject();
                }
                // Comprueba si el jugador está en movimiento
                if (characterMovement.PlayerIsStopped()) {
                    characterMovement.PlayerHasStopped();
                }
                break;
            case State.Sprint:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    GetComponent<AbilityController>().UseAbility();
                    characterState = State.Falling;
                } else if (characterMovement.PlayerIsStopped()) { // Comprueba si el jugador está en movimiento
                    characterMovement.PlayerHasStopped();
                }
                break;
            case State.Crouching:
                // Comprueba si el jugador no está apoyado en ninguna superficie 
                if (!characterMovement.CharacterIsGrounded()) {
                    characterMovement.Stand();
                    characterState = State.Falling;
                }
                break;
            case State.Dead:
            case State.AstralProjection:
                break;
            default:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Falling;
                }
                break;
        }
    }

    /// <summary>
    /// Resucita al personaje
    /// </summary>
    public void Ressurect() {
        characterState = State.Idle;
        GetComponent<Renderer>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        if (ResurrectCharacterEvent != null) {
            ResurrectCharacterEvent(gameObject);
        }
        //Animacion, Efectos, Cambio de imagen.....
        AudioManager.Play(resurrectSound, false, 1);
    }

    IEnumerator Sacrifice() {
        // Bloquea al jugador
        InputManager.SetLock(true);
        // Efecto y sonido del "sacrificio"
        AudioManager.Play(sacrificeSound, false, 1);
        Rigidbody rig = GetComponent<Rigidbody>();
        rig.isKinematic = true;
        float upDistance = 3.0f;
        float d = upDistance * Time.deltaTime;
        do {
            // Eleva levemente al personaje
            transform.Translate(new Vector3(0, d, 0));
            upDistance -= d;
            d = upDistance * Time.deltaTime;
            yield return null;
        } while (upDistance > 0.2f);
        GetComponent<Renderer>().enabled = false; //Temporal
        //rig.isKinematic = false;
        // Actualización estado del personaje
        characterState = State.Dead;
        if (KillCharacterEvent != null) {
            KillCharacterEvent(gameObject);
        }
        characterManager.CharacterKilled(this);
        // Desbloquea al jugador
        InputManager.SetLock(false);
    }

    /// <summary>
    /// Función para indicar que el botón de sacrificio ha sido pulsado, sacificando al personaje sólo en caso de encontrarse en los estados adecuados
    /// </summary>
    public void SacrificeButton() {
        // Comprueba el estado del jugador
        switch (characterState) {
            case State.Idle:
            case State.Walking:
                StartCoroutine(Sacrifice());
                break;
        }
    }

    /// <summary>
    /// Cambia el estado asustado del personaje
    /// </summary>
    /// <param name="scared">true para ponerlo asustado, false para que deje de estarlo</param>
    public void SetScared(bool scared) {
        if (scared) {
            characterState = State.Scared;
        } else if (characterState.Equals(State.Scared)) {
            characterState = State.Idle;
        }
    }
}