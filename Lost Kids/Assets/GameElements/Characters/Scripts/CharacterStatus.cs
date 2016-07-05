using UnityEngine;
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
    public enum State { AstralProjection, Breaking, BigJumping, Crouching, Dead, Falling, Idle, Jumping, Pushing, Sacrifice, Scared, Sprint, Walking, Telekinesis, Using }


    //Referencia al manager de personajes
    public GameObject characterManagerPrefab;
    private InputManagerTLK inputManager;
    private CharacterManager characterManager;
    private CharacterMovement characterMovement;
    private PlayerUse playerUse;
    private Animator characterAnimator;
    private Rigidbody rigBody;

    public float astralSpeed = 0.0f;
    public float standingSpeed = 8000f;
    public float jumpImpulse = 200000f;
    public float firstJumpImpulse = 10000f;
    public float astralJumpImpulse = 2000f;
    public float jumpingSpeed = 5000f;
    public float crouchingSpeed = 4000f;
    public float pushingSpeed = 4000f;
    public float maxSacrificeHeight;

    //Habitacion en la que se encuentra el personaje
    public int currentRoom = 0;

    // Personaje
    private State characterState;
    private float totalJumpImpulse;
    public float maxJumpImpulse;
    public State initialCharacterState;
    public CharacterName characterName;
    private bool jumpButtonUp = false;
    private float sacrificeHeight;
    // Audio variables
    private AudioLoader audioLoader;
    private AudioSource resurrectSound;
    private AudioSource dieSound;
    private AudioSource sacrificeSound;
    private AudioSource stepSound;
    private AudioSource pushSound;

    private ParticlesActivator deadParticles;
    private ParticlesActivator resurrectionParticles;

    // Use this for initialization
    void Awake() {
        if (characterManagerPrefab == null) {
            characterManagerPrefab = GameObject.FindGameObjectWithTag("CharacterManager");
        }
        inputManager = GameObject.Find("InputManagerTLK").GetComponent<InputManagerTLK>();
        characterManager = characterManagerPrefab.GetComponent<CharacterManager>();
        characterMovement = GetComponent<CharacterMovement>();
        playerUse = GetComponent<PlayerUse>();
        characterAnimator = GetComponent<Animator>();
        rigBody = GetComponent<Rigidbody>();

        deadParticles = transform.Find("DeadParticles").gameObject.GetComponent<ParticlesActivator>();
        resurrectionParticles = transform.Find("ResurrectionParticles").gameObject.GetComponent<ParticlesActivator>();

    }

    // Use this for initialization
    void Start() {
        // Inicialización
        characterState = initialCharacterState;
        sacrificeHeight = 0.0f;
        //Se saca el objeto del padre para poder añadirlo como hijo a nuevos elementos
        transform.parent = null;
        // Obtención sonidos
        audioLoader = GetComponent<AudioLoader>();
        resurrectSound = audioLoader.GetSound("Resurrect");
        dieSound = audioLoader.GetSound("Die");
        sacrificeSound = audioLoader.GetSound("Sacrifice");
        stepSound = audioLoader.GetSound("Steps");
        pushSound = audioLoader.GetSound("Push");
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
                    SetAnimatorTrigger("AstralProjection");
                    break;
                case AbilityName.BigJump:
                    characterState = State.BigJumping;
                    SetAnimatorTrigger("BigJump");
                    break;
                case AbilityName.Break:
                    characterState = State.Breaking;
                    SetAnimatorTrigger("Break");
                    break;
                case AbilityName.Push:
                    characterState = State.Pushing;
                    SetAnimatorTrigger("Push");
                    break;
                case AbilityName.Sprint:
                    characterState = State.Sprint;
                    SetAnimatorTrigger("Sprint");
                    break;
                case AbilityName.Telekinesis:
                    characterState = State.Telekinesis;
                    SetAnimatorTrigger("Telekinesis");
                    break;
            }
        }

        return res;
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
    /// Función para indicar que el botón de salto ha sido pulsado, cambiando el estado del personaje a 'Jumping' en caso de ser necesario.
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
            case State.Walking:
            case State.Idle:
                // Comienza la acción de saltar
                AudioManager.Stop(stepSound);
                characterState = State.Jumping;
                characterMovement.Jump(firstJumpImpulse, true);
                totalJumpImpulse = firstJumpImpulse;
                SetAnimatorTrigger("Jump");
                break;
            case State.AstralProjection:
                if (!jumpButtonUp) {
                    // Comprueba si ha alcanzado el impulso de salto inicial
                    if (totalJumpImpulse < firstJumpImpulse) {
                        characterMovement.Jump(astralJumpImpulse, false);
                        totalJumpImpulse += astralJumpImpulse;
                    }
                }
                break;
        }
    }

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
                GetComponent<AbilityController>().DeactivateActiveAbility();
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
                GetComponent<AbilityController>().DeactivateActiveAbility();
                break;
            case State.Dead:
            case State.Sacrifice:
                kill = false;
                break;
        }
        if (kill) {
            characterState = State.Dead;
            SetAnimatorTrigger("Dead");
            //Animacion, Efectos, Cambio de imagen.....
            //GetComponentInChildren<ParticlesActivator>().Show();
            deadParticles.Show();
            GetComponentInChildren<CharacterIcon>().ActiveCanvas(false);
            GetComponentInChildren<Renderer>().enabled = false; //Temporal
            rigBody.isKinematic = true;
            AudioManager.Stop(stepSound);
            AudioManager.Play(dieSound, false, 1);
                       //Reinicia el transform en caso de morir estando subido a una plataforma
            transform.parent = null;

            iTween.ShakePosition(Camera.main.gameObject, new Vector3(1, 1, 0), 0.5f);

            XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, 1, 1);
            //InputManager.ActiveDevice.Vibrate(100f);

            Invoke("DeathNotification", 0.5f);

        }
    }

    public void DeathNotification()
    {
        XInputDotNetPure.GamePad.SetVibration(XInputDotNetPure.PlayerIndex.One, 0, 0);
        if (KillCharacterEvent != null)
        {
            KillCharacterEvent(gameObject);
        }
        characterManager.CharacterKilled(this);
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
                characterMovement.MoveCharacterNormal(horizontal, vertical, standingSpeed / 2, true);
                characterState = State.Walking;
                characterAnimator.ResetTrigger("Idle");
                SetAnimatorTrigger("Walk");
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
                characterAnimator.SetBool("isPushing", ((pushNormal.z * vertical < 0.0f) || (pushNormal.x * horizontal > 0.0f)));
                characterMovement.MoveCharacterAxes(horizontal, vertical, pushingSpeed, pushNormal);
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ability">Habilidad cuya ejecución se desea finalizar</param>
    public void EndAbility(CharacterAbility ability) {
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
            SetAnimatorTrigger("Idle");
        }
    }

    // Update is called once per frame
    void Update() {
        characterAnimator.SetFloat("speed", characterMovement.GetPlayerSpeed(true));
        // Comprueba si el jugador está apoyado sobre alguna superficie 
        switch (characterState) {
            case State.Walking:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    AudioManager.Stop(stepSound);
                    characterState = State.Falling;
                    SetAnimatorTrigger("Fall");
                } else if (characterMovement.PlayerIsStopped()) { // Comprueba si el jugador está en movimiento
                    characterMovement.PlayerHasStopped();
                    characterState = State.Idle;
                    characterAnimator.ResetTrigger("Walk");
                    SetAnimatorTrigger("Idle");
                }
                break;
            case State.Idle:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Jumping;
                    //SetAnimatorTrigger("Fall");
                }
                break;
            case State.Falling:
                // Comprueba si el jugador está apoyado en alguna superficie 
                if (characterMovement.CharacterIsGrounded()) {
                    characterState = State.Idle;
                    SetAnimatorTrigger("Land");
                    inputManager.LockTime(0.3f);
                }
                break;
            case State.Jumping:
                // Si empieza a caer, cambia de estado
                if (characterMovement.CharacterIsFalling()) {
                    jumpButtonUp = false;
                    totalJumpImpulse = 0.0f;
                    characterState = State.Falling;
                    SetAnimatorTrigger("Fall");
                } else if (characterMovement.CharacterIsGrounded()) {  // Comprueba si el jugador está apoyado en alguna superficie 
                    jumpButtonUp = false;
                    totalJumpImpulse = 0.0f;
                    characterState = State.Idle;
                    SetAnimatorTrigger("Land");
                }
                break;
            case State.BigJumping:
                // Si empieza a caer, cambia de estado
                if (characterMovement.CharacterIsFalling()) {
                    GetComponent<AbilityController>().DeactivateActiveAbility();
                    characterState = State.Falling;
                    SetAnimatorTrigger("Fall");
                }
                break;
            case State.Pushing:
                //TEMPORAL!! Suelta el objeto si el personaje se cae
                if (!characterMovement.CharacterIsGrounded()) {
                    GetComponent<PushAbility>().ReleaseObject();
                    characterState = State.Falling;
                    SetAnimatorTrigger("Fall");
                }
                // Comprueba si el jugador está en movimiento
                if (characterMovement.PlayerIsStopped()) {
                    characterMovement.PlayerHasStopped();
                }
                break;
            case State.Sprint:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    GetComponent<AbilityController>().DeactivateActiveAbility();
                    characterState = State.Falling;
                    SetAnimatorTrigger("Fall");
                } else if (characterMovement.PlayerIsStopped()) { // Comprueba si el jugador está en movimiento
                    characterMovement.PlayerHasStopped();
                }
                break;
            case State.Crouching:
                // Comprueba si el jugador no está apoyado en ninguna superficie 
                if (!characterMovement.CharacterIsGrounded()) {
                    characterMovement.Stand();
                    characterState = State.Falling;
                    SetAnimatorTrigger("Fall");
                }
                break;
            case State.Dead:
            case State.Sacrifice:
                break;
            case State.AstralProjection:
                // Comprueba si el jugador está apoyado en alguna superficie 
                if (characterMovement.CharacterIsGrounded()) {
                    jumpButtonUp = false;
                    totalJumpImpulse = 0.0f;
                }
                break;
            default:
                // Si está en el aire, cambia de estado
                if (!characterMovement.CharacterIsGrounded()) {
                    characterState = State.Falling;
                    SetAnimatorTrigger("Fall");
                }
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
                // Comprueba si se trata del Kodama
                if (playerUse.IsKodama()) {
                    playerUse.Use();
                } else {
                    // Comprueba si puede usar el objeto
                    if (playerUse.CanUse()) {
                        // Animación de uso
                        characterAnimator.SetTrigger("Use");
                        inputManager.LockTime(1.1f);
                        if (playerUse.Use()) {
                            // El jugador queda usando el objeto
                            characterState = State.Using;
                            characterAnimator.SetBool("using", true);
                        } else {
                            // El jugador deja de usar el objeto
                            characterState = State.Idle;
                            characterAnimator.SetTrigger("Idle");
                            //inputManager.LockTime(0.2f);
                        }
                    }
                }
                break;
            case State.AstralProjection:
                // Comprueba si puede usar el objeto
                if (playerUse.CanUse()) {
                    // Animación de uso
                    characterAnimator.SetTrigger("Use");
                    inputManager.LockTime(1.1f);
                    // Jugador usa el objeto
                    playerUse.Use();
                }
                break;
            case State.Using:
                playerUse.StopUsing();
                characterState = State.Idle;
                characterAnimator.SetBool("using", false);
                characterAnimator.SetTrigger("Idle");
                inputManager.LockTime(0.2f);
                break;
        }
    }

    /// <summary>
    /// Resucita al personaje
    /// </summary>
    public void Ressurect() {
        // Reinicia las máquinas de estado
        characterAnimator.Rebind();
        characterState = State.Idle;
        resurrectionParticles.Show();
        Invoke("EnablePlayer", 0.8f);
        //Animacion, Efectos, Cambio de imagen.....
        AudioManager.Play(resurrectSound, false, 1);
    }

    public void EnablePlayer() {
        GetComponentInChildren<Renderer>().enabled = true;
        rigBody.isKinematic = false;
        if (ResurrectCharacterEvent != null) {
            ResurrectCharacterEvent(gameObject);
        }
    }
    
    // Se ejecuta cuando se termina el proceso de sacrificio del personaje
    void SacrificeEnd() {
        AudioManager.Stop(sacrificeSound);
        GetComponentInChildren<Renderer>().enabled = false; //Temporal
        //rig.isKinematic = false;
        // Actualización estado del personaje
        characterState = State.Dead;
        if (KillCharacterEvent != null) {
            KillCharacterEvent(gameObject);
        }
        characterManager.CharacterKilled(this);
    }

    // Se ejecuta cuando comienza el proceso de sacrificio del personaje
    void SacrificeStart() {
        // Efecto y sonido del "sacrificio"
        characterState = State.Sacrifice;
        SetAnimatorTrigger("Sacrifice");
        AudioManager.Stop(stepSound);
        AudioManager.Play(sacrificeSound, false, 1);
        GetComponentInChildren<CharacterIcon>().ActiveCanvas(false);
        rigBody.isKinematic = true;
    }

    /// <summary>
    /// Función para indicar que el botón de sacrificio ha sido pulsado, sacrificando al personaje sólo en caso de encontrarse en los estados adecuados
    /// </summary>
    public void SacrificeButton() {
        // Comprueba el estado del jugador
        switch (characterState) {
            case State.Sacrifice:
                if (sacrificeHeight < maxSacrificeHeight) {
                    // El personaje sigue ascendiendo
                    float d = 3.0f * Time.deltaTime;
                    transform.Translate(new Vector3(0, d, 0));
                    sacrificeHeight += d;
                } else {
                    // El personaje se sacrifica
                    sacrificeHeight = 0.0f;
                    SacrificeEnd();
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
    public void SacrificeButtonUp() {
        // Comprueba el estado del jugador
        switch (characterState) {
            case State.Sacrifice:
                characterState = State.Falling;
                AudioManager.Stop(sacrificeSound);
                SetAnimatorTrigger("Fall");
                sacrificeHeight = 0.0f;
                rigBody.isKinematic = false;
                break;
        }
    }

    void SetAnimatorTrigger(string trigger) {
        // Desactiva cualquier otro posible trigger
        characterAnimator.ResetTrigger("Idle");
        characterAnimator.ResetTrigger("Walk");
        characterAnimator.ResetTrigger("Jump");
        characterAnimator.ResetTrigger("Fall");
        characterAnimator.ResetTrigger("Land");
        characterAnimator.ResetTrigger("Dead");
        characterAnimator.ResetTrigger("Scared");
        characterAnimator.ResetTrigger("Sacrifice");
        characterAnimator.ResetTrigger("Use");
        // Desactiva triggers de las habilidades
        if (characterName.Equals(CharacterName.Aoi)) {
            characterAnimator.ResetTrigger("BigJump");
            characterAnimator.ResetTrigger("Sprint");
        } else if (characterName.Equals(CharacterName.Akai)) {
            characterAnimator.ResetTrigger("Break");
            characterAnimator.ResetTrigger("Push");
        } else {
            characterAnimator.ResetTrigger("Telekinesis");
            characterAnimator.ResetTrigger("AstralProjection");
        }
        // Activa el trigger correspondiente
        characterAnimator.SetTrigger(trigger);
    }

    /// <summary>
    /// Cambia el estado asustado del personaje
    /// </summary>
    /// <param name="scared">true para ponerlo asustado, false para que deje de estarlo</param>
    public void SetScared(bool scared) {
        if (scared) {
            characterState = State.Scared;
            SetAnimatorTrigger("Scared");
        } else if (characterState.Equals(State.Scared)) {
            characterState = State.Idle;
            SetAnimatorTrigger("Idle");
        }
    }
}