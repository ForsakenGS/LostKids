using UnityEngine;
using System.Collections;

public class CollectibleEffects : MonoBehaviour {
    [Header("Vertical Movement")]
    public float maxHeight;
    public float verticalSpeed;
    [Header("Rotation")]
    public float xSpeed;
    public float ySpeed;

    // Vertical movement variables 
    private float height;
    private Vector3 direction;
    // Rotation variables
    private Vector3 rotationSpeed;
    // Rigidbody reference
    private Rigidbody rigbody;

    void Awake() {
        rigbody = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start() {
        height = 0.0f;
        direction = Vector3.up;
        rotationSpeed = new Vector3(0, ySpeed, 0);
    }

    void OnCollisionEnter(Collision col) {
        if (!rigbody.isKinematic) {
            rigbody.isKinematic = true;
            rigbody.useGravity = false;
        }
    }

    void OnEnable() {
        height = 0.0f;
    }

    // Update is called once per frame
    void Update() {
        // Comprueba si le afecta la gravedad
        if (!rigbody.useGravity) {
            // Comprueba dirección del movimiento vertical
            if ((direction.Equals(Vector3.up)) && (height > maxHeight)) {
                // Altura máxima, cambio de dirección
                direction = Vector3.down;
            } else if ((direction.Equals(Vector3.down)) && (height < 0)) {
                // Altura mínima, cambio de dirección
                direction = Vector3.up;
            }
            // Movimiento vertical
            Vector3 translation = direction * verticalSpeed * Time.deltaTime;
            height += direction.y * translation.magnitude;
            transform.Translate(translation);
            // Rotación
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}