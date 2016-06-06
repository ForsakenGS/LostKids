using UnityEngine;
using System.Collections;

public class KillerSphere : MonoBehaviour {
	public int force;

    public bool isEnabled = true;

    [HideInInspector]
    public bool active = false;

    public float lifeSpan = 2;

    public bool continuousLaunch;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private RockController rockController;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        rigidBody = GetComponent<Rigidbody>();
    }

    public void Activate()
    {
        active = true;
        Launch();

    }

    public void DeActivate()
    {
        active = false;
        Reset();
    }

    public void Launch () {
        if (active)
        {
            rigidBody.isKinematic = false;
            rigidBody.AddForce(force * transform.forward);
            rockController.Invoke("Reset", lifeSpan);
        }
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Reset()
    {
        rigidBody.isKinematic = true;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        if(continuousLaunch)
        {
            Launch();
        }
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<CharacterStatus>().Kill();
            Reset();        
        }
    }


}

