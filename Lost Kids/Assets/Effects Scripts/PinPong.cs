using UnityEngine;
using System.Collections;

public class PinPong : MonoBehaviour {

    public float delay = 0.0f;
    public float floatSpeed = 0.1f;
    public float floatDistance = 1;
    private Vector3 basePosition;

	// Use this for initialization
	void Awake () {
	    basePosition = transform.localPosition;
	}

    void OnEnable()
    {
        Invoke("MoveUp", delay);
    }

    void OnDisable()
    {
        transform.position = basePosition;
        iTween.Stop(gameObject);
    }

    // Update is called once per frame
    void Update () {
	
	}

    void MoveUp()
    {
        iTween.MoveTo(gameObject, iTween.Hash("y", transform.position.y + floatDistance, "speed", floatSpeed,
            "easeType", iTween.EaseType.easeInOutSine, "oncomplete", "MoveDown", "islocal", true));
    }

    void MoveDown()
    {
        iTween.MoveTo(gameObject, iTween.Hash("y", basePosition.y, "speed", floatSpeed,
            "easeType", iTween.EaseType.easeInOutSine, "oncomplete", "MoveUp", "islocal", true));
    }
}
