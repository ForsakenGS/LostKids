using UnityEngine;
using System.Collections;

public class KodamaHead : MonoBehaviour {


    public float rotationAngle = 50f;
    public float rotationTime=1.5f;

    public float rotationInterval=2f;

    public float shakeChance = 0.5f;

    public float shakeAmount = 15f;
    public float shakeTime=0.5f;

    Vector3 baseRotation;
    Vector3 leftRotation;
    Vector3 rightRotation;
    Vector3 shakeRotation;


    bool wasLeft = false;

    void OnEnable()
    {
       RotateLeft();
    }

    void OnDisable()
    {
        CancelInvoke();
        transform.rotation = Quaternion.Euler(baseRotation);
    }
	// Use this for initialization
	void Awake () {
        baseRotation = new Vector3(0, 180, 0);
        leftRotation = new Vector3(0, 180, rotationAngle);
        rightRotation = new Vector3(0, 180, -rotationAngle);
        shakeRotation = new Vector3(0, 0, shakeAmount);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void RotateLeft()
    {
        wasLeft = true;
        iTween.RotateTo(gameObject,iTween.Hash("rotation", leftRotation,"time", rotationTime,"easetype",iTween.EaseType.easeInOutSine));
        if(Random.value<shakeChance)
        {
            Invoke("Shake", rotationTime);
        }
        else
        {
            Invoke("RotateRight", rotationTime + NextInterval());
        }
    }

    void RotateRight()
    {
        wasLeft = false;
        iTween.RotateTo(gameObject, iTween.Hash("rotation", rightRotation, "time", rotationTime, "easetype", iTween.EaseType.easeInOutSine));
        if (Random.value < shakeChance)
        {
            Invoke("Shake", rotationTime);
        }
        else
        {
            Invoke("RotateLeft", rotationTime + NextInterval());
        }
    }

    void RestoreRotation()
    {
        iTween.RotateTo(gameObject, baseRotation, rotationTime);
        if(!wasLeft)
        {
            Invoke("RotateLeft", rotationTime + NextInterval());
        }
        else
        {
            Invoke("RotateRight", rotationTime + NextInterval());
        }
    }

    void Shake()
    {
        iTween.ShakeRotation(gameObject,shakeRotation, shakeTime);
        Invoke("RestoreRotation", shakeTime + NextInterval());
    }

    private float NextInterval()
    {
        return rotationInterval + Random.Range(-rotationInterval / 20f, +rotationInterval / 20f);
    }
}
