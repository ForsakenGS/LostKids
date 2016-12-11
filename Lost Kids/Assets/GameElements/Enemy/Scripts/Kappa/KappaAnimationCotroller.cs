using UnityEngine;
using System.Collections;

public class KappaAnimationCotroller : MonoBehaviour {
    private Animator animatorController;

	// Use this for initialization
	void Start () {
        animatorController = GetComponentInChildren<Animator>();
	}
	
    public bool HideAnimation() {
        animatorController.SetTrigger("Hide");

        return true;
    }

    public bool JibeAnimation() {
        animatorController.SetTrigger("Jibe");

        return true;
    }

    public bool ShowAnimation() {
        animatorController.SetTrigger("Show");

        return true;
    }

    public bool SwipeAnimation() {
        animatorController.SetTrigger("Swipe");

        return true;
    }

    public bool ThrowRockAnimation() {
        animatorController.SetTrigger("ThrowRock");

        return true;
    }
}