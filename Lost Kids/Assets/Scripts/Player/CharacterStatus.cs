using UnityEngine;
using System.Collections;

public class CharacterStatus : MonoBehaviour {
	/// <summary>
	/// Clase para definir los diferentes estados en los que se puede encontrar un personaje
	/// </summary>
	public enum State {Breaking, Crouching, Dead, BigJumping, Jumping, Pushing, Scared, Sprint, Standing, Using}
	/// <summary>
	/// Clase para definir los diferentes personajes que pueden implementar una máquina de estados
	/// </summary>
	public enum CharacterName {Aoi, Akai, Murasaki};
    
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

	// Se lanza cuando el jugador entra en contacto con otro objeto del juego con Collider. Detecta el final de los saltos.
	void OnCollisionEnter(Collision col) {
		switch (characterState) {
		case State.BigJumping:
			if (characterMovement.CharacterIsGrounded()) {
				GetComponent<AbilityController>().UseAbility();
				characterState = State.Standing;
			}
			break;
		case State.Jumping:
			if (characterMovement.CharacterIsGrounded()) {
				characterState = State.Standing;
			}
			break;
		}
	}

	// Se lanza cuando el jugador deja de estar en contacto con otro objeto del juego con Collider. Detecta la caída libre del personaje
	void OnCollisionExit(Collision col) {
		switch (characterState) {
		case State.BigJumping:
		case State.Jumping:
			break;
		default:	// CAMBIAR!! Hay que hacerlo para cada estado
			if (!characterMovement.CharacterIsGrounded()) {
				characterState = State.Jumping;
			}
			break;
		}
	}

	// Update is called once per frame
	void Update() {
		Debug.Log(characterName.ToString() + " -> " + characterState.ToString());
	}

	void FixedUpdate() {
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
        characterManager.CharacterKilled(this);    
    }

	public void JumpButton() {
		switch (characterState) {
		case State.Standing:
			characterMovement.Jump(jumpImpulse);
			characterState = State.Jumping;
			break;
		}
	}

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

	public void MovementButtons(float horizontal, float vertical) {
		switch (characterState) {
		case State.Crouching:
			characterMovement.MoveCharacterNormal(horizontal, vertical, crouchingSpeed);
			break;
		case State.Jumping:
		case State.BigJumping:
			characterMovement.MoveCharacterNormal(horizontal, vertical, jumpingSpeed);
			break;
		case State.Pushing:
			characterMovement.MoveCharacterAxes(horizontal, vertical, pushingSpeed, GetComponent<PushAbility>().GetPushNormal());
			break;
		case State.Sprint:
		case State.Standing:
			characterMovement.MoveCharacterNormal(horizontal, vertical, standingSpeed);
			break;
		}
	}

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
        //Animacion, Efectos, Cambio de imagen.....
        AudioManager.Play(audioLoader.GetSound("Resurrect"), false, 1);
    }

    /// <summary>
    /// Devuelve true si el personaje esta disponible para su manejo
    /// Puede encontrarse indisponible si esta muerto, o asustado
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