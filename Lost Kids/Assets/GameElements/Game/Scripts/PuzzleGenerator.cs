using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PuzzleGenerator : MonoBehaviour {

    public GameObject[] puzzles;

    public GameObject[] ways;

    public int numPuzzles;

	// Use this for initialization
	void Start () {

        int rnd = 0;
        Vector3 exit = transform.position;

        for (int i = 0; i < numPuzzles; i++) {
            rnd = Random.Range(0, puzzles.Length);
            GameObject puzzle = (GameObject) GameObject.Instantiate(puzzles[rnd], exit, Quaternion.identity);
            exit = puzzle.transform.FindChild("Exit").transform.position;
        }
	}

	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.G)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
	}


}
