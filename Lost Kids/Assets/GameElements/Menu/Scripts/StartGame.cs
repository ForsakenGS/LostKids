using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
    
    void Start()
    {
        Invoke("NextScene", 5);
    }
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
