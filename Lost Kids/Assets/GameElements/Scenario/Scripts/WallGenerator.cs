using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallGenerator : MonoBehaviour {

    public enum Direction { Horizontal, Vertical }
    [Header("Wall Settings")]
    public Direction direction;

    public float wallCount;

    [Header("Walls Collection")]
    public List<GameObject> wallPrefabs;

    private float wallSize;

    private GameObject lastWall;

    private Vector3 newPosition;

    

    // Use this for initialization
    void Awake () {
        GameObject newWall;
        wallSize = wallPrefabs[0].GetComponent<Renderer>().bounds.size.x;
        Vector3 offset;

        if (direction.Equals(Direction.Horizontal)) {
            offset = new Vector3(1, 0, 0.5f);
        } else {
            offset = new Vector3(0.5f, 0, 1);
        }
        
        newPosition = transform.position + offset;

        for(int i=0;i< wallCount; i++)
        {

            if (direction.Equals(Direction.Horizontal)) {
                newWall= Instantiate(GetNewWall(), newPosition, Quaternion.identity) as GameObject;
                newPosition.x += wallSize;
            } else  {
                newWall = Instantiate(GetNewWall(), newPosition, Quaternion.AngleAxis(90,Vector3.up)) as GameObject;
                newPosition.z += wallSize;
            }
            newWall.transform.parent = transform;
            
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
