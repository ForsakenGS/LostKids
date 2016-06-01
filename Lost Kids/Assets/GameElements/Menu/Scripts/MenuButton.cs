using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButton : MonoBehaviour {

    public float unSelectedAlpha = 0.4f;
    private AudioLoader audioLoader;

    private GameManager gameManager;

    Image buttonImage;
    Color buttonColor;

    void OnEnable()
    {
        buttonColor.a = unSelectedAlpha;
        buttonImage.color = buttonColor;
    }
    void Awake()
    {
        audioLoader = GetComponent<AudioLoader>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        buttonImage = GetComponent<Image>();
        buttonColor = buttonImage.color;
        buttonColor.a = unSelectedAlpha;
        buttonImage.color = buttonColor;
    }
	// Use this for initialization
	void Start () {


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clicked()
    {
        AudioManager.Play(audioLoader.GetSound("Clicked"), false, 1);

    }

    public void Clicked(string sc) {
        AudioManager.Play(audioLoader.GetSound("Clicked"), false, 1);
        gameManager.PrepareScene(sc);
    }

    public void Selected()
    {

        AudioManager.Play(audioLoader.GetSound("Selected"), false, 1);
        buttonColor.a = 1;
        buttonImage.color = buttonColor;

    }

    public void DeSelected()
    {
        buttonColor.a = unSelectedAlpha;
        buttonImage.color = buttonColor;
    }
}
