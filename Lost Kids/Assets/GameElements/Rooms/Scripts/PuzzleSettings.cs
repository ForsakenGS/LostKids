using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Flags]
public enum PuzzleTags2
{
    Platforms = 1 << 0,
    Sequence = 1 << 1,
    Levers = 1 << 2,
    DarkFear = 1 << 3,
    WaterFear = 1 << 4,
    TreeFear = 1 << 5,
    Collectibles = 1 << 6,
    Secret = 1 << 7
};

public class PuzzleSettings : MonoBehaviour {

    public int difficulty;

    public enum PuzzleSizes { Small, Medium, Big };

    public PuzzleSizes puzzleSize;

    [HideInInspector]
    public PuzzleTags2 tags;

    [SerializeField]
    public List<CharacterName> requiredCharacters;

    [HideInInspector]
    public List<PuzzleTags2> puzzleTags;

    // Use this for initialization
    void Start () {
        string[] stringSeparators = new string[] { ", " };
        foreach (string t in tags.ToString().Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries))
        {
            puzzleTags.Add((PuzzleTags2)Enum.Parse(typeof(PuzzleTags2), t));
        }
        

	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
