using UnityEngine;
using System.Collections;

/* Class to control the movement of a character getting player's controls from InputManager */
public class CharacterMovement : MonoBehaviour {
	// Speed & impulse for each possible state
	public float extraGravity= 1200f;
	public float turnSmoothing = 15f;
	public float groundCheckDistance = 1.45f;

    private CameraManager cameraManager;
	private Rigidbody rigBody;
	private Collider standingColl;
	private Collider crouchingColl;

	// Use this for references
	void Awake () {
		cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        rigBody = GetComponent<Rigidbody>();
        Collider[] colliders = GetComponents<Collider>();
        standingColl = colliders[0];
        crouchingColl = colliders[1];
    }

	// Use this for initialization
	void Start () {
		standingColl.enabled = true;
		crouchingColl.enabled = false;
	}

    /// <summary>
    /// Comprueba si el personaje tiene algún objeto bajo sus pies mediante el lanzamiento de hasta 5 rayos
    /// </summary>
    /// <returns></returns>
    public bool CharacterIsGrounded() {
        bool grounded = false;
        int rayCnt = 0;
        Vector3 ray = new Vector3();

        do {
            // Elige el rayo a lanzar
            switch (rayCnt) {
                case 0:
                    ray = transform.position + (Vector3.down * 0.2f);
                    break;
                case 1:
                    ray = transform.position + (Vector3.down * 0.2f) + (Vector3.forward * 0.3f);
                    break;
                case 2:
                    ray = transform.position + (Vector3.down * 0.2f) + (Vector3.back * 0.3f);
                    break;
                case 3:
                    ray = transform.position + (Vector3.down * 0.2f) + (Vector3.left * 0.4f);
                    break;
                case 4:
                    ray = transform.position + (Vector3.down * 0.2f) + (Vector3.right * 0.4f);
                    break;
            }
            // helper to visualise the ground check ray in the scene view
            #if UNITY_EDITOR
            Debug.DrawLine(ray, ray + (Vector3.down * groundCheckDistance), Color.blue, 10000);
            #endif
            // Lanza el rayo y comprueba si colisiona con otro objeto
            grounded = (Physics.Raycast(ray, Vector3.down, groundCheckDistance));
            rayCnt += 1;
            Debug.Log(rayCnt + ":" + ray);
        } while ((!grounded) && (rayCnt < 5));

        return grounded;
	}

	/// <summary>
	/// Indica al script que el botón de agachado ha sido pulsado
	/// </summary>
	public void Crouch () {
		// Standing => Crouching
		standingColl.enabled = false;
		crouchingColl.enabled = true;
		transform.Translate(new Vector3(0, -0.5f,0));
		transform.localScale -= new Vector3(0,0.5f,0); // CAMBIAR! No se debe modificar el tamaño del objeto
	}

    /// <summary>
    /// Provoca un efecto de gravedad extra sobre el personaje
    /// </summary>
	public void ExtraGravity() {
		// Extra gravity to fall down quickly
		rigBody.AddForce(new Vector3(0, -1 * extraGravity, 0), ForceMode.Force);
	}

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

	/// <summary>
	/// Indica al script que el botón de salto ha sido pulsado
	/// </summary>
	public void Jump (float jumpImpulse) {
		rigBody.AddForce(new Vector3(0, jumpImpulse, 0), ForceMode.Force);
	}

	public void MoveCharacterAxes (float horizontal, float vertical, float speed, Vector3 normal) {
		Vector3 forceToApply = new Vector3();

		if ((horizontal != 0f) || (vertical != 0f)) {
			if (Mathf.Abs(horizontal) > Mathf.Abs(vertical)) {
				normal.z = 0;
				if (normal.x > 0) {
					normal.x *= -horizontal;
				} else if(normal.x < 0) {
					normal.x *= horizontal;
				}
			} else {
				normal.x = 0;
				if (normal.z > 0) {
					normal.z *= -vertical;
				} else if (normal.z < 0) {
					normal.z *= vertical;
				}
			}
			forceToApply = normal.normalized;
		}
		if (!forceToApply.Equals(Vector3.zero)) {
			rigBody.AddForce(forceToApply * speed, ForceMode.Force);
		}
	}

	public void MoveCharacterNormal (float horizontal, float vertical, float speed) {
		Vector3 forceToApply = new Vector3();

		if ((horizontal != 0f) || (vertical != 0f)) {
			forceToApply += new Vector3(horizontal, 0, vertical);
			forceToApply = GetVectorRelativeToObject(forceToApply, cameraManager.CurrentCamera().transform);
		}
		if (!forceToApply.Equals(Vector3.zero)) {
			rigBody.AddForce(forceToApply * speed, ForceMode.Force);
			Rotating(forceToApply.x, forceToApply.z);
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

	public void Stand() {
		// Crouching => Standing
		standingColl.enabled = true;
		crouchingColl.enabled = false;
		transform.Translate(new Vector3(0, 0.5f,0));
		transform.localScale += new Vector3(0,0.5f,0); // CAMBIAR! No se debe modificar el tamaño del objeto
	}
}