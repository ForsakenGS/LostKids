using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour {

    private ArrayList messages;

    public TextAsset messageFile;

    public Text msg;
    public RawImage img;

    public float normalLetterSpeed;
    public float fastLetterSpeed;
    private float letterSpeed;

    public delegate void LockUnlockAction();
    public static event LockUnlockAction LockUnlockEvent;

    private enum State { FastMessage, EndMessage, NextMessage };
    private State messageState;
    private int nextIndex;

    // Use this for initialization
    void Start () {

        nextIndex = 0;

        messageState = State.FastMessage;

        messages = new ArrayList();

        letterSpeed = normalLetterSpeed;

        FillMessages();
	}
	
    private void FillMessages() {

        string fullText = messageFile.text;

        string[] formatText = fullText.Split(';');

        for(int i = 0; i < formatText.Length; i++) {

            messages.Add((string)formatText[i]);

        }

    }


    public void ShowMessage(int index) {
        Debug.Log(messages[index]);

        img.gameObject.SetActive(true);
        msg.gameObject.SetActive(true);

        LockUnlockEvent();

        StartCoroutine(TypeText(index));        
        
    }

    IEnumerator TypeText(int index) {

        msg.text = string.Empty;

        string aux = (string)messages[index];

        string[] lines = aux.Split('@');

        for (int i = 0; i < lines.Length; i++)
        {
            char[] line = lines[i].ToCharArray();
            if(line.Length != 1) {
                for (int j = 0; j < line.Length; j++) {
                    msg.text += line[j];
                    yield return new WaitForSeconds(letterSpeed);
                }
                
                msg.text += "\n";
                
            } else {
                int.TryParse(line[0].ToString(), out nextIndex);
                messageState = State.NextMessage;
            }
        }

        if(!messageState.Equals(State.NextMessage)) {
            messageState = State.EndMessage;
        }

        yield return 0;
    }

    public void SkipText() {

        switch(messageState) {
            case State.FastMessage:
                letterSpeed = fastLetterSpeed;
                break;
            case State.EndMessage:
                messageState = State.FastMessage;
                letterSpeed = normalLetterSpeed;
                LockUnlockEvent();
                img.gameObject.SetActive(false);
                msg.gameObject.SetActive(false);
                break;
            case State.NextMessage:
                messageState = State.FastMessage;
                letterSpeed = normalLetterSpeed;
                StartCoroutine(TypeText(nextIndex));
                break;
        }  
    
    }


}
