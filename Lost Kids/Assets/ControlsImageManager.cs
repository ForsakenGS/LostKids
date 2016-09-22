using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlsImageManager : MonoBehaviour {

    public Image padImage;
    public Sprite aoiPad;
    public Sprite akaiPad;
    public Sprite kiPad;
    public Text skill1, skill2;
    public Text skill1Aoi, skill2Aoi;
    public Text skill1Akai, skill2Akai;
    public Text skill1Ki, skill2Ki;


    void OnEnable()
    {
        GameObject activeCharacter = CharacterManager.GetActiveCharacter();
        if(skill1Aoi!=null)
            skill1Aoi.enabled = false;
        if (skill2Aoi != null)
            skill2Aoi.enabled = false;
        if (skill1Akai != null)
            skill1Akai.enabled = false;
        if (skill2Akai != null)
            skill2Akai.enabled = false;
        if (skill1Ki != null)
            skill1Ki.enabled = false;
        if (skill2Ki != null)
            skill2Ki.enabled = false;
        if (skill1 != null)
            skill1.enabled = false;
        if (skill2 != null)
            skill2.enabled = false;

        if (activeCharacter==null)
        {
            SetStandardControls();
        }
        else if (activeCharacter.name.Contains("Aoi"))
        {
            SetAoiControls();
        }
        else if(activeCharacter.name.Contains("Akai"))
        {
            SetAkaiControls();
        }
        else if (activeCharacter.name.Contains("Ki"))
        {
            SetKiControls();
        }

    }

    public void SetAoiControls()
    {
        padImage.sprite = aoiPad;
        skill1Aoi.enabled = true;
        skill2Aoi.enabled = true;
    }

    public void SetStandardControls()
    {
        padImage.sprite = aoiPad;
        skill1.enabled = true;
        skill2.enabled = true;
    }

    public void SetAkaiControls()
    {
        padImage.sprite = akaiPad;
        skill1Akai.enabled = true;
        skill2Akai.enabled = true;
    }

    public void SetKiControls()
    {
        padImage.sprite = kiPad;
        skill1Ki.enabled = true;
        skill2Ki.enabled = true;
    }


    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
