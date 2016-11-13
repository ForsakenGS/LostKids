using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System;
using UnityEngine.SceneManagement;


public enum Worlds { World1, World2, World3 };

public class LevelGenerator : MonoBehaviour {



    //Mundo al que corresponde el nivel
    public Worlds world;

    //Numero de habitaciones en el nivel
    public int totalRooms;

    //Listado de todas las rooms disponibles
    public GameObject[] listOfRooms;

    //Listado de todas las ways disponibles
    public GameObject[] listOfWays;

    //Tiempo(pasos) que se impide volver a aparecer una room
    public int banTime;


    //Rangos de dificultad para la busqueda de rooms candidatas
    public int initialMinDifficulty = 1;
    public int initialMaxDifficulty = 10;

    //Ways concretas que deben aparecer en cada paso del nivel
    //Cuando no se asigne un way concreto, se busca uno aleatorio
    public GameObject[] specialWays;

    //Diferencia maxima entre los personajes necesarios para los puzzles
    //Cuando un personaje aparece como necesario mas que otro, se debe priorizar el menos usado
    public int maxRequiredCharacterDiff=2;

    //Diccionario de habitaciones/pasillos baneados, y el tiempo que les queda
    private Dictionary<RoomSettings, int> bannedRooms;

    //Estructura de las salas y pasillos del nivel
    private static List<GameObject> levelStructure;

    //Rangos de dificultad para la busqueda de rooms candidatas
    private int minDifficulty=1;
    private int maxDifficulty =10;


    //Crecimiento de dificultad minima y maxima en cada paso
    public float minDifficultyIncrease = 1.5f;
    public float maxDifficultyIncrease = 1f;

    //Numero de rooms que hay en en cada momento
    private int actualRooms;

    //Contador de tags que se han incluido hasta el momento

    private Dictionary<PuzzleTags,int> actualTagsCount;



    //Contador de aparicion de los personajes requeridos para los puzzles
    private Dictionary<CharacterName, int> actualCharacterTags;

    //Variable donde se almacenaran las diferentes restricciones a la busqueda de una nueva room
    System.Func<RoomSettings, bool> constraints;
    //Listado donde se almacenan por separado todas las restricciones, para construir la constraint final
    List<System.Func<RoomSettings,bool>> constraintsList;

    
    //Listado de referencias a los scripts de las rooms (para obtener su informacion)
    private List<RoomSettings> roomsList;

    //Particulas de aparicion de la sala
    public ParticleSystem roomParticles;

    [Header("Level Goal Configuration")]
    public GameObject levelGoalPrefab;
    public string levelGoalString;

    // Use this for initialization
    void Start () {

        roomsList = new List<RoomSettings>();
        for(int i=0;i<listOfRooms.Length;i++)
        {
            roomsList.Add(listOfRooms[i].GetComponent<RoomSettings>());
        }

        GenerateLevelStructure();
        InstantiateLevel();


    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
#endif
    }


    void InstantiateLevel()
    {
        Vector3 nextRoomPosition = transform.position;
        GameObject prev = null;
        GameObject room=null;
        ParticleSystem particles;
        for (int i = 0; i < levelStructure.Count; i++)
        {  
            room=Instantiate(levelStructure[i], nextRoomPosition, Quaternion.identity) as GameObject;

            
            //Se actualiza la referencia a la siguiente habitacion del way con la nueva room
            if (prev != null && prev.GetComponent<WaySettings>() != null)
            {
                prev.GetComponent<WaySettings>().nextRoom = room.GetComponent<RoomSettings>();
            }

            //Se actualiza la referencia a la anterior habitacion del way con la anterior room
            if (prev!=null && room.GetComponent<WaySettings>()!=null)
            {
                    room.GetComponent<WaySettings>().prevRoom = prev.GetComponent<RoomSettings>();
            }
            
            if(room.GetComponent<RoomSettings>()!= null)
            {
                particles = Instantiate(roomParticles, nextRoomPosition + Vector3.right * 7, Quaternion.identity) as ParticleSystem;
                room.GetComponent<RoomSettings>().SetParticles(particles);
                TranslateRoom(room);
            }
            
            nextRoomPosition = room.transform.FindChild("Exit").transform.position;
            prev = room;
        }
        // Instancia el LevelGoal del nivel
        room = Instantiate(levelGoalPrefab, nextRoomPosition, Quaternion.identity) as GameObject;
        room.GetComponentInChildren<LevelGoal>().nextLevel = levelGoalString;
    }

    /// <summary>
    /// Metodo que genera proceduralmente la estructura del nivel y la almacena en una lista
    /// SOLO GENERA LA ESTRUCTURA, DEBE INSTANCIARSE POSTERIORMENTE
    /// </summary>
    void GenerateLevelStructure()
    {
        List<RoomSettings> candidates;
        RoomSettings selected =null;

        
        InitializeLevel();
        
        while (actualRooms < totalRooms)
        {
            if (banTime > 0)
            {
                UpdateBans(selected);
            }
            UpdateActualTags(selected);
            UpdateConstraints();
            levelStructure.Add(GetWay(actualRooms));
            candidates = GetCandidateList();
            selected = GetRandomRoom(candidates);
            levelStructure.Add(selected.gameObject);  
            actualRooms++;  
        }
    }
    /// <summary>
    /// Inicializacion de la estructura de nivel y estructuras y datos auxiliares
    /// </summary>
    void InitializeLevel()
    {
        actualRooms = 0;
        constraintsList = new List<Func<RoomSettings, bool>>();
        levelStructure = new List<GameObject>();
        bannedRooms = new Dictionary<RoomSettings, int>();

        actualCharacterTags = new Dictionary<CharacterName, int>();

        //Se inicializa la lista de personajes requeridos
        foreach(CharacterName name in Enum.GetValues(typeof(CharacterName)))
        {
            actualCharacterTags.Add(name, 0);
        }

        //Se inicializa la lista de tags de puzzles
        actualTagsCount = new Dictionary<PuzzleTags, int>();
        foreach (PuzzleTags name in Enum.GetValues(typeof(PuzzleTags)))
        {
            actualTagsCount.Add(name, 0);
        }

    }

    /// <summary>
    /// Desplaza la room para encajar la entrada
    /// </summary>
    /// <param name="room"></param>
    void TranslateRoom(GameObject room)
    {
        Vector3 entry = room.transform.Find("Entry").transform.position;
        Vector3 offset = entry - room.transform.position;
        room.transform.position -= offset;
    }

    /// <summary>
    /// Reduce en uno la duracion de las salas baneadas
    /// Si el tiempo de una sala llega a 0, se elimina del diccionario
    /// </summary>
    void UpdateBans(RoomSettings selectedRoom)
    {
        //Se disminuye en uno el contador de bans de las rooms
        foreach (RoomSettings entry in bannedRooms.Keys.ToList())
        {
            if(bannedRooms[entry]>1)
            {
                bannedRooms[entry] -= 1;
            }
            else
            {
                bannedRooms.Remove(entry);
            }
        }

        if (selectedRoom != null)
        {
            //Se añade/actualiza el ban de la nueva room
            if (!bannedRooms.ContainsKey(selectedRoom))
            {
                bannedRooms.Add(selectedRoom, banTime);
            }
            else
            {
                bannedRooms[selectedRoom] = banTime;
            }
        }
    }


    /// <summary>
    /// Actualiza los contadores de etiquetas ya aparecidas con las de la ultima room seleccionada
    /// Actualiza contadores de personajes necesarios y de tags de puzzles aparecidos hasta el momento
    /// </summary>
    /// <param name="selectedRoom">Hacitacion seleccionada cuyos tags se añaden al conjunto</param>
    void UpdateActualTags(RoomSettings selectedRoom)
    {
        if(selectedRoom!=null)
        {
            //Añade al total de apariciones las etiquetas de personajes necesarios
            foreach(CharacterName character in selectedRoom.requiredCharacters)
            {
                actualCharacterTags[character]++;
            }

            //Añade al total de apariciones las etiquetas de puzzle, agregandolas al diccionario
            //si aparecen por primera vez

            foreach (PuzzleTags tag in selectedRoom.tags)
            {
                    actualTagsCount[tag]++;
            }
        }
    }

    /// <summary>
    /// Metodo que actualiza las restricciones para la busqueda de salas candidatas
    /// Se actualizan en varios niveles: dificultad, personajes, y Tags
    /// </summary>
    void UpdateConstraints()
    {
        constraintsList.Clear();
        //Se inicializa con la restriccion de dificultad 
        //SIEMPRE DEBE EJECUTARSE EN PRIMER LUGAR
        UpdateDifficultyConstraints();

        //Se actualizan las restricciones respecto a los personajes necesarios
        //UpdateCharacterConstraints();

        //Se actualizan las restricciones respecto a las tags especificas de puzzles
        //UpdatePuzzleTagContraints();

        //Se construye la expresion conjunta
        BuildConstraints();



    }

    /// <summary>
    /// Metodo que devuelve un way ( pasillo) a instanciar, en funcion de la posicion(numero) 
    /// de la siguiente sala. Se devuelve el way especial asociado a esa posicion. 
    /// Si no se ha asignado niguno especial, devuelve uno aleatorio
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    GameObject GetWay(int room)
    {
        if(specialWays[room]!=null)
        {
            return specialWays[room];
        }
        else
        {
            return GetRandomWay();
        }
    }


    /// <summary>
    /// Metodo que actualiza las restricciones de ventana de nivel para las salas, 
    /// en base a un crecimiento establecido para los limites inferior y superior
    /// </summary>
    void UpdateDifficultyConstraints()
    {
        System.Func<RoomSettings, bool> difficultyConstraint;

        minDifficulty = (int)(actualRooms * minDifficultyIncrease) + initialMinDifficulty;
        maxDifficulty = (int)(actualRooms * maxDifficultyIncrease) + initialMaxDifficulty;

        difficultyConstraint = r => r.difficulty >= minDifficulty && r.difficulty <= maxDifficulty;

        constraintsList.Add(difficultyConstraint);
        //constraints = difficultyConstraint;
    }

    /// <summary>
    /// Actualiza las restricciones respecto a los personajes necesarios
    /// Si un personaje supera a otro en aparicion por encima de un umbral
    /// Se prioriza la aparicion del personaje menos necesitado
    /// </summary>
    void UpdateCharacterConstraints()
    {
        List<int> values = actualCharacterTags.Values.ToList();
        int maxValue = values.Max();
        int minValue = values.Min();
        int index = -1;
        if (Math.Abs(maxValue - minValue) > maxRequiredCharacterDiff)
        {
            index = values.IndexOf(minValue);

            Func<RoomSettings, bool> characterTagConstraint = r => r.requiredCharacters.Contains(actualCharacterTags.Keys.ElementAt(index));
            constraintsList.Add(characterTagConstraint);
            //constraints = PredicateBuilder.And(constraints, characterTagConstraint);
            
        }
    }

    /// <summary>
    /// Actualiza las restricciones de tags de puzzles
    /// Evita que vuelva a salir el tag mas repetido y
    /// prioriza que salga el tag menos usado
    /// </summary>
    void UpdatePuzzleTagContraints()
    {
        List<int> values = actualTagsCount.Values.ToList();
        int maxValue = values.Max();
        int minValue = values.Min();
        if(minValue!=maxValue)
        { 
            int maxUsedIndex = values.IndexOf(maxValue);
            int minUsedIndex = values.IndexOf(minValue);

            Func<RoomSettings, bool> includePuzzleTagConstraint = r => r.tags.Contains(actualTagsCount.Keys.ElementAt(minUsedIndex));
            constraintsList.Add(includePuzzleTagConstraint);

            Func<RoomSettings, bool> excludePuzzleTagConstraint = r => !r.tags.Contains(actualTagsCount.Keys.ElementAt(maxUsedIndex));
            constraintsList.Add(excludePuzzleTagConstraint);
            //constraints = PredicateBuilder.And(constraints, includePuzzleTagConstraint);

        }
    }
    /// <summary>
    /// Metodo que devuelve la lista de rooms candidatas tras aplicarle las restricciones
    /// Si no se encuentran candidatos, se van eliminando restricciones
    /// </summary>
    /// <returns>Listado de rooms candidatas</returns>
    private List<RoomSettings> GetCandidateList()
    {
        List<RoomSettings> candidates = roomsList.Where(constraints).ToList();
        while (candidates.Count<1)
        {
            constraintsList.RemoveAt(constraintsList.Count - 1);
            if (constraintsList.Count > 0)
            {
                BuildConstraints();
                candidates = roomsList.Where(constraints).ToList();
            }
            else
            {
                candidates = roomsList;
            }
        }
        return candidates;
    }

    /// <summary>
    /// Metodo que construye una sola expresion con todas las restricciones de la lista
    /// </summary>
    /// <returns></returns>
    private Func<RoomSettings, bool> BuildConstraints()
    {
        constraints = constraintsList[0];
        for(int i=1;i<constraintsList.Count; i++)
        {
            constraints.And(constraintsList[i]);
        }
        return constraints;
    }

    /// <summary>
    /// Devuelve un way (pasillo) aleatorio de la lista de pasillos posibles
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomWay()
    {
        return listOfWays[UnityEngine.Random.Range(0, listOfWays.Length)];
    }

    /// <summary>
    /// Metodo que devuelve una room aleatoria de una lista de candidatos
    /// evitando las que se encuentren baneadas
    /// </summary>
    /// <param name="candidates">lista de candidatos a elegir</param>
    /// <returns></returns>
    private RoomSettings GetRandomRoom(List<RoomSettings> candidates)
    {
        int index;
        while (candidates.Count > 1)
        {
            index = UnityEngine.Random.Range(0, candidates.Count - 1);
            if (bannedRooms.ContainsKey(candidates[index]))
            {
                candidates.Remove(candidates[index]);
            }
            else
            {
                return candidates[index];
            }
        }
        return candidates[0];
    }



    /// <summary>
    /// Metodo que se llama desde el inspector cuando se modifica algun valor
    /// Se utiliza para mantener el mismo tamaño entre las salas y pasillos
    /// </summary>
    void OnValidate()
    {
        if (specialWays.Length != totalRooms)
        {
            specialWays = copyAndResize(specialWays, totalRooms);
        }

    }


    /// <summary>
    /// Metodo auxiliar para redimensionar un array manteniendo su contenido
    /// </summary>
    /// <param name="array">array a dimensionar</param>
    /// <param name="size">nuevo tamaño</param>
    /// <returns>array redimensionado manteniendo los valores anteriores</returns>
    private GameObject[] copyAndResize(GameObject[] array, int size)
    {
        GameObject[] temp = new GameObject[size];
        Array.Copy(array, temp, Math.Min(array.Length, size));
        array = temp;
        return array;

    }

    public static List<GameObject> GetCurrentLevelStrcture()
    {
        return levelStructure;
    }


}
