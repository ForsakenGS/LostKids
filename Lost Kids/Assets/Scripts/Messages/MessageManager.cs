using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour {

    private ArrayList messages;

    public TextAsset messageFile;

	// Use this for initialization
	void Start () {
        messages = new ArrayList();

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

        string msg = (string)messages[index];

        string[] lines = msg.Split('@');

        Debug.Log(lines.Length);

        //BroadcastMessage("Lock");
    }


}
