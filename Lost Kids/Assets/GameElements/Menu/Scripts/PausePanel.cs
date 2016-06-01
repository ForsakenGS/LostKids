using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour {

    GameObject mainPanel;
    GameObject settingsPanel;
    GameObject menuConfirmPanel;
    GameObject desktopConfirmPanel;

    UnityEngine.UI.Button resumeButton;
    UnityEngine.UI.Button settingsButton;
    UnityEngine.UI.Button menuButton;
    UnityEngine.UI.Button quitButton;



    //Instancia para el singleton
    public static PausePanel instance { get; private set; }
    // Use this for initialization


    void Awake()
    {
        if (instance != null && instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
        if (instance == null)
        {
            instance = this;
            mainPanel = transform.Find("MainPanel").gameObject;
            settingsPanel = transform.Find("Settings").gameObject;
            menuConfirmPanel = transform.Find("MenuConfirm").gameObject;
            desktopConfirmPanel = transform.Find("DesktopConfirm").gameObject;

            resumeButton = transform.Find("MainPanel/ResumeButton").GetComponent<UnityEngine.UI.Button>();
            settingsButton=transform.Find("MainPanel/SettingsButton").GetComponent<UnityEngine.UI.Button>();
            menuButton=transform.Find("MainPanel/MenuButton").GetComponent<UnityEngine.UI.Button>();
            quitButton=transform.Find("MainPanel/QuitButton").GetComponent<UnityEngine.UI.Button>();


        }
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


	
	}



    public void Show()
    {    
        mainPanel.SetActive(true);
        
    }

    public void Hide()
    {
        GameManager.ResumeGame();
        mainPanel.SetActive(false);
    }

    public void ShowMenuConfirmationPanel()
    {
        
        mainPanel.SetActive(false);
        menuConfirmPanel.SetActive(true);
        menuConfirmPanel.transform.Find("NoButton").GetComponent<UnityEngine.UI.Button>().Select();
    }
    public void HideMenuConfirmationPanel()
    {
        menuConfirmPanel.SetActive(false);
        Show();
        menuButton.Select();
    }

    public void ShowDesktopConfirmationPanel()
    {
        mainPanel.SetActive(false);
        desktopConfirmPanel.SetActive(true);
        desktopConfirmPanel.transform.Find("NoButton").GetComponent<UnityEngine.UI.Button>().Select();
    }
    public void HideDesktopConfirmationPanel()
    {
        desktopConfirmPanel.SetActive(false);
        Show();
        quitButton.Select();
    }

    public void ShowSettingsPanel()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    

    public void HideSettingsPanel()
    {
        settingsPanel.SetActive(false);
        Show();
        settingsButton.Select();
    }

    public void ChangeScene(string sc)
    {
        GameManager.GoToScene(sc);
    }

    public void QuitGame()
    {
        GameManager.QuitGame();
    }
    
    public static void ShowPanel()
    {
        instance.Show();
        instance.resumeButton.Select();
    }

    public static void HidePanel()
    {
        instance.Hide();
    }

    


}
