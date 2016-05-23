using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {


    private static LevelData levelData;

    private static SceneFade fader;

    void Awake()
    {
        fader = GetComponent<SceneFade>();
    }


    void Start () {
        fader.StartScene();
        LevelStart();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void LevelStart()
    {
        /*
        levelData = GameData.LevelStart(SceneManager.GetActiveScene().name);
        levelData.name = SceneManager.GetActiveScene().name;
        CollectibleObject[] collectibleObjects = FindObjectsOfType<CollectibleObject>();
        List<string> collectibleList = new List<string>();
        for (int i = 0; i < collectibleObjects.Length; i++)
        {
            collectibleList.Add(collectibleObjects[i].id);
        }
        levelData.setCollectibles(collectibleList);
        levelData.StartTimer();
        */
    }

    public static void LevelEnd(string nextLevel)
    {
        //levelData.EndTimer();
        //GameData.UpdateLevels(levelData.name, levelData);
        fader.nextScene=nextLevel;
        fader.EndScene();
    }
}
