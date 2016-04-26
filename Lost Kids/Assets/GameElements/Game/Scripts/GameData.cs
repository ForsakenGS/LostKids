using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData  {

    [SerializeField]
    List<string> levelsCompleted= new List<string>();
    
    [SerializeField]
    List<string> collectibles= new List<string>();

    [SerializeField]
    List<LevelData> levels = new List<LevelData>();

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

    public static void UpdateLevels(string level, LevelData data)
    {
        if (Instance.levelsCompleted.Contains(level))
        {
            Instance.levels[Instance.levelsCompleted.IndexOf(data.name)] = data;
        }
        else
        {
            Instance.levelsCompleted.Add(level);
            Instance.levels.Add(data);
        }

        DataManager.Save();
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

    public static LevelData LevelStart(string level)
    {
        if(Instance.levelsCompleted.Contains(level))
        {
            return Instance.levels[Instance.levelsCompleted.IndexOf(level)];
        }
        else
        {
            return new LevelData();
        }
    }

    public static LevelData getLevelData(string level)
    {
        return Instance.levels[Instance.levelsCompleted.IndexOf(level)];
    }
}
