using UnityEngine;
using InControl;
using UnityEngine.UI;
using System.Collections.Generic;

public class TransitionSceneManager : MonoBehaviour {

    public GameObject MessageManager;
    private MessageManager messageManager;
    private SceneFade fader;

    public int messagesCount;
    public string previousScene;
    public string nextScene;


    public Text timeText;
    public Text totalCollectiblesText;
    public Text actualCollectiblesText;
    private LevelData levelData;


    private bool inputAvailable = false;
    private bool inTransition = false;

    // Use this for initialization
    void Start() {
        messageManager = MessageManager.GetComponent<MessageManager>();
        /*
        levelData = GameData.getLevelData(previousScene);

        timeText.text = levelData.finishTime+"";
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
        */
        fader = GetComponent<SceneFade>();
        fader.nextScene = nextScene;
        fader.StartScene();

        Invoke("StartMessages", fader.fadeSpeed * 2);
    }

    // Update is called once per frame
    void Update() {
        if (inputAvailable && SkipMessageInput()) {
            if (!messageManager.MessageEnded()) {
                messageManager.SkipText();
                inputAvailable = false;
                Invoke("AllowInput", 1);
            } else if (!inTransition){
                inTransition = true;
                fader.EndScene();
            }
        }
    }

    // Devuelve si el jugador desea pasar el mensaje
    bool SkipMessageInput() {
        InputControl skipInput = InputManager.ActiveDevice.GetControl(InputControlType.Action1);
        return ((Input.GetButton("Submit")) || (skipInput.IsPressed) || (skipInput.WasPressed) || Input.anyKeyDown);
    }


    void StartMessages() {
        List<int> indexList = new List<int>(messagesCount);
        for (int i = 0; i < messagesCount; i++) {
            indexList.Add(i);
        }
        messageManager.ShowConversation(indexList);
        inputAvailable = true;
    }

    void AllowInput() {
        inputAvailable = true;
    }
}
