using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutScene : MonoBehaviour {

    public GameObject cutSceneCamera;

    public bool timed = false;

    public float cutSceneTime=1;

    public void Start()
    {

    }

    public void BeginCutScene(CutSceneManager.FadeInOutAction method)
    {
        if(timed)
        {
            CutSceneManager.StartCutScene(cutSceneCamera,method,cutSceneTime);
        }
        else
        {
            CutSceneManager.StartCutScene(cutSceneCamera, method);
        }
    }

  


}

