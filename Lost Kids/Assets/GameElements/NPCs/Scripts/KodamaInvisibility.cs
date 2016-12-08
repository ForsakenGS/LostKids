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

        if(minDistance==0)
        {
            bodyColor.a = 1;
            body.GetComponent<Renderer>().material.color = bodyColor;

            faceColor.a = 1;
            face.GetComponent<Renderer>().material.color = faceColor;

        }
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

                if (distance > 1.8)
                {
                    newAlpha = Mathf.InverseLerp(minDistance, 0.0f, distance-1.8f);
                    

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

    void OnTriggerExit(Collider col) {
        if(CharacterManager.IsActiveCharacter(col.gameObject))
        {
            newAlpha = 0;
            bodyColor.a = newAlpha;
            body.GetComponent<Renderer>().material.color = bodyColor;

            faceColor.a = newAlpha;
            face.GetComponent<Renderer>().material.color = faceColor;
        }
    }
}
