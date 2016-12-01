using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour {

    //Velocidad de fadeIn/Out
    public float fadeSpeed = 1.5f;         

    //Texturas para el fadeIn/out
    public Image fadeInImage;
    public Image fadeOutImage;

    private bool sceneStarting = true;      

    public string nextScene;

    private GameManager gameManager;



    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }


    void Update()
    {

    }


    IEnumerator FadeIn()
    {
        // Lerp the colour of the texture between itself and transparent.
        Color target = fadeOutImage.color;
        target.a = 0;
        // If the texture is almost clear...
        while (fadeInImage.color.a > 0.05f)
        {
            fadeInImage.color = Color.Lerp(fadeInImage.color, target, fadeSpeed * Time.deltaTime);
   
            yield return null;
        }
        fadeInImage.color = Color.clear;
        fadeInImage.enabled = false;

        // The scene is no longer starting.
        sceneStarting = false;
        yield return 0;
    }


    public void StartScene()
    {
        // Fade the texture to clear.
        fadeInImage.enabled = true;
        fadeInImage.canvasRenderer.SetAlpha(1);
        fadeInImage.CrossFadeAlpha(0, fadeSpeed, false);
    }


    public void EndScene()
    {
        // Make sure the texture is enabled.
        fadeOutImage.enabled = true;
        fadeOutImage.canvasRenderer.SetAlpha(0.01f);
        // Start fading towards black.
        fadeOutImage.CrossFadeAlpha(1, fadeSpeed, false);
        Invoke("ChangeScene", fadeSpeed);
    }

    public void ChangeScene()
    {
        gameManager.ChangeScene(nextScene);
    }
}
