using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PausePanel : MonoBehaviour {

    GameObject mainPanel;
    GameObject settingsPanel;
    GameObject controlsPanel;
    GameObject menuConfirmPanel;
    GameObject desktopConfirmPanel;
    GameObject background;

    UnityEngine.UI.Button resumeButton;
    UnityEngine.UI.Button settingsButton;
    UnityEngine.UI.Button controlsButton;
    UnityEngine.UI.Button menuButton;
    UnityEngine.UI.Button quitButton;

    UnityEngine.UI.Button settingsBackButton;
    UnityEngine.UI.Button controlsBackButton;
    /*
    UnityEngine.UI.Button controlsKeyboardButton;
    UnityEngine.UI.Button controlsPadButton;
    */





    //Instancia para el singleton
    public static PausePanel instance { get; private set; }
    // Use this for initialization


    void Awake() {
        if (instance != null && instance != this) {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
        if (instance == null) {
            instance = this;
            mainPanel = transform.Find("MainPanel").gameObject;
            settingsPanel = transform.Find("Settings").gameObject;
            controlsPanel = transform.Find("ControlsPanel").gameObject;
            menuConfirmPanel = transform.Find("MenuConfirm").gameObject;
            desktopConfirmPanel = transform.Find("DesktopConfirm").gameObject;
            background = transform.Find("Background").gameObject;

            resumeButton = transform.Find("MainPanel/ResumeButton").GetComponent<UnityEngine.UI.Button>();
            settingsButton = transform.Find("MainPanel/SettingsButton").GetComponent<UnityEngine.UI.Button>();
            controlsButton = transform.Find("MainPanel/ControlsButton").GetComponent<UnityEngine.UI.Button>();
            menuButton = transform.Find("MainPanel/MenuButton").GetComponent<UnityEngine.UI.Button>();
            quitButton = transform.Find("MainPanel/QuitButton").GetComponent<UnityEngine.UI.Button>();

            settingsBackButton = transform.Find("Settings/BackButton").GetComponent<UnityEngine.UI.Button>();
            controlsBackButton = transform.Find("ControlsPanel/BackButton").GetComponent<UnityEngine.UI.Button>();
            /*
            controlsKeyboardButton = transform.Find("ControlsPanel/KeyboardButton").GetComponent<UnityEngine.UI.Button>();
            controlsPadButton = transform.Find("ControlsPanel/PadButton").GetComponent<UnityEngine.UI.Button>();
            */

        }
    }
    void Start() {

    }

    // Update is called once per frame
    void Update() {


    }



    public void Show() {
        mainPanel.SetActive(true);
        background.SetActive(true);
    }

    public void Hide() {
        GameManager.ResumeGame();
        InputManagerTLK.SetMenuMode(false);
        mainPanel.SetActive(false);
        controlsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        desktopConfirmPanel.SetActive(false);
        menuConfirmPanel.SetActive(false);
        background.SetActive(false);
        // Se cambia botón activo para que al mostrarse de nuevo el menú de pausa aparezca remarcado resumeButton correctamente
        GetComponentInChildren<EventSystem>().SetSelectedGameObject(quitButton.gameObject);
    }

    public void ShowMenuConfirmationPanel() {

        mainPanel.SetActive(false);
        menuConfirmPanel.SetActive(true);
        menuConfirmPanel.transform.Find("NoButton").GetComponent<UnityEngine.UI.Button>().Select();
    }
    public void HideMenuConfirmationPanel() {
        menuConfirmPanel.SetActive(false);
        Show();
        menuButton.Select();
    }

    public void ShowDesktopConfirmationPanel() {
        mainPanel.SetActive(false);
        desktopConfirmPanel.SetActive(true);
        desktopConfirmPanel.transform.Find("NoButton").GetComponent<UnityEngine.UI.Button>().Select();
    }
    public void HideDesktopConfirmationPanel() {
        desktopConfirmPanel.SetActive(false);
        Show();
        quitButton.Select();
    }

    public void ShowSettingsPanel() {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        settingsBackButton.Select();
    }


    public void HideSettingsPanel() {
        settingsPanel.SetActive(false);
        Show();
        settingsButton.Select();
    }

    public void ShowControlsPanel() {
        mainPanel.SetActive(false);
        controlsPanel.SetActive(true);
        controlsBackButton.Select();

        /*
        if(Input.GetJoystickNames().Length>0 && !Input.GetJoystickNames()[0].Equals(""))
        {
            controlsPadButton.Select();
            controlsPadButton.onClick.Invoke();
        }
        else
        {
            controlsKeyboardButton.Select();
            controlsKeyboardButton.onClick.Invoke();
        }
        */
    }

    public void HideControlsPanel() {
        controlsPanel.SetActive(false);
        Show();
        controlsButton.Select();
    }

    public void ChangeScene(string sc) {
        GameManager.GoToScene(sc);
    }

    public void QuitGame() {
        GameManager.QuitGame();
    }

    public static void ShowPanel() {
        instance.Show();
        instance.resumeButton.Select();
    }

    public static void HidePanel() {

        instance.Hide();
    }




}
