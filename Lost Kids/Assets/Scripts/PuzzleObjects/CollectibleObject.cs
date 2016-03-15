using UnityEngine;

public class CollectibleObject : MonoBehaviour {

    public string id;

    private bool used = false;
    // Use this for initialization
    void Start () {

        if(GameData.AlreadyCollected(id))
        {
            Color color = GetComponent<Renderer>().material.color;
            color.a = 0.3f;
            GetComponent<Renderer>().material.color = color;
            used = true;
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
                GameData.UpdateCollectibles(this.id);
            }
            GetComponent<FadeOutDestroy>().FadeAndDestroy();
        }
    }
}
