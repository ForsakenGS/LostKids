using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutScene : MonoBehaviour {

    public GameObject cutSceneCamera;

    public bool timed = false;

    public float cutSceneTime=1;

    public bool alwaysShow = false;

    private bool shown = false;

    public void Start()
    {

    }

    public void BeginCutScene(CutSceneManager.FadeInOutAction method)
    {
        if (alwaysShow || !shown)
        {
            shown = true;
            if (timed)
            {
                CutSceneManager.StartCutScene(cutSceneCamera, method, cutSceneTime);
            }
            else
            {
                CutSceneManager.StartCutScene(cutSceneCamera, method);
            }
        }
    }

  


}

