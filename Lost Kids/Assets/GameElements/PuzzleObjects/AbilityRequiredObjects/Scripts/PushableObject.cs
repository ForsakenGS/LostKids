using UnityEngine;
using System.Collections;

public class PushableObject : MonoBehaviour {


    protected bool onUse;

    public bool pushOnly;

    private Vector3 size;

    private Rigidbody rigidBody;

    private PushAbility pushAbility;


    public float groundedRayDistance = 1.2f;


    // Use this for initialization
    void Start() {

        size = GetComponent<BoxCollider>().bounds.size;
        rigidBody = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update() {
        if(pushAbility==null && IsGrounded())
        {
            rigidBody.isKinematic = true;
        }
    }

    public void Release() {
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (pushAbility != null)
        {
            pushAbility.gameObject.GetComponent<AbilityController>().DeactivateActiveAbility(false);
            pushAbility = null;
        }
    }

    public void Grab(PushAbility pa) {
        rigidBody.isKinematic = false;
        gameObject.layer = LayerMask.NameToLayer("PushableObjects");
        pushAbility = pa;
    }

    private bool IsGrounded() {
        bool grounded = false;
        int rayCnt = 0;
        Vector3 ray = new Vector3();

        do {
            // Elige el rayo a lanzar
            switch (rayCnt) {
                case 0:

                    ray = transform.position;
                    break;
                case 1:
                    ray = transform.position + (Vector3.forward * size.z / 2);
                    break;
                case 2:
                    ray = transform.position + (Vector3.back * size.z / 2);
                    break;
                case 3:
                    ray = transform.position + (Vector3.left * size.x / 2);
                    break;
                case 4:
                    ray = transform.position + (Vector3.right * size.x / 2);
                    break;
            }

            if(gameObject.name.Contains("GiantRock"))
            {
                ray = transform.position + Vector3.down * groundedRayDistance;
            }
            //ray.y += 0.05f;
            Debug.DrawLine(ray, ray + (Vector3.down*0.1f ), Color.blue, 10000);
            // Lanza el rayo y comprueba si colisiona con otro objeto
            grounded = (Physics.Raycast(ray, Vector3.down, 0.1f));
            rayCnt += 1;

        } while ((!grounded) && (rayCnt < 5));

        return grounded;
    }

    void OnCollisionExit(Collision col) {
        if (!CharacterManager.IsActiveCharacter(col.gameObject) && pushAbility != null && !IsGrounded() && !col.gameObject.CompareTag("Breakable") && !col.gameObject.CompareTag("KappaRunway")) {
            pushAbility.gameObject.GetComponent<AbilityController>().DeactivateActiveAbility(false);
        }
    }

    void OnCollisionEnter(Collision col) {
        if (pushAbility == null && IsGrounded()) {
            rigidBody.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Player")) {
            col.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.CompareTag("Player") && col.transform.parent == transform) {
            col.transform.parent = null;
        }
    }

}
