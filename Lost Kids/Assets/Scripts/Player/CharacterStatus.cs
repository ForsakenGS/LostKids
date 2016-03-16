using UnityEngine;
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
    public enum State { Breaking, Crouching, Dead, BigJumping, Jumping, Pushing, Scared, Sprint, Standing, Using }
    /// <summary>
    /// Clase para definir los diferentes personajes que pueden implementar una máquina de estados
    /// </summary>
    public enum CharacterName { Aoi, Akai, Murasaki };

    //Referencia al manager de personajes
    public GameObject characterManagerPrefab;
    private CharacterManager characterManager;
    private CharacterMovement characterMovement;
    private PlayerUse playerUse;

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

    }

    // Update is called once per frame
    void Update() {
        bool groundedCharacter = characterMovement.CharacterIsGrounded();
        switch (characterState) {
            case State.Jumping:
                if (groundedCharacter) {
                    characterState = State.Standing;
                }
                break;
            case State.BigJumping:
                if (groundedCharacter) {
                    GetComponent<AbilityController>().UseAbility();
                    characterState = State.Standing;
                }
                break;
            default:    // CAMBIAR!! Hay que hacerlo para cada estado
                if (!groundedCharacter) {
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
        characterState = State.Dead;
        //Animacion, Efectos, Cambio de imagen.....
        GetComponent<Renderer>().enabled = false; //Temporal
        GetComponent<Rigidbody>().isKinematic = true;
        characterManager.CharacterKilled(this);
        if (KillCharacterEvent != null) {
            KillCharacterEvent(gameObject);
        }
    }

    /// <summary>
    /// Función para indicar que el botón de salto ha sido pulsado, cambiando el estado del personaje a 'Jumping' en caso de ser necesario.
    /// </summary>
	public void JumpButton() {
        switch (characterState) {
            case State.Standing:
                characterMovement.Jump(jumpImpulse);
                characterState = State.Jumping;
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
            case State.Crouching:
                characterMovement.MoveCharacterNormal(horizontal, vertical, crouchingSpeed);
                break;
            case State.Pushing:
                characterMovement.MoveCharacterAxes(horizontal, vertical, pushingSpeed, GetComponent<PushAbility>().GetPushNormal());
                break;
        }
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
            }
        }

        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ability">Habilidad cuya ejecución se desea finalizar</param>
    public void EndAbility(CharacterAbility ability) {
        switch (ability.GetType().ToString()) {
            case "BreakAbility":
            case "PushAbility":
            case "SprintAbility":
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
        AudioManager.Play(audioLoader.GetSound("Resurrect"), false, 1);
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
}