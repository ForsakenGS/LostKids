using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ExitApplication (){
        Application.Quit ();
	}

	public void GoToScene(string sc) {
        SceneManager.LoadScene(sc);
	}
}



	
