using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollectionUIButton : MonoBehaviour {


    public Collections collectionName;
    private Text buttonText;

    public string lockedText = "?????";
    public bool unlocked = false;
    private Collection collection;
	// Use this for initialization
	void Start () {

        //collection = GameData.GetCollection(collectionName);
        buttonText = GetComponentInChildren<Text>();
        if (!PlayerPrefs.HasKey(collectionName.ToString()))
        {
            buttonText.text = lockedText;
            //unlocked = false;
                
        }
        else
        {
            buttonText.text = collectionName.ToString().Replace("_", "-");
            unlocked = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clicked()
    {
        if(unlocked)
        {
            
            CollectionManager.Instance.ShowCollection(collectionName);
        }
    }
}
