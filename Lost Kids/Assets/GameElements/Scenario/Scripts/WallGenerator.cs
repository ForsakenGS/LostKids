using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallGenerator : MonoBehaviour {


    public List<GameObject> wallPrefabs;

    private float wallSize;

    public float wallCount;

    private GameObject lastWall;

    private Vector3 newPosition;

    // Use this for initialization
    void Start () {
        wallSize = wallPrefabs[0].GetComponent<Renderer>().bounds.size.x;
        newPosition = transform.position;
        for(int i=0;i< wallCount; i++)
        {
            Instantiate(GetNewWall(), newPosition, Quaternion.identity);
            newPosition.x += wallSize;
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Obtiene un nuevo muro de la lista, eliminandolo para la siguiente iteracion 
    /// y añadiendo de nuevo el anterior para evitar repetidos
    /// </summary>
    /// <returns></returns>
    GameObject GetNewWall()
    {
        GameObject newWall = wallPrefabs[Random.Range(0, wallPrefabs.Count)];

        wallPrefabs.Remove(newWall);
        if (lastWall != null)
        {
            wallPrefabs.Add(lastWall);
        }
        lastWall = newWall;

        return newWall;
    }
}
