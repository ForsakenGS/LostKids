using UnityEngine;
using System.Collections;
using System;

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
    public enum State { AstralProjection, Breaking, BigJumping, Crouching, Dead, Falling, Idle, Jumping, Pushing, Sacrifice, Scared, Sprint, Walking, Telekinesis, Using, Victory }

    //Referencia al manager de personajes
    public GameObject characterManagerPrefab;
    private CharacterManager characterManager;
    private CharacterMovement characterMovement;
    private PlayerUse playerUse;
    //private Animator characterAnimator;
    private Rigidbody rigBody;
    private SpecialIdleController specialIdleController;

    public float astralSpeed = 0.0f;
    public float standingSpeed = 8000f;
    public float jumpImpulse = 200000f;
    public float firstJumpImpulse = 10000f;
    public float astralJumpImpulse = 2000f;
    public float jumpingSpeed = 5000f;
    public float crouchingSpeed = 4000f;
    public float pushingSpeed = 4000f;
    public float maxSacrificeHeight;
    public float maxJumpImpulse;
    public float maxAstralJumpImpulse;

    //Habitacion en la que se encuentra el personaje
    public int currentRoom = 0;

    //Particulas
    private ParticlesActivator deadParticles;
    private ParticlesActivator resurrectionParticles;
    [HideInInspector]
    public ParticlesActivator playerParticles;
    public float abilityEmissionRate = 150.0f;

    public State initialCharacterState;
    public CharacterName characterName;
    private State characterState;
    private float totalJumpImpulse;
    private bool jumpButtonUp;
    private float sacrificeHeight;
    private bool lockedByAnimation;
    // Audio variables
    private AudioLoader audioLoader;
    private AudioSource resurrectSound;
    private AudioSource dieSound;
    private AudioSource sacrificeSound;
    private AudioSource pushSound;

    // Use this for initialization
    void Awake() {
        if (characterManagerPrefab == null) {
            characterManagerPrefab = GameObject.FindGameObjectWithTag("CharacterManager");
        }
        characterManager = characterManagerPrefab.GetComponent<CharacterManager>();
        characterMovement = GetComponent<CharacterMovement>();
        playerUse = GetComponent<PlayerUse>();
        CharacterAnimationController.SetAnimatorReference(characterName, GetComponent<Animator>());
        rigBody = GetComponent<Rigidbody>();
        specialIdleController = GetComponent<SpecialIdleController>();
        deadParticles = transform.Find("DeadParticles").gameObject.GetComponent<ParticlesActivator>();
        resurrectionParticles = transform.Find("ResurrectionParticles").gameObject.GetComponent<ParticlesActivator>();
        playerParticles = transform.Find("PlayerParticles").gameObject.GetComponent<ParticlesActivator>();
    }

    // Use this for initialization
    void Start() {
        // Inicialización
        Initialization();
        //Se saca el objeto del padre para poder añadirlo como hijo a nuevos elementos
        transform.parent = null;
        // Obtención sonidos
        audioLoader = GetComponent<AudioLoader>();
        resurrectSound = audioLoader.GetSound("Resurrect");
        dieSound = audioLoader.GetSound("Die");
        sacrificeSound = audioLoader.GetSound("Sacrifice");
        pushSound = audioLoader.GetSound("Push");
    }

    /// <summary>
    /// Comienza la ejecución de una habilidad concreta, modificando el estado en que se encuentra el personaje
    /// </summary>
    /// <param name="ability">Habilidad que se desea iniciar a ejecutar</param>
    /// <returns><c>true</c> si es posible iniciar la habilidad, en cuyo caso modifica además el estado del personaje; <c>false</c> en otro caso</returns>
    public bool StartAbility(CharacterAbility ability) {
        // Comprueba que el personaje no esté bloqueado por alguna animación
        bool res = !lockedByAnimation;
        if (res) {
            // Estados desde los que se puede iniciar cualquier habilidad: Walking, Idle
            res = (characterState.Equals(State.Walking) || characterState.Equals(State.Idle));
            if (res) {
                // Actualiza el estado del personaje
                switch (ability.abilityName) {
                    case AbilityName.AstralProjection:
                        characterState = State.AstralProjection;
                        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.ASTRAL_PROJECTION);
                        break;
                    case AbilityName.BigJump:
                        characterState = State.BigJumping;
                        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.BIG_JUMP);
                        break;
                    case AbilityName.Break:
                        characterState = State.Breaking;
                        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.BREAK);
                        break;
                    case AbilityName.Push:
                        characterState = State.Pushing;
                        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.PUSH);
                        break;
                    case AbilityName.Sprint:
                        characterState = State.Sprint;
                        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.SPRINT);
                        break;
                    case AbilityName.Telekinesis:
                        characterState = State.Telekinesis;
                        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.TELEKINESIS);
                        break;
                }
                //INCREMENTAR EL NUMERO DE PARTICULAS
                playerParticles.IncreaseEmission(abilityEmissionRate);
                // Inicia la habilidad
                res = ability.ActivateAbility();
            }
        }

        return res;
    }

    public bool CurrentStateIs(State s) {
        return characterState.Equals(s);
    }

    /// <summary>
    /// Función para indicar que el botón de agacharse ha sido pulsado, cambiando el estado del personaje a 'Crouching' en caso de ser necesario.
    /// </summary>
    public void CrouchButton() {
        // Comprueba que el personaje no esté bloqueado por alguna animación
        if (!lockedByAnimation) {
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
    }

    public void EnablePlayer() {
        GetComponentInChildren<Renderer>().enabled = true;
        rigBody.isKinematic = false;
        if (ResurrectCharacterEvent != null) {
            ResurrectCharacterEvent(gameObject);
        }
    }

    void FixedUpdate() {
        // Aplica gravedad extra sobre el personaje si está cayendo o en 
        if (characterState.Equals(State.Falling)) {
            characterMovement.ExtraGravity(1);
        } else if (characterState.Equals(State.AstralProjection)) {
            characterMovement.ExtraGravity(0.1f);
        }
    }

    void Initialization() {
        characterState = initialCharacterState;
        totalJumpImpulse = 0.0f;
        jumpButtonUp = false;
        sacrificeHeight = 0.0f;
        lockedByAnimation = false;
        playerParticles.DecreaseEmission();
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
    /// Función para indicar que el botón de salto está siendo pulsado, añadiendo impulso al salto en caso de ser necesario.
    /// </summary>
	public void JumpButton() {
        switch (characterState) {
            case State.Jumping:
                if (!jumpButtonUp) {
                    // Comprueba si ha alcanzado el impulso de salto máximo
                    if (totalJumpImpulse < maxJumpImpulse) {
                        characterMovement.Jump(jumpImpulse, false);
                        totalJumpImpulse += jumpImpulse;
                    }
                }
                break;
            case State.AstralProjection:
                if (!jumpButtonUp) {
                    // Comprueba si ha alcanzado el impulso de salto inicial
                    if (totalJumpImpulse < maxAstralJumpImpulse) {
                        characterMovement.Jump(astralJumpImpulse, false);
                        totalJumpImpulse += astralJumpImpulse;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Función para indicar que el botón de salto ha sido pulsado, cambiando el estado del personaje a 'Jumping' en caso de ser necesario.
    /// </summary>
	public void JumpButtonDown() {
        // Comprueba que el personaje no esté bloqueado por alguna animación
        if (!lockedByAnimation) {
            switch (characterState) {
                case State.Walking:
                case State.Idle:
                    // Comienza la acción de saltar
                    characterState = State.Jumping;
                    characterMovement.Jump(firstJumpImpulse, true);
                    totalJumpImpulse = firstJumpImpulse;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.JUMP);
                    break;
                case State.Sprint:
                    // Termina la habilidad de Sprint
                    if (GetComponent<AbilityController>().DeactivateActiveAbility(false)) {
                        characterState = State.Jumping;
                        characterMovement.Jump(firstJumpImpulse, true);
                        totalJumpImpulse = firstJumpImpulse;
                        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.JUMP);
                    }
                    break;
                case State.AstralProjection:
                    // Impulso inicial de la proyección astral al levitar
                    characterMovement.Jump(3 * astralJumpImpulse, false);
                    totalJumpImpulse += 3 * astralJumpImpulse;
                    break;
            }
        }
    }

    /// <summary>
    /// Función para indicar que el botón de salto se ha dejado de pulsar
    /// </summary>
    public void JumpButtonUp() {
        if (characterState.Equals(State.Jumping)) {
            jumpButtonUp = true;
        }
    }

    /// <summary>
    /// Mata al personaje y notifica su nuevo estado al manager, que desactivara su control y lo movera al checkpoint
    /// </summary>
    public void Kill() {
        bool kill = true;
        switch (characterState) {
            case State.AstralProjection:
                // La habilidad debe terminar su ejecución
                GetComponent<AbilityController>().DeactivateActiveAbility(true);
                // El personaje no debe morir
                kill = false;
                break;
            case State.Pushing:
            case State.BigJumping:
            case State.Breaking:
            case State.Telekinesis:
            case State.Sprint:
                // La habilidad debe terminar su ejecución
                GetComponent<AbilityController>().DeactivateActiveAbility(true);
                break;
            case State.Dead:
            case State.Sacrifice:
                kill = false;
                break;
        }
        if (kill) {
            // Limpieza del inventario
            GetComponent<CharacterInventory>().SetEmpty();
            // Cambios de estado
            characterState = State.Dead;
            CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.DEAD);
            //Animacion, Efectos, Cambio de imagen.....
            //GetComponentInChildren<ParticlesActivator>().Show();
            deadParticles.Show();
            GetComponentInChildren<CharacterIcon>().ActiveCanvas(false);
            GetComponentInChildren<Renderer>().enabled = false; //Temporal
            rigBody.isKinematic = true;
            AudioManager.Play(dieSound, false, 1);
            //Reinicia el transform en caso de morir estando subido a una plataforma
            transform.parent = null;

            iTween.ShakePosition(Camera.main.gameObject, new Vector3(1, 1, 0), 0.5f);

            XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, 1, 1);
            Invoke("DeathNotification", 0.5f);
        }
    }

    public void DeathNotification() {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, 0, 0);
        if (KillCharacterEvent != null) {
            KillCharacterEvent(gameObject);
        }
        characterManager.CharacterKilled(this);
        characterManager.ActivateNextCharacter();
    }

    public void LockByAnimation() {
        // Bloquea al personaje
        lockedByAnimation = true;
    }

    /// <summary>
    /// Función para indicar que alguno de los botones de movimiento ha sido pulsado, realizando el correspondiende movimiento y 
    /// variando la velocidad en función del estado en que se encuentre el personaje.
    /// </summary>
    /// <param name="horizontal">Movimiento sobre el eje X a aplicar sobre el personaje</param>
    /// <param name="vertical">Movimiento sobre el eje Z a aplicar sobre el personaje</param>
    public void MovementButtons(float horizontal, float vertical) {
        if (!lockedByAnimation) {
            switch (characterState) {
                case State.Idle:
                    characterMovement.MoveCharacterNormal(horizontal, vertical, standingSpeed / 2, true);
                    characterState = State.Walking;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.WALK);
                    break;
                case State.Walking:
                case State.Sprint:
                    characterMovement.MoveCharacterNormal(horizontal, vertical, standingSpeed, true);
                    break;
                case State.Falling:
                case State.Jumping:
                case State.BigJumping:
                    characterMovement.MoveCharacterNormal(horizontal, vertical, jumpingSpeed, false);
                    break;
                case State.AstralProjection:
                    characterMovement.MoveCharacterNormal(horizontal, vertical, astralSpeed, false);
                    break;
                case State.Crouching:
                    characterMovement.MoveCharacterNormal(horizontal, vertical, crouchingSpeed, false);
                    break;
                case State.Pushing:
                    // Calcula si el personaje empuja o arrastra hacia él la caja y selecciona animación
                    Vector3 pushNormal = GetComponent<PushAbility>().GetPushNormal();
                    CharacterAnimationController.SetAnimatorPropIsPushing((pushNormal.z * vertical < 0.0f) || (pushNormal.x * horizontal < 0.0f));
                    characterMovement.MoveCharacterAxes(horizontal, vertical, pushingSpeed, pushNormal);
                    break;
            }
        }
    }

    public void NegationAnimation() {
        // Comprueba si se puede ejecutar la animación
        if ((!lockedByAnimation) && ((characterState.Equals(State.Walking) || characterState.Equals(State.Idle)))) {
            // Ejecuta la animación y bloquea al personaje
            CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.NEGATION);
            AudioManager.Play(audioLoader.GetSound("Negation"), false, 1);
            LockByAnimation();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ability">Habilidad cuya ejecución se desea finalizar</param>
    public void EndAbility(CharacterAbility ability) {

        //PARAR CANTIDAD DE PARTICULAS
        playerParticles.DecreaseEmission();

        switch (ability.abilityName) {
            //case AbilityName.AstralProjection:
            //case AbilityName.Break:
            case AbilityName.Push:
                AudioManager.Stop(pushSound);
                //case AbilityName.Sprint:
                //case AbilityName.Telekinesis:
                break;
        }
        // Cambia de estado salvo que se trate de la habilidad "BigJump"
        if (!ability.abilityName.Equals(AbilityName.BigJump)) {
            characterState = State.Idle;
            CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.IDLE);
        }
    }

    public void UnlockByAnimation() {
        // Desbloquea al personaje
        lockedByAnimation = false;
        // Comprueba si es necesario un cambio de estado
        switch (characterState) {
            case State.Breaking:
                // La habilidad debe terminarse
                GetComponent<AbilityController>().DeactivateActiveAbility(false);
                characterState = State.Idle;
                break;
            case State.Victory:
                // El personaje debe repetir la animación
                CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.VICTORY);
                lockedByAnimation = true;
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        // Velocidad jugador
        CharacterAnimationController.SetAnimatorPropSpeed(characterName, characterMovement.GetPlayerSpeed(true));
        // Comprueba si el jugador está apoyado sobre alguna superficie 
        switch (characterState) {
            case State.Walking:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Falling;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.FALL);
                } else if (characterMovement.PlayerIsStopped()) { // Comprueba si el jugador está en movimiento
                    characterState = State.Idle;
                    //characterAnimator.ResetTrigger("Walk"); //TODO
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.IDLE);
                }
                break;
            case State.Idle:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Jumping;
                } else {
                    // Idle especial
                    if ((!specialIdleController.isActiveAndEnabled) && (!lockedByAnimation)) {
                        specialIdleController.enabled = true;
                    }
                }
                break;
            case State.Falling:
                // Comprueba si el jugador está apoyado en alguna superficie 
                if (characterMovement.CharacterIsGrounded()) {
                    characterState = State.Idle;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.LAND);
                }
                break;
            case State.Jumping:
                // Si empieza a caer, cambia de estado
                if (characterMovement.CharacterIsFalling()) {
                    jumpButtonUp = false;
                    totalJumpImpulse = 0.0f;
                    characterState = State.Falling;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.FALL);
                }
                break;
            case State.BigJumping:
                // Si empieza a caer, cambia de estado
                if (characterMovement.CharacterIsFalling()) {
                    GetComponent<AbilityController>().DeactivateActiveAbility(false);
                    characterState = State.Falling;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.FALL);
                }
                break;
            case State.Pushing:
                //TEMPORAL!! Suelta el objeto si el personaje se cae
                if (!characterMovement.CharacterIsGrounded()) {
                    GetComponent<AbilityController>().DeactivateActiveAbility(false);
                    characterState = State.Falling;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.FALL);
                }
                // Comprueba si el jugador está en movimiento
                if (characterMovement.PlayerIsStopped()) {
                    characterMovement.PlayerHasStopped();
                }
                break;
            case State.Sprint:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    GetComponent<AbilityController>().DeactivateActiveAbility(false);
                    characterState = State.Falling;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.FALL);
                } else if (characterMovement.PlayerIsStopped()) { // Comprueba si el jugador está en movimiento
                    GetComponent<AbilityController>().DeactivateActiveAbility(false);
                    characterState = State.Idle;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.IDLE);
                }
                break;
            case State.Crouching:
                // Comprueba si el jugador no está apoyado en ninguna superficie 
                if (!characterMovement.CharacterIsGrounded()) {
                    characterMovement.Stand();
                    characterState = State.Falling;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.FALL);
                }
                break;
            case State.Dead:
            case State.Sacrifice:
            case State.Victory:
                break;
            case State.AstralProjection:
                // Comprueba si el jugador está apoyado en alguna superficie 
                if (characterMovement.CharacterIsGrounded()) {
                    jumpButtonUp = false;
                    totalJumpImpulse = 0.0f;
                    CharacterAnimationController.SetAnimatorPropInAir(false);
                } else {
                    // Se encuentra levitando
                    CharacterAnimationController.SetAnimatorPropInAir(true);
                }
                break;
            default:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Falling;
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.FALL);
                }
                break;
        }
        // Controla que el personaje no se quede en animación de caída
        CharacterAnimationController.CheckFallAnimation(characterName, characterMovement.CharacterIsGrounded());
    }

    /// <summary>
    /// Función para indicar que el botón de uso ha sido pulsado, cambiando el estado del personaje a 'Idle' si el objeto usado es instantáneo, o a 'Using' en caso de que el objeto requiera ser mantenido para su uso.
    /// </summary>
    public void UseButton() {
        switch (characterState) {
            case State.Idle:
            case State.Walking:
                // Comprueba que el personaje no esté bloqueado por alguna animación
                if (!lockedByAnimation) {
                    // Comprueba si se trata de un NPC
                    if (playerUse.IsNPC()) {
                        playerUse.Use();
                    } else {
                        // Comprueba si puede usar el objeto
                        if (playerUse.CanUse()) {
                            // Animación de uso
                            //SOLO SE HACE LA ANIMACION PARA LAS PALANCAS
                            if (playerUse.UsingLever())
                            {
                                LockByAnimation();
                                CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.USE);
                            }
                            if (playerUse.Use() && playerUse.UsingLever()) {
                                // El jugador queda usando el objeto
                                characterState = State.Using;
                                CharacterAnimationController.SetAnimatorPropUsing(characterName, true);
                            } else {
                                // El jugador no permanece usando el objeto
                                characterState = State.Idle;
                            }
                        }
                    }
                }
                break;
            case State.AstralProjection:
                // Comprueba que el personaje no esté bloqueado por alguna animación
                if (!lockedByAnimation) {
                    // Comprueba si puede usar el objeto
                    if (playerUse.CanUse()) {
                        // Animación de uso
                        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.USE);
                        // Jugador usa el objeto
                        playerUse.Use();
                    }
                }
                break;
            case State.Using:
                // Comprueba si el personaje se encuentra en la animación "LeverHold"
                if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("LeverHold")) {
                    playerUse.StopUsing();
                    characterState = State.Idle;
                    CharacterAnimationController.SetAnimatorPropUsing(characterName, false);
                    CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.IDLE);
                }
                break;
        }
    }

    /// <summary>
    /// Restaura los valores del personaje a la configuración inicial
    /// </summary>
    public void ResetCharacter() {
        // Reinicia los elementos con los que puede estar interaccionando el personaje
        if (characterState.Equals(State.Using)) {
            playerUse.StopUsing();
        }
        // Reinicia el estado de las habilidades
        GetComponent<AbilityController>().ResetAbilities();
        // Reinicia a los valores por defecto
        Initialization();
        GetComponent<Animator>().Rebind();
        // Esconde el tooltip del jugador
        GetComponentInChildren<CharacterIcon>().ActiveCanvas(false);
    }

    /// <summary>
    /// Resucita al personaje
    /// </summary>
    public void Ressurect() {
        // Reinicia las máquinas de estado
        ResetCharacter();
        resurrectionParticles.Show();
        Invoke("EnablePlayer", 0.8f);
        //Animacion, Efectos, Cambio de imagen.....
        AudioManager.Play(resurrectSound, false, 1);
    }

    // Se ejecuta cuando se termina el proceso de sacrificio del personaje
    void SacrificeEnd() {
        // Comprueba si se ha alcanzado distancia objetivo
        if (sacrificeHeight >= maxSacrificeHeight) {
            // Sitúa los objetos del inventario en posición personaje y vacía inventario
            GetComponent<CharacterInventory>().SetEmptyToPosition(transform.position + 3 * Vector3.down);
            AudioManager.Stop(sacrificeSound);
            GetComponentInChildren<Renderer>().enabled = false;
            // Actualización estado del personaje
            sacrificeHeight = 0.0f;
            characterState = State.Dead;
            if (KillCharacterEvent != null) {
                KillCharacterEvent(gameObject);
            }
            characterManager.CharacterKilled(this);
            characterManager.ActivateNextCharacter();
        }
    }

    // Se ejecuta cuando comienza el proceso de sacrificio del personaje
    void SacrificeStart() {
        // Comprueba que el personaje no esté bloqueado por alguna animación
        if (!lockedByAnimation) {
            // Actualización estado
            characterState = State.Sacrifice;
            // Efecto y sonido del "sacrificio"
            rigBody.velocity = Vector3.zero;
            CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.SACRIFICE);
            AudioManager.Play(sacrificeSound, false, 1);
            GetComponentInChildren<CharacterIcon>().ActiveCanvas(false);
            //rigBody.isKinematic = true;
        }
    }

    /// <summary>
    /// Función para indicar que el botón de sacrificio ha sido pulsado, sacrificando al personaje sólo en caso de encontrarse en los estados adecuados
    /// </summary>
    public void SacrificeButtons() {
        // Comprueba el estado del jugador
        switch (characterState) {
            case State.Sacrifice:
                if (sacrificeHeight < maxSacrificeHeight) {
                    // El personaje sigue ascendiendo
                    float d = 3.0f * Time.deltaTime;
                    transform.Translate(new Vector3(0, d, 0));
                    sacrificeHeight += d;
                }
                break;
            case State.Idle:
            case State.Walking:
                SacrificeStart();
                break;
        }
    }

    /// <summary>
    /// Función para indicar que el botón de sacrificio se ha dejado de pulsar, anulando el sacrificio del personaje si aún no se ha llevado a cabo
    /// </summary>
    public void SacrificeButtonsUp() {
        // Comprueba el estado del jugador
        switch (characterState) {
            case State.Sacrifice:
                characterState = State.Falling;
                AudioManager.Stop(sacrificeSound);
                CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.FALL);
                sacrificeHeight = 0.0f;
                rigBody.isKinematic = false;
                break;
        }
    }

    /// <summary>
    /// Reproduce la animacion de victoria y en caso de ser final de nivel, bloquea el personaje hasta que los demas lo completen
    /// </summary>
    public void Victory(bool levelEnd) {
        //Girar al personaje para que se va de frente mejor?
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        // Ejecuta la animación
        CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.VICTORY);
        LockByAnimation();
        if (levelEnd) {
            // Fin del nivel
            characterState = State.Victory;
        }
    }

    /// <summary>
    /// Cambia el estado asustado del personaje
    /// </summary>
    /// <param name="scared">true para ponerlo asustado, false para que deje de estarlo</param>
    public void SetScared(bool scared) {
        if (scared) {
            characterState = State.Scared;
            CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.SCARED);
        } else if (characterState.Equals(State.Scared)) {
            characterState = State.Idle;
            CharacterAnimationController.SetAnimatorTrigger(characterName, CharacterAnimationController.IDLE);
        }
    }
}