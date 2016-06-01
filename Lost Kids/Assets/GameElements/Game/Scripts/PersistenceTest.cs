using UnityEngine;
using System.Collections;

public class PersistenceTest : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        if(!DataManager.Load())
        {
            DataManager.NewSave();
        }
        else
        {
            DataManager.SetCurrentGame(0);
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
