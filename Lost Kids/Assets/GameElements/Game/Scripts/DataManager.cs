using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public static class DataManager
{

    public static List<GameData> savedGames = new List<GameData>();

    public static GameData currentGame;

    public static bool Save()
    {
        bool saved = false;
        FileStream file=null;
        if (!savedGames.Contains(currentGame))
        {
            savedGames.Add(currentGame);
        }
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + "/savedGames.tlk");
            bf.Serialize(file, savedGames);
            saved = true;
        }catch(System.Exception e)
        {
            Debug.Log("Error guardando fichero: \n" + e);
        }
        if (file != null)
        {
            file.Close();
        }

        return saved;
    }

    /// <summary>
    /// Carga las partisas guardadas desde fichero
    /// </summary>
    /// <returns>Devuelve true cuando la carga sea correcta</returns>
    public static bool Load()
    {
        bool loaded = false;
        if (File.Exists(Application.persistentDataPath + "/savedGames.tlk"))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savedGames.tlk", FileMode.Open);
                savedGames = (List<GameData>)bf.Deserialize(file);
                file.Close();
                loaded = true;
            }catch(System.Exception e)
            {
                Debug.Log("Error cargando fichero guardado \n" + e);
            }
        }

        return loaded;
    }

    /// <summary>
    /// Crea un nuevo save vacio
    /// </summary>
    public static void NewSave()
    {
        currentGame = GameData.Instance;

    }

    /// <summary>
    /// Selecciona la partida actual de entre las partidas guardadas
    /// </summary>
    /// <param name="index"></param>
    public static void SetCurrentGame(int index)
    {
        if (savedGames[index] != null)
        {
            currentGame = savedGames[index];
            GameData.SetInstance(currentGame);
        }
    }


    /// <summary>
    /// Copia la partida actual al save introducido por parametro
    /// </summary>
    /// <param name="dest"></param>
    public static void CloneSave(int dest)
    {
        savedGames[dest] = currentGame;
    }

    /// <summary>
    /// Devuelve la referencia a la informacion de la partida actual
    /// </summary>
    /// <returns></returns>
    public static GameData GetCurrentGame()
    {
        return currentGame;
    }
}