using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour {

    public GameLevels level;
    public Sprite lockedImage;
    public Sprite unlockedImage;
    public UnityEngine.UI.Button levelButton;  
    public bool unlocked=false;

    public string levelName_EN;
    public string levelName_ES;
    public Text levelLabel;

	// Use this for initialization
	void Start () {
        if(levelButton==null)
        {
            levelButton = GetComponentInChildren<UnityEngine.UI.Button>();
        }
       

        if (GameData.LevelUnlocked(level.ToString()))
        {
            unlocked = true;
            levelButton.image.sprite = unlockedImage;
        }
        else
        {
            levelButton.image.sprite = lockedImage;
        }
        
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clicked()
    {
        if(unlocked)
        {
            GameManager.GoToScene(level.ToString());
        }
    }

    public void Selected()
    {
        if (LocalizationManager.language.Equals("es"))
        {
            levelLabel.text = levelName_EN;
        }
        else
        {
            levelLabel.text = levelName_ES;
        }
    }

}
