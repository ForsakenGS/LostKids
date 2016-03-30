using UnityEngine;
using System.Collections;


public class FlockingManager : MonoBehaviour
{
    public float minVelocity = 5;
    public float maxVelocity = 20;
    public float randomness = 1;
    public int flockSize = 20;
    public GameObject prefab;
    public GameObject leader;

    public Vector3 flockCenter;
    public Vector3 flockVelocity;

    private GameObject[] members;

    void Start()
    {
        members = new GameObject[flockSize];
        Collider spawnZone = GetComponent<Collider>();
        for (var i = 0; i < flockSize; i++)
        {
            Vector3 position = new Vector3(
                Random.value * spawnZone.bounds.size.x,
                leader.transform.position.y,
                Random.value * spawnZone.bounds.size.z
            ) - spawnZone.bounds.extents;

            GameObject boid = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            boid.transform.parent = transform;
            boid.transform.position = position;
            boid.GetComponent<FlockMember>().SetController(gameObject);
            members[i] = boid;
        }
    }

    void Update()
    {
        Vector3 theCenter = Vector3.zero;
        Vector3 theVelocity = Vector3.zero;

        foreach (GameObject member in members)
        {
            theCenter = theCenter + member.transform.position;
            theVelocity = theVelocity + member.GetComponent<Rigidbody>().velocity;
        }

        flockCenter = theCenter / (flockSize);
        flockVelocity = theVelocity / (flockSize);
    }
}

