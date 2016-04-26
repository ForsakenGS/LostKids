using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour {
	private CameraManager cameraManager;
    Vector3 lookPosition;
    Image tooltipImage;
	// Use this for initialization
	void Start () {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        tooltipImage = GetComponent<Canvas>().GetComponent<Image>();


    }
	
	// Update is called once per frame
	void LateUpdate () {

        transform.LookAt(cameraManager.CurrentCamera().transform);

    }

    public void SetImage(Sprite image)
    {
        tooltipImage.sprite = image;
    }

    public void ActiveCanvas(bool active)
    {
        GetComponent<Image>().enabled = active;
    }
}
