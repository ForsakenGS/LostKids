using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LevelData  {

    [SerializeField]
    public string name;

    [SerializeField]
    public float bestTime=float.MaxValue;

    [SerializeField]
    public List<string> collectibles;

    [SerializeField]
    public List<string> levelStructure;

    [SerializeField]
    public int currentCheckPoint; 
    /*
    [HideInInspector]
    public float startTime;
    [HideInInspector]
    public float finishTime;
    */


    public void StartTimer()
    {
        //startTime = Time.time;
    }

    public void EndTimer()
    {
        /*
        finishTime = Mathf.RoundToInt(Time.time-startTime);
        if(finishTime<bestTime)
        {
            bestTime = finishTime;
            bestTime = Mathf.RoundToInt(bestTime);
        }
        */
    }

    public void setCollectibles(List<string> list)
    {
        collectibles = list;
    }


}
