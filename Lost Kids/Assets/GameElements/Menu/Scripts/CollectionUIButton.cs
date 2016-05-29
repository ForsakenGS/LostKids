using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollectionUIButton : MonoBehaviour {


    public Collections collectionName;
    private Text buttonText;
    public GameObject CollectionManager;
    private CollectionManager collectionManager;
    public string lockedText = "?????";
    public bool unlocked = false;
    private Collection collection;
	// Use this for initialization
	void Start () {
        collectionManager = CollectionManager.GetComponent<CollectionManager>();
        collection = GameData.GetCollection(collectionName);
        buttonText = GetComponentInChildren<Text>();
        if (collection ==null)
        {
            buttonText.text = lockedText;
            //unlocked = false;
                
        }
        else
        {
            buttonText.text = collectionName.ToString();
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
            collectionManager.ShowCollection(collection);
        }
    }
}
