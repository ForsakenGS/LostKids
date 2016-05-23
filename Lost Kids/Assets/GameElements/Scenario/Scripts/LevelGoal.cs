﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour {


    private List<GameObject> characterList;

    private HashSet<GameObject> charactersOnGoal;

    private LevelData levelData;

    public string nextLevel;
	// Use this for initialization
	void Start () {

        characterList = CharacterManager.GetCharacterList();
        charactersOnGoal = new HashSet<GameObject>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(CharacterManager.IsActiveCharacter(col.gameObject))
        {
            charactersOnGoal.Add(col.gameObject);
            CheckGoalCompleted();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (CharacterManager.IsActiveCharacter(col.gameObject))
        {
            charactersOnGoal.Remove(col.gameObject);

        }   
    }

    bool CheckGoalCompleted()
    {

        if (charactersOnGoal.Count == characterList.Count)
        {
            GameManager.CompleteLevel(SceneManager.GetActiveScene().name);
            LevelManager.LevelEnd(nextLevel);

        }
        return false;
        
    }





}
