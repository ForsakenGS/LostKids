using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
    
    // Update is called once per frame
	void Update () {
	    if(Input.anyKeyDown) {
            NextScene();
        }
	}

    public void NextScene() {
        SceneManager.LoadScene("LanguageSelection");
    }
}
