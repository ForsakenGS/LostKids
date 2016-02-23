using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour {

    private ArrayList messages;

    public TextAsset messageFile;

    public Text msg;

    public float normalLetterSpeed;

    public float fastLetterSpeed;

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

        msg.text = string.Empty;

        StartCoroutine(TypeText(index));

        //BroadcastMessage("Lock");
    }

    IEnumerator TypeText(int index) {
        string aux = (string)messages[index];

        string[] lines = aux.Split('@');

        for (int i = 0; i < lines.Length; i++)
        {
            char[] line = lines[i].ToCharArray();
            for (int j = 0; j < line.Length; j++) {
                msg.text += line[j];
                yield return new WaitForSeconds(normalLetterSpeed);
            }
                
            if (i < lines.Length - 1)
            {
                msg.text += "\n";
            }
        }

        yield return 0;
    }


}
