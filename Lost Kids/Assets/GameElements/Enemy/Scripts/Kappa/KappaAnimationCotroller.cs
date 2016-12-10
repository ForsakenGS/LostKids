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

    public bool SwipeAnimation() {
        animatorController.SetTrigger("Swipe");

        return true;
    }

    public bool ThrowRockAnimation() {
        animatorController.SetTrigger("ThrowRock");

        return true;
    }

    //void Update() {
    //    if (Input.GetKeyDown(KeyCode.Z)) {
    //        Debug.Log("hide");
    //        HideAnimation();
    //    }
    //    if (Input.GetKeyDown(KeyCode.X)) {
    //        Debug.Log("jibe");
    //        JibeAnimation();
    //    }
    //    if (Input.GetKeyDown(KeyCode.C)) {
    //        Debug.Log("swipe");
    //        SwipeAnimation();
    //    }
    //    if (Input.GetKeyDown(KeyCode.V)) {
    //        Debug.Log("rock");
    //        ThrowRockAnimation();
    //    }
    //}
}