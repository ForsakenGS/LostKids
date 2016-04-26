using UnityEngine;
using System.Collections;

public class KillerSphere : MonoBehaviour {
	public int force;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private RockController rockController;
    private Rigidbody rigidBody;

    public float lifeSpan = 2;

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        rigidBody = GetComponent<Rigidbody>();
        rockController = GetComponent<RockController>();

    }
	
	public void Launch () {
		rigidBody.AddForce (force * transform.forward);
        rockController.Invoke("Reset", lifeSpan);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Reset()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<CharacterStatus>().Kill();
            rockController.Reset();
            
        }
    }


}

