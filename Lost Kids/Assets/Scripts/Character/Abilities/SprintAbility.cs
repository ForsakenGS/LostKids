using UnityEngine;
using System.Collections;

public class SprintAbility : CharacterAbility {
	public float duration = 10;

	private float executionTime;

	public override bool EndExecution () {
		return false;
	}

	public override bool StartExecution () {
		return false;
	}

	// Use this for initialization
	void Start () {
		executionTime = 0;
	}

	// Update is called once per frame
	void Update () {
		executionTime += Time.deltaTime;
	}
}