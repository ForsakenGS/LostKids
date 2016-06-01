using UnityEngine;
using System.Collections;

public class KodamaInvisibility : MonoBehaviour {

    public Transform body;
    public Transform face;
    public float minDistance=3;
    private float distance;
    private Color bodyColor;
    private Color faceColor;
    private float newAlpha;
	// Use this for initialization
	void Start () {
        bodyColor = body.GetComponent<Renderer>().material.color;
        faceColor= body.GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider col)
    {
        if(CharacterManager.IsActiveCharacter(col.gameObject))
        {
            distance = Vector3.Distance(transform.position, col.transform.position);
            if(distance<=minDistance)
            {

                if (distance > 0)
                {
                    newAlpha = Mathf.InverseLerp(minDistance, 0.0f,
        distance);
                    

                }
                else
                {
                    newAlpha = 1;
                }
                bodyColor.a = newAlpha;
                body.GetComponent<Renderer>().material.color = bodyColor;

                faceColor.a = newAlpha;
                face.GetComponent<Renderer>().material.color = faceColor;
            }
        }
    }
}
