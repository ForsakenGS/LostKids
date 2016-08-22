using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour {

    private static Image fadeImage;

    public float FadeTime = 0.5f;
    private static float fadeTime;

    private static RectTransform blackBarTop;
    private static RectTransform blackBarBot;

    private static Vector2 topBarEndPos;
    private static Vector2 botBarEndPos;

    private static Vector2 topBarStartPos;
    private static Vector2 botBarStartPos;

    public static float barMovementTime;
    public float BarMovementTime = 1;

    private static CameraManager cameraManager;
    public static CutSceneManager instance = null;

    public static GameObject cutSceneCamera;
    public static float cutSceneTime;

    public delegate void FadeInOutAction();
    public event FadeInOutAction FadeInOutEvent;

    public delegate void CutSceneState();
    public static CutSceneState CutSceneActivation;
    public static CutSceneState CutSceneDeactivation;

    void Awake() {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        instance = this;
        fadeImage = transform.Find("FadeImage").GetComponent<Image>();
        fadeTime = FadeTime;
        barMovementTime = BarMovementTime;
        blackBarTop = transform.Find("BlackBarTop").GetComponent<RectTransform>();
        topBarStartPos = blackBarTop.anchoredPosition;
        topBarEndPos = topBarStartPos + Vector2.down * (blackBarTop.rect.height - 40);
        blackBarBot = transform.Find("BlackBarBot").GetComponent<RectTransform>();
        botBarStartPos = blackBarBot.anchoredPosition;
        botBarEndPos = botBarStartPos + Vector2.up * (blackBarBot.rect.height - 40);

    }

    void OnEnable() {
        //ShowBars();
    }

    void OnDisable() {
        blackBarTop.anchoredPosition = topBarStartPos;
        blackBarBot.anchoredPosition = botBarStartPos;

    }

    public static void StartCutScene(GameObject cam, FadeInOutAction action) {
        instance.FadeInOutEvent += action;
        cutSceneTime = 0;
        cutSceneCamera = cam;
        InputManagerTLK.SetLock(true);
        FadeIn();
        instance.Invoke("ActivateCutScene", fadeTime);

    }

    public static void StartCutScene(GameObject cam, FadeInOutAction action, float time) {
        instance.FadeInOutEvent += action;
        cutSceneTime = time;
        cutSceneCamera = cam;
        InputManagerTLK.SetLock(true);
        FadeIn();
        instance.Invoke("ActivateCutScene", fadeTime);

    }

    public void EndCutScene() {
        FadeIn();
        instance.Invoke("DeactivateCutScene", fadeTime);

    }

    private void ActivateCutScene() {
        // Evento activación CutScene
        CutSceneActivation();
        FadeOut();
        ShowBars();
        cameraManager.ChangeCameraFade(cutSceneCamera);
        if (cutSceneTime > 0) {
            Invoke("EndCutScene", cutSceneTime);
        } else {
            MessageManager.ConversationEndEvent += EndCutScene;
        }

        instance.Invoke("DoAction", fadeTime);
    }

    private void DoAction() {
        if (FadeInOutEvent != null) {
            FadeInOutEvent();
        }
        FadeInOutEvent = null;
    }



    private void DeactivateCutScene() {
        // Evento activación CutScene
        CutSceneDeactivation();
        MessageManager.ConversationEndEvent -= EndCutScene;
        CameraManager.CutSceneEvent -= EndCutScene;
        HideBars(fadeTime);
        cameraManager.RestoreCamera(cutSceneCamera);
        FadeOut();
        Invoke("RestoreControl", fadeTime);
    }

    public void RestoreControl() {
        InputManagerTLK.SetLock(false);
    }

    public static void ShowBars() {

        iTween.MoveTo(blackBarTop.gameObject, topBarEndPos, barMovementTime);
        iTween.MoveTo(blackBarBot.gameObject, botBarEndPos, barMovementTime);
    }

    public static void ShowBars(float time) {

        iTween.MoveTo(blackBarTop.gameObject, topBarEndPos, time);
        iTween.MoveTo(blackBarBot.gameObject, botBarEndPos, time);
    }

    public static void HideBars() {

        iTween.MoveTo(blackBarTop.gameObject, topBarStartPos, barMovementTime);
        iTween.MoveTo(blackBarBot.gameObject, botBarStartPos, barMovementTime);
    }

    public static void HideBars(float time) {
        iTween.MoveTo(blackBarTop.gameObject, topBarStartPos, time);
        iTween.MoveTo(blackBarBot.gameObject, botBarStartPos, time);
    }

    public static void FadeIn() {

        fadeImage.enabled = true;
        fadeImage.canvasRenderer.SetAlpha(0);
        fadeImage.CrossFadeAlpha(1, fadeTime, false);
    }

    public static void FadeOut() {

        fadeImage.canvasRenderer.SetAlpha(1);
        fadeImage.CrossFadeAlpha(0, fadeTime, false);
        //fadeImage.enabled = false;

    }
}
