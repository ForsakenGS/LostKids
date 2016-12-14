using UnityEngine;
using UnityEngine.UI;

public class CollectibleObject : MonoBehaviour {

    public Collections collectionName;

    public CollectionPieces collectionPiece;

    public float replaceChance = 100;

    private Canvas tooltip;

    private bool used = false;
    // Use this for initialization
    void Start () {

        if(GameData.AlreadyCollected(collectionName,collectionPiece))
        {
            if(Random.Range(0,100.01f)<=replaceChance)
            {
                System.Array pieces = System.Enum.GetValues(typeof(Collections));
                
                collectionPiece = (CollectionPieces)pieces.GetValue(Random.Range(0,pieces.Length));
            }
            /*
            if (GameData.AlreadyCollected(collectionName, collectionPiece))
            {
                //Color color = GetComponentInChildren<Renderer>().material.color;
               // color.a = 0.3f;
                //GetComponent<Renderer>().material.color = color;
                //used = true;
            }
            */
        }
        tooltip = GetComponentInChildren<Canvas>();
        tooltip.transform.parent = null;
        tooltip.GetComponent<Image>().sprite= Resources.Load<Sprite>("Collections/" + collectionName.ToString() + "/" + "Image");

    }
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter(Collider col)
    {
        if (!used)
        {
            if (col.CompareTag("Player"))
            {
                used = true;
                GameData.AddCollectible(collectionName, collectionPiece);

                GetComponent<RotatingItem>().speed *= 4f;
                ParticleSystem.EmissionModule em = GetComponent<ParticleSystem>().emission;
                ParticleSystem.MinMaxCurve cur = em.rate;
                cur.constantMax = 25;
                em.rate = cur;

                AudioManager.Play(GetComponent<AudioSource>(), false, 0.6f);

                GetComponent<FadeOutDestroy>().FadeAndDestroy();
                iTween.MoveTo(gameObject, transform.position + Vector3.up * 3, GetComponent<FadeOutDestroy>().fadeTime);

                tooltip.GetComponent<Image>().enabled = true;
                iTween.FadeTo(tooltip.gameObject, 0, 1.5f);
               
                Destroy(tooltip.gameObject, 1.5f);

            }
        }
    }
}
