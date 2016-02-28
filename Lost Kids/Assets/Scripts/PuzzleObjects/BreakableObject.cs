using UnityEngine;
using System.Collections;
using System;

public abstract class BreakableObject : MonoBehaviour, IBreakable {


    public int maxHitPoints;
    protected int currentHitPoints;

    public void Break()
    {
        Destroy(gameObject);
    }

    public void TakeHit()
    {
        currentHitPoints--;
        if(currentHitPoints <= 0)
        {
            Break();
        }
    }

    // Use this for initialization
    void Start () {
        currentHitPoints = maxHitPoints;


    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
