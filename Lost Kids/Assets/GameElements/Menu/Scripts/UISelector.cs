using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISelector : MonoBehaviour {

    Image selectorImage;
    Color selectorColor;
    public GameObject selector;
    public float unSelectedAlpha = 0.4f;
    private AudioLoader audioLoader;
    // Use this for initialization


    void OnEnable()
    {
        selectorColor.a = unSelectedAlpha;
        selectorImage.color = selectorColor;
    }
    void Awake()
    {
        selectorImage = selector.GetComponent<Image>();
        selectorColor = selectorImage.color;
        selectorColor.a = unSelectedAlpha;
        selectorImage.color = selectorColor;
        audioLoader = GetComponent<AudioLoader>();
    }
    void Start () {



    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Selected()
    {
        AudioManager.Play(audioLoader.GetSound("Selected"), false, 1);
        selectorColor.a = 1;
        selectorImage.color = selectorColor;
        if (GetComponentInParent<LevelSelectionButton>() != null)
        {
            GetComponentInParent<LevelSelectionButton>().Selected();
        }

    }

    public void DeSelected()
    {
        selectorColor.a = unSelectedAlpha;
        selectorImage.color = selectorColor;
    }
}
