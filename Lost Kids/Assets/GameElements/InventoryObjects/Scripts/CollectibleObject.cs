using UnityEngine;

public class CollectibleObject : MonoBehaviour {

    public Collections collectionName;

    public CollectionPieces collectionPiece;

    public float replaceChance = 20;

    private bool used = false;
    // Use this for initialization
    void Start () {

        if(GameData.AlreadyCollected(collectionName,collectionPiece))
        {
            if(Random.Range(0,100.01f)<replaceChance)
            {
                System.Array pieces = System.Enum.GetValues(typeof(Collections));
                
                collectionPiece = (CollectionPieces)pieces.GetValue(Random.Range(0,pieces.Length));
            }

            if (GameData.AlreadyCollected(collectionName, collectionPiece))
            {
                Color color = GetComponent<Renderer>().material.color;
                color.a = 0.3f;
                GetComponent<Renderer>().material.color = color;
                used = true;
            }
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter(Collider col)
    {
        if (!used)
        {
            if (CharacterManager.IsActiveCharacter(col.gameObject))
            {
                used = true;
                GameData.AddCollectible(collectionName,collectionPiece);
            }
            GetComponent<RotatingItem>().speed *= 4f;
            GetComponent<FadeOutDestroy>().FadeAndDestroy();
            iTween.MoveTo(gameObject, transform.position + Vector3.up * 3, GetComponent<FadeOutDestroy>().fadeTime);
        }
    }
}
