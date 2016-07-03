using UnityEngine;
using System.Collections;

public class KodamaCall : MonoBehaviour {


    public float callInterval = 3000f;

    public  AudioSource callSound;

    void OnEnable()
    {
        Invoke("MakeCall", callInterval);
    }

    void OnDisable()
    {
        CancelInvoke("MakeCall");
    }

    void Awake()
    {
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void MakeCall()
    {
        callSound.Play();
        Invoke("MakeCall", callInterval + Random.Range(-callInterval / 10f, callInterval / 10f));
    }
}
