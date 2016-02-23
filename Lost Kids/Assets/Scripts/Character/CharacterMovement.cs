using UnityEngine;
using System.Collections;

/* Class to control the movement of a character getting player's controls from InputManager */
public class CharacterMovement : MonoBehaviour {
	/* Different states for the character: Standing - Jumping - Crouching */
	private enum State {Standing, Jumping, Crouching}

	// Speed & impulse for each possible state
	public float standingSpeed = 8000f;
	public float jumpingImpulse = 40f;
	public float jumpingSpeed = 5000f;
	public float extraGravity= 1.2f;
	public float crouchingSpeed = 4000f;
	public float turnSmoothing = 15f;
	public float groundCheckDistance = 1.2f;
	[HideInInspector]
	public float speedModifier;
	[HideInInspector]
	public float jumpImpulseModifier;

    private CameraController cameraController;

	private Rigidbody rigBody;
	private Collider standingColl;
	private Collider crouchingColl;
	private State characterState;
	private bool startJump;
	private float speed;

	// Use this for initialization
	void Start () {
		// Components
		rigBody = GetComponent<Rigidbody>();
		Collider[] colliders = GetComponents<Collider>();
		standingColl = colliders[0];
		standingColl.enabled = true;
		crouchingColl = colliders[1];
		crouchingColl.enabled = false;
		// Variables
		characterState = State.Standing;
		startJump = false;
		speedModifier = 1.0f;
		jumpImpulseModifier = 1.0f;
		speed = 0f;

        //Referente to camera controller
        cameraController = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraController>();
	}

	// Check if the character is over the floor by using a raycast
	bool CharacterIsGrounded() {
		// helper to visualise the ground check ray in the scene view
		#if UNITY_EDITOR
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
		#endif

		return (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, groundCheckDistance));
	}

	// Called at a fixed interval. Physics code here!
	void FixedUpdate () {
		if (!CharacterIsGrounded()) {
			// Extra gravity to fall down quickly
			Vector3 forceToApply = new Vector3(0, -1 * extraGravity, 0);
			rigBody.AddForce(forceToApply * (speedModifier * speed), ForceMode.Force);
		}
	}

	// Called at a fixed interval. Physics code here!
//	void FixedUpdate () {
//		Vector3 forceToApply = new Vector3();
//		float speed = 0.0f;
//
//		// Decide force to apply depending on current player's state
//		switch (characterState) {
//		case State.Jumping:
//			if (startJump) {
//				// Impulse to jump
//				forceToApply += new Vector3(0, jumpImpulseModifier * jumpingImpulse, 0);
//				startJump = false;
//				// Reset jumpImpulseModifier
//				if (jumpImpulseModifier != 1.0f) {
//					jumpImpulseModifier = 1.0f;
//				}
//			} else {
//				// Extra gravity to fall down quickly
//				forceToApply += new Vector3(0, -1 * extraGravity, 0);
//			}
//			speed = jumpingSpeed;
//			break;
//		case State.Crouching:
//			speed = crouchingSpeed;
//			break;
//		case State.Standing:
//			speed = standingSpeed;
//			break;
//		}
//		// Character's movement
//		float horizontal = Input.GetAxis("Horizontal");
//		float vertical = Input.GetAxis("Vertical");
//		if ((horizontal != 0f) || (vertical != 0f)) {
//			forceToApply += new Vector3(horizontal, 0, vertical);
//		}
//		// Force relativily applied to camera's field of view
//		forceToApply = GetVectorRelativeToObject(forceToApply, cameraController.CurrentCamera().transform);
//		if (!forceToApply.Equals(Vector3.zero)) {
//			rigBody.AddForce(forceToApply * (speedModifier * speed), ForceMode.Force);
//			Rotating(forceToApply.x, forceToApply.z);
//		}
//	}

	// Return the given vector relative to the given camera 
	Vector3 GetVectorRelativeToObject(Vector3 inputVector, Transform camera) {
		Vector3 objectRelativeVector = Vector3.zero;
		if (inputVector != Vector3.zero) {
			// Forward vector for camera's field of view
			Vector3 forward = camera.TransformDirection(Vector3.forward);
			forward.y = 0f;
			forward.Normalize();
			// Calculate new vector
			Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
			Vector3 relativeRight = inputVector.x * right;
			Vector3 relativeForward = inputVector.z * forward;
			objectRelativeVector = relativeRight + relativeForward;
			// Normalizing vector
			if (objectRelativeVector.magnitude > 1f) {
				objectRelativeVector.Normalize();	
			}
		}
		// Set original y-coordinate, because height in world is not camera depending
		objectRelativeVector += new Vector3(0,inputVector.y,0);

		return objectRelativeVector;	
	}

	// Make the character to da an extra jump
	public void ExtraJump (float modif) {
		if ((characterState.Equals(State.Jumping))) {
			jumpImpulseModifier = modif;
			startJump = true;
		}
	}

	// Change character's rotation to make it look at the direction it is going
	void Rotating (float horizontal, float vertical) {
		// Create a rotation based on the horizontal and vertical inputs
		Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Lerp(rigBody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		// Change the players rotation to this new rotation.
		rigBody.MoveRotation(newRotation);
	}

	public void JumpButton () {
		if ((characterState.Equals(State.Standing)) && (Input.GetButtonDown("Jump"))) {
			// Standing => Jumping
			characterState = State.Jumping;
			startJump = true;
		}
	}

	public void CrouchButton () {
		if (characterState.Equals(State.Standing)) {
			// Standing => Crouching
			characterState = State.Crouching;
			standingColl.enabled = false;
			crouchingColl.enabled = true;
			transform.Translate(new Vector3(0, -0.5f,0));
			transform.localScale -= new Vector3(0,0.5f,0); // CAMBIAR! No se debe modificar el tamaño del objeto
		} else if (characterState.Equals(State.Crouching)) {
			// Crouching => Standing
			characterState = State.Standing;
			standingColl.enabled = true;
			crouchingColl.enabled = false;
			transform.Translate(new Vector3(0, 0.5f,0));
			transform.localScale += new Vector3(0,0.5f,0); // CAMBIAR! No se debe modificar el tamaño del objeto
		}
	}

	public void MoveCharacter (float horizontal, float vertical) {
		Vector3 forceToApply = new Vector3();

		// Decide force to apply depending on current player's state
		switch (characterState) {
		case State.Jumping:
			if (startJump) {
				// Impulse to jump
				forceToApply += new Vector3(0, jumpImpulseModifier * jumpingImpulse, 0);
				startJump = false;
				// Reset jumpImpulseModifier
				if (jumpImpulseModifier != 1.0f) {
					jumpImpulseModifier = 1.0f;
				}
			}
			speed = jumpingSpeed;
			break;
		case State.Crouching:
			speed = crouchingSpeed;
			break;
		case State.Standing:
			speed = standingSpeed;
			break;
		}
		// Character's movement
		if ((horizontal != 0f) || (vertical != 0f)) {
			forceToApply += new Vector3(horizontal, 0, vertical);
		}
		// Force relativily applied to camera's field of view
		forceToApply = GetVectorRelativeToObject(forceToApply, cameraController.CurrentCamera().transform);
		if (!forceToApply.Equals(Vector3.zero)) {
			rigBody.AddForce(forceToApply * (speedModifier * speed), ForceMode.Force);
			Rotating(forceToApply.x, forceToApply.z);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Changing character's state
		if ((characterState.Equals(State.Jumping)) & (CharacterIsGrounded())) {
			// Jumping => Standing
			characterState = State.Standing;
			GetComponent<MultipleJumpAbility>().jumpNumber = 0;  //CAMBIAR!
		}
//		if ((characterState.Equals(State.Standing)) && (Input.GetButtonDown("Crouch"))) {
//			// Standing => Crouching
//			characterState = State.Crouching;
//			standingColl.enabled = false;
//			crouchingColl.enabled = true;
//			transform.Translate(new Vector3(0, -0.5f,0));
//			transform.localScale -= new Vector3(0,0.5f,0); // CAMBIAR! No se debe modificar el tamaño del objeto
//		} else if ((characterState.Equals(State.Crouching)) && (Input.GetButtonDown("Crouch"))) {
//			// Crouching => Standing
//			characterState = State.Standing;
//			standingColl.enabled = true;
//			crouchingColl.enabled = false;
//			transform.Translate(new Vector3(0, 0.5f,0));
//			transform.localScale += new Vector3(0,0.5f,0); // CAMBIAR! No se debe modificar el tamaño del objeto
//		} else if ((characterState.Equals(State.Standing)) && (Input.GetButtonDown("Jump"))) {
//			// Standing => Jumping
//			characterState = State.Jumping;
//			startJump = true;
//		} else if ((characterState.Equals(State.Jumping)) & (CharacterIsGrounded())) {
//			// Jumping => Standing
//			characterState = State.Standing;
//			GetComponent<MultipleJumpAbility>().jumpNumber = 0;  //CAMBIAR!
//		}
	}
}
