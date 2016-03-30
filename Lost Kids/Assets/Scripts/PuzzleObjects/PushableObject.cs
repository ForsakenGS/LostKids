using UnityEngine;
using System.Collections;

public class PushableObject : MonoBehaviour {


    protected bool onUse;

    public bool pushOnly;

    private Vector3 size;

    private Rigidbody rigidBody;

    private PushAbility pushAbility;



	// Use this for initialization
	void Start () { 

        size = GetComponent<Collider>().bounds.size;
        rigidBody = GetComponent<Rigidbody>();


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Release()
    {
        Debug.Log("Sueltame!");
        if (IsGrounded())
        {
            rigidBody.isKinematic = true;
            Debug.Log("Vuelve a su estado!");
        } 
        pushAbility = null;
    }

    public void Grab(PushAbility pa)
    {
        rigidBody.isKinematic = false;
        pushAbility = pa;
    }

    private bool IsGrounded()
    {
        bool grounded = false;
        int rayCnt = 0;
        Vector3 ray = new Vector3();

        do
        {
            // Elige el rayo a lanzar
            switch (rayCnt)
            {
                case 0:
                    ray = transform.position + (Vector3.down * size.y/2);
                    break;
                case 1:
                    ray = transform.position + (Vector3.down * size.y / 2) + (Vector3.forward * size.z/2);
                    break;
                case 2:
                    ray = transform.position + (Vector3.down * size.y / 2) + (Vector3.back * size.z / 2);
                    break;
                case 3:
                    ray = transform.position + (Vector3.down * size.y / 2) + (Vector3.left * size.x / 2);
                    break;
                case 4:
                    ray = transform.position + (Vector3.down * size.y / 2) + (Vector3.right * size.x / 2);
                    break;
            }
            ray.y += 0.05f;
            Debug.DrawLine(ray, ray + (Vector3.down ), Color.blue, 10000);
            // Lanza el rayo y comprueba si colisiona con otro objeto
            grounded = (Physics.Raycast(ray, Vector3.down, 0.1f));
            rayCnt += 1;

        } while ((!grounded) && (rayCnt < 5));

        return grounded;
    }

    void OnCollisionExit(Collision col)
    {
        if(pushAbility!=null && col.transform.position.y<transform.position.y)
        {
            pushAbility.ReleaseObject();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(pushAbility == null && IsGrounded())
        {
            rigidBody.isKinematic = true;
        }
    }

}
