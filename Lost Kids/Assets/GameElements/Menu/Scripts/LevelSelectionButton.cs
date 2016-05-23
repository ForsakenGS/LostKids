using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour {

    public GameLevels level;
    public Sprite lockedImage;
    public Sprite unlockedImage;
    public UnityEngine.UI.Button levelButton;  
    public bool unlocked=false;
    public Image selector;

    private Color selectorColor;
	// Use this for initialization
	void Start () {
        if(levelButton==null)
        {
            levelButton = GetComponentInChildren<UnityEngine.UI.Button>();
        }
        if(selector==null)
        {
            selector = GetComponentInChildren<Image>();
        }
        selectorColor = selector.color;

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
        selectorColor.a = 1;
        selector.color = selectorColor;
    }

    public void DeSelected()
    {
        selectorColor.a = 0;
        selector.color = selectorColor;
    }
}
