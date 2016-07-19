using UnityEngine;
using System.Collections;
using System;

public class BoulderTrap : AbstractTrap {

    public int force;

    public float lifeSpan = 2;

    public bool continuousLaunch;

    public float initialDelay = 0;
    public float reLaunchDelay = 1;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private RockController rockController;
    private Rigidbody rigidBody;
    private Renderer renderer;


    void OnEnable()
    {
        if (fireOnEnable)
        {
            FireTrap();
        }
    }

    void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        rigidBody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        renderer.enabled = false;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public override void Activate()
    {
        DisableTrap();
    }

    public override void CancelActivation()
    {
        EnableTrap();
    }

    public override void DisableTrap()
    {
        isEnabled = false;
        active = false;

    }

    public override void EnableTrap()
    {
        isEnabled = true;

    }

    public override void FireTrap()
    {
        
        if (isEnabled)
        {
            Invoke("Launch", initialDelay);
        }

    }

    public override void FireTrapOneShot()
    {
        
    }

    public void Launch()
    {
        if (isEnabled && !active)
        {
            active = true;
            renderer.enabled = true;
            rigidBody.isKinematic = false;
            rigidBody.AddForce(force * transform.forward);
            Invoke("Reset", lifeSpan);
        }
    }

    public void Reset()
    {
        active = false;

        renderer.enabled = false;
        rigidBody.isKinematic = true;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        if (continuousLaunch)
        {
            Invoke("Launch", reLaunchDelay);
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //CancelInvoke();
            col.gameObject.GetComponent<CharacterStatus>().Kill();
            //Reset();
        }
    }


}
