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


    void Awake()
    {
                /*
        fadeInImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        fadeOutImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        */
    }

    void Start() {
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


    IEnumerator FadeOut()
    {

        Color target = fadeOutImage.color;
        target.a = 1;
        while (fadeOutImage.color.a < 0.95f)
        {
            
            fadeInImage.color = Color.Lerp(fadeInImage.color, target, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        
        gameManager.PrepareScene(nextScene);
        
        //SceneManager.LoadScene(nextScene);
        yield return 0;
    }


    public void StartScene()
    {
        // Fade the texture to clear.
        //fadeInImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        fadeInImage.enabled = true;
        fadeInImage.canvasRenderer.SetAlpha(1);
        fadeInImage.CrossFadeAlpha(0, fadeSpeed, false);
    }


    public void EndScene()
    {
        // Make sure the texture is enabled.
        //fadeOutImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        fadeOutImage.enabled = true;
        fadeOutImage.canvasRenderer.SetAlpha(0.01f);
        // Start fading towards black.
        fadeOutImage.CrossFadeAlpha(1, fadeSpeed, false);
        Invoke("ChangeScene", fadeSpeed);
    }

    public void ChangeScene()
    {
        gameManager.PrepareScene(nextScene);
        //SceneManager.LoadScene(nextScene);
    }
}
