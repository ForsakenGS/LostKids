using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData  {

    [SerializeField]
    List<string> levelsCompleted= new List<string>();
    
    [SerializeField]
    List<string> collectibles= new List<string>();

    private static GameData instance;

    public static GameData Instance
    {
          get { return instance ?? (instance = new GameData()); }
    }
    
    public static void SetInstance(GameData inst)
    {
        instance = inst;
    }

    protected GameData()
    { 

        collectibles = new List<string>();
        levelsCompleted = new List<string>();
    }

    public static void UpdateCollectibles(string id)
    {
        if (!Instance.collectibles.Contains(id))
        {
            Instance.collectibles.Add(id);
        }

        DataManager.Save();
        Debug.Log("Conseguido colectionable " + id);
    }

    public static void UpdateLevels(string level)
    {
        if (!Instance.levelsCompleted.Contains(level))
        {
            Instance.levelsCompleted.Add(level);
        }
    }

    /// <summary>
    /// Devuelve si el collectible con el id introducido ya ha sido adquirido
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool AlreadyCollected(string id)
    {

        return Instance.collectibles.Contains(id);
    }
}
