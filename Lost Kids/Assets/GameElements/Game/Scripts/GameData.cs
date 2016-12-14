using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData  {

    [SerializeField]
    List<string> levelsCompleted= new List<string>();

    [SerializeField]
    List<string> unlockedLevels = new List<string>();

    [SerializeField]
    List<Collection> collections= new List<Collection>();

    [SerializeField]
    List<string> solvedRooms = new List<string>();

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

        collections = new List<Collection>();
        levelsCompleted = new List<string>();
        solvedRooms = new List<string>();
        unlockedLevels = new List<string>();
        unlockedLevels.Add(GameLevels.Tutorial1.ToString());
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

    public static void CompleteLevel(string level)
    {
        if (!Instance.levelsCompleted.Contains(level))
        {
            Instance.levelsCompleted.Add(level);

        }
    }

    public static void UnlockLevel(string level)
    {
        if (!Instance.unlockedLevels.Contains(level))
        {
            Instance.unlockedLevels.Add(level);

        }
    }

    public static void UpdateRooms(string id)
    {
        if (!Instance.solvedRooms.Contains(id))
        {
            Instance.solvedRooms.Add(id);
        }

        DataManager.Save();
        Debug.Log("Room superada " + id);
    }

    public static void AddCollectible(Collections name, CollectionPieces piece)
    {
        /*
        //Se busca si ya existe esa coleccion en la partida
        Collection col = GetCollection(name);
        if (col != null)
        {
            col.AddPiece(piece);
        }
        //Si no existe, se crea
        else
        {
            Collection coll = new Collection(name);
            //Se añade la nueva pieza de la coleccion
            coll.AddPiece(piece);
            Instance.collections.Add(coll);
        }
        */
        PlayerPrefs.SetInt(name.ToString(), 1);
        PlayerPrefs.SetInt(name.ToString() + piece.ToString(), 1);

        Debug.Log("Obtenido collectionable: " + name.ToString() + piece.ToString());

        //DataManager.Save();
    }

    /// <summary>
    /// Devuelve si la pieza de la coleccion ya se ha recogido
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool AlreadyCollected(Collections name, CollectionPieces piece)
    {
        bool collected = false;
        //Se busca si ya existe esa coleccion en la partida
        //Collection col = GetCollection(name);
        if (PlayerPrefs.HasKey(name.ToString()+piece.ToString()))
        {
            collected = true;
        }
        return collected;
    }

    public static Collection GetCollection(Collections name)
    {
        Collection col = null;

        foreach (Collection c in Instance.collections)
        {
            if (c.collection.Equals(name))
            {
                col = c;
                break;
            }
        }
        return col;
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

    public static bool LevelUnlocked(string level)
    {
        return Instance.unlockedLevels.Contains(level);
    }
}
