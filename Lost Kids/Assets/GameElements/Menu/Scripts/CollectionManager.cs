using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour {

    public Image UIImage;
    public Text UIName;
    public Text UIDescription;
    public Text UIHistory;
    public Sprite lockedImage;
    public string lockedDescription = "?????";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void ShowCollection(Collection collection)
    {
        string collectionName = collection.collection.ToString();
        UIName.text = collectionName;
        if(collection.collectedPieces.Contains(CollectionPieces.Image))
        {
            LoadImage(collectionName);
        }
        else
        {
            UIImage.sprite = lockedImage;
        }

        if (collection.collectedPieces.Contains(CollectionPieces.Description))
        {
            LoadDescription(collectionName);
        }
        else
        {
            UIDescription.text = lockedDescription;
        }

        LoadText(collection);

        Resources.UnloadUnusedAssets();

    }

    void LoadImage(string collectionName)
    {
        Sprite img = Resources.Load<Sprite>("Collections/" + collectionName+ "/"+ "Image");
        if (img != null)
        {
            UIImage.sprite = img;
        }
        else
        {
            UIImage.sprite = lockedImage;
        }


    }

    void LoadDescription(string collectionName)
    {
        Text txt = Resources.Load<Text>("Collections/" + collectionName+"/" + "Description");
        if (txt != null)
        {
            UIDescription.text = txt.text;
        }
        else
        {
            UIDescription.text = lockedDescription;
        }
    }

    void LoadText(Collection collection)
    {
        string collectionName = collection.collection.ToString();
        string resultText = "";
        Text txt = null;

        if (collection.collectedPieces.Contains(CollectionPieces.Text1))
        {
            txt = Resources.Load<Text>("Collections/" + collectionName + "/" + "Text1");
            if (txt != null)
            {
                resultText += txt.text + "\n";
            }
            else
            {
                resultText += ".....\n?????\n.....\n";
            }
        }
        else
        {
            resultText += ".....\n?????\n.....\n";
        }

        txt = null;

        if (collection.collectedPieces.Contains(CollectionPieces.Text2))
        {
            txt = Resources.Load<Text>("Collections/" + collectionName + "/" + "Text2");
            if (txt != null)
            {
                resultText += txt.text + "\n";
            }
            else
            {
                resultText += ".....\n?????\n.....\n";
            }
        }
        else
        {
            resultText += ".....\n?????\n.....\n";
        }


        txt = null;
        if (collection.collectedPieces.Contains(CollectionPieces.Text3))
        {
            txt = Resources.Load<Text>("Collections/" + collectionName + "/" + "Text3");
            if (txt != null)
            {
                resultText += txt.text + "\n";
            }
            else
            {
                resultText += ".....\n?????\n.....\n";
            }
        }
        else
        {
            resultText += ".....\n?????\n.....\n";
        }

        UIHistory.text = resultText;
    }


}
