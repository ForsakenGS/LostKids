using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PuzzleSettings : MonoBehaviour {

    public int difficulty;

    public enum PuzzleSizes { Small, Medium, Big };

    public PuzzleSizes puzzleSize;

    [System.Flags]
    public enum PuzzleTags { BigJump, Sprint, Push, Break, Telekinesis, Astral, Platforms,
        Sequence, Levers, Buttons, CanDie, DarkFear, WaterFear, TreeFear, Collectibles, Secret };

    [SerializeField]  [Flags] PuzzleTags tags;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
