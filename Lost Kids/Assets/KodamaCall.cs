using UnityEngine;
using System.Collections;

public class KodamaCall : MonoBehaviour {
    public float callInterval = 3000f;

    private AudioSource callSound;

    void OnEnable() {
        Invoke("MakeCall", callInterval);
    }

    void OnDisable() {
        CancelInvoke("MakeCall");
    }

    // Use this for references
    void Awake() {
        callSound = GetComponent<AudioSource>();
    }

    void MakeCall() {
        callSound.Play();
        Invoke("MakeCall", callInterval + Random.Range(-callInterval / 10f, callInterval / 10f));
    }
}
