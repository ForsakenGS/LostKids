using UnityEngine;
using System.Collections;

public class FlockMember : MonoBehaviour {

    private GameObject Controller;
    private bool inited = false;
    private float minVelocity;
    private float maxVelocity;
    private float randomness;
    private GameObject flockLeader;

    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        StartCoroutine("FlockSteeringBehaviour");
    }

    IEnumerator FlockSteeringBehaviour()
    {
        while (true)
        {
            if (inited)
            {
                rigidBody.velocity = rigidBody.velocity + Calc() * Time.deltaTime;

                // enforce minimum and maximum speeds for the boids
                float speed = rigidBody.velocity.magnitude;
                if (speed > maxVelocity)
                {
                    rigidBody.velocity = rigidBody.velocity.normalized * maxVelocity;
                }
                else if (speed < minVelocity)
                {
                    rigidBody.velocity = rigidBody.velocity.normalized * minVelocity;
                }
            }

            float waitTime = Random.Range(0.3f, 0.5f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Vector3 Calc()
    {
        Vector3 randomize = new Vector3((Random.value * 2) - 1, (Random.value * 2) - 1, (Random.value * 2) - 1);

        randomize.Normalize();
        FlockingManager boidController = Controller.GetComponent<FlockingManager>();
        Vector3 flockCenter = boidController.flockCenter;
        Vector3 flockVelocity = boidController.flockVelocity;
        Vector3 follow = flockLeader.transform.position;

        flockCenter = flockCenter - transform.position;
        flockVelocity = flockVelocity - rigidBody.velocity;
        follow = follow - transform.position;

        return (flockCenter + flockVelocity + follow * 2 + randomize * randomness);
    }

    public void SetController(GameObject theController)
    {
        Controller = theController;
        FlockingManager boidController = Controller.GetComponent<FlockingManager>();
        minVelocity = boidController.minVelocity;
        maxVelocity = boidController.maxVelocity;
        randomness = boidController.randomness;
        flockLeader = boidController.leader;
        inited = true;
    }
}
