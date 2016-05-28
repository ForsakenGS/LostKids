using UnityEngine;
using System.Collections;

public class ProximityZoom : MonoBehaviour {

    float newZoom;
    float distance;
    public float minDistance = 4.5f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider col)
    {
        if (CharacterManager.IsActiveCharacter(col.gameObject))
        {
            distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance <= minDistance)
            {
                if (distance > 0)
                {
                    float ratio = (minDistance - distance) / minDistance;
                    newZoom = 60 - 40 * ratio;

                    Camera.main.fieldOfView = newZoom;


                }
            }
        }
    }
}
