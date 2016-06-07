using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActivableFade : MonoBehaviour {

    private Image fadeImage;

    public float fadeSpeed;

    public float timeCutScene;

    private CameraManager cameraManager;

    public delegate void FadeInOutAction();
    public static event FadeInOutAction FadeInOutEvent;

    private GameObject camera;

    public void Start() {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        fadeImage = GameObject.FindGameObjectWithTag("FadeImage").GetComponent<Image>();
    }

    public void OnEnable() {
        CameraManager.CutSceneEvent += EndCutScene;
    }

    public void OnDisable() {
        CameraManager.CutSceneEvent -= EndCutScene;
    }

    public void StartCutScene(GameObject cam)
    {
        camera = cam;
        FadeIn();
        Invoke("ActivateCutScene", fadeSpeed);
    }

    public void EndCutScene()
    {
        FadeIn();
        Invoke("DeactivateCutScene", fadeSpeed);
    }

    private void ActivateCutScene() {
        cameraManager.ChangeCameraFade(camera, timeCutScene);
        FadeOut();
        Invoke("DoAction", fadeSpeed);
    }

    private void DoAction() {
        FadeInOutEvent();
    }

    private void DeactivateCutScene() {
        cameraManager.RestoreCamera(camera);
        FadeOut();
    }

    private void FadeIn() {
        fadeImage.enabled = true;
        fadeImage.canvasRenderer.SetAlpha(0);
        fadeImage.CrossFadeAlpha(1, fadeSpeed, false);
    }

    private void FadeOut() {
        fadeImage.canvasRenderer.SetAlpha(1);
        fadeImage.CrossFadeAlpha(0, fadeSpeed, false);
    }


}
