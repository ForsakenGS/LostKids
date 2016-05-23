using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour {

    GameObject mainPanel;
    GameObject settingsPanel;
    GameObject menuConfirmPanel;
    GameObject desktopConfirmPanel;
 
    

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
            settingsPanel = transform.Find("MainPanel/Settings").gameObject;
            menuConfirmPanel = transform.Find("MainPanel/MenuConfirm").gameObject;
            desktopConfirmPanel = transform.Find("MainPanel/DesktopConfirm").gameObject;
            

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
        menuConfirmPanel.SetActive(true);
    }
    public void HideMenuConfirmationPanel()
    {
        menuConfirmPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ShowDesktopConfirmationPanel()
    {
        desktopConfirmPanel.SetActive(true);
    }
    public void HideDesktopConfirmationPanel()
    {
        desktopConfirmPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
        
    }
    

    public void HideSettingsPanel()
    {
        settingsPanel.SetActive(false);
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
    }

    public static void HidePanel()
    {
        instance.Hide();
    }

    


}
