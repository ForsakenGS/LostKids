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

    private string languageSuffix;
	// Use this for initialization

    public static CollectionManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }
	void Start () {
        if ((LocalizationManager.language == null) || (LocalizationManager.language.Equals("es")))
        {
            languageSuffix = "_ES";
        }
        else {
            languageSuffix = "_EN";
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void ShowCollection(Collections collection)
    {
        string collectionName = collection.ToString();
        UIName.text = collectionName;
        //if(collection.collectedPieces.Contains(CollectionPieces.Image))
        if(PlayerPrefs.HasKey(collection.ToString()+CollectionPieces.Image.ToString()))
        {
            LoadImage(collectionName);
        }
        else
        {
            UIImage.sprite = lockedImage;
        }

        //if (collection.collectedPieces.Contains(CollectionPieces.Description))
        if(PlayerPrefs.HasKey(collection.ToString() + CollectionPieces.Description.ToString()))
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
        TextAsset txt = Resources.Load<TextAsset>("Collections/" + collectionName+"/" + "Description"+languageSuffix);
        if (txt != null)
        {
            UIDescription.text = txt.text;
        }
        else
        {
            UIDescription.text = lockedDescription;
        }
    }

    void LoadText(Collections collection)
    {
        string collectionName = collection.ToString();
        string resultText = "";
        TextAsset txt = null;

        //if (collection.collectedPieces.Contains(CollectionPieces.Text1))
        if (PlayerPrefs.HasKey(collection.ToString() + CollectionPieces.Text1.ToString()))
        {
            txt = Resources.Load<TextAsset>("Collections/" + collectionName + "/" + "Text1"+languageSuffix);
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

        //if (collection.collectedPieces.Contains(CollectionPieces.Text2))
        if (PlayerPrefs.HasKey(collection.ToString() + CollectionPieces.Text2.ToString()))
        {
            txt = Resources.Load<TextAsset>("Collections/" + collectionName + "/" + "Text2"+languageSuffix);
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
        //if (collection.collectedPieces.Contains(CollectionPieces.Text3))
        if (PlayerPrefs.HasKey(collection.ToString() + CollectionPieces.Text3.ToString()))
        {
            txt = Resources.Load<TextAsset>("Collections/" + collectionName + "/" + "Text3"+languageSuffix);
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
