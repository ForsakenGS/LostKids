using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TransitionSceneManager : MonoBehaviour {

    public GameObject MessageManager;
    private MessageManager messageManager;
    private SceneFade fader;

    public string previousScene;
    public string nextScene;


    public Text timeText;
    public Text totalCollectiblesText;
    public Text actualCollectiblesText;
    private LevelData levelData;

    private bool inputAvailable = false;
	// Use this for initialization
	void Start () {
        messageManager = MessageManager.GetComponent<MessageManager>();

        levelData = GameData.getLevelData(previousScene);

        //timeText.text = levelData.finishTime+"";
        totalCollectiblesText.text = levelData.collectibles.Count+"";

        int actualCollectibles = 0;
        foreach (string collId in levelData.collectibles)
        {
            if(GameData.AlreadyCollected(collId))
            {
                actualCollectibles++;
            }
        }

        actualCollectiblesText.text = actualCollectibles + "";

        fader = GetComponent<SceneFade>();
        fader.nextScene = nextScene;
        fader.StartScene();
        
        Invoke("StartMessages", fader.fadeSpeed*2);
	}
	
	// Update is called once per frame
	void Update () {
        if(inputAvailable && Input.GetButtonDown("Jump"))
        {
            if (!messageManager.MessageEnded())
            {
                messageManager.SkipText();
                inputAvailable = false;
                Invoke("AllowInput", 1);
            }
            else
            {

                fader.EndScene();
            }
        }
	
	}


    void StartMessages()
    {
        messageManager.ShowMessage(0);
        inputAvailable = true;
    }

    void AllowInput()
    {
        inputAvailable = true;
    }
}
