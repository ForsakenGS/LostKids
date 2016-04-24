using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FearSource : MonoBehaviour,IActivable {


    [System.Flags]
    public enum Characters { Aoi, Akai, Ki };


    [SerializeField]
    [Flags]
    public Characters affectedCharacters;

    public Characters affectedChar;

    public Material activeMaterial;

    public Material inactiveMaterial;


    private List<string> affected;

    private WarningZone warningZone;

    private FearZone fearZone;


    // Use this for initialization
    void Start () {

        affected = new List<string>();
        
        /*
        for (int i = 0; i < affectedCharacters.Count; i++)
        {
            affected.Add(affectedCharacters.ToString());
        }
        */

        affected.Add(affectedChar.ToString());
        
        warningZone = GetComponentInChildren<WarningZone>();
        warningZone.SetAffectedCharacters(affected);

        fearZone =GetComponentInChildren<FearZone>();
        fearZone.SetAffectedCharacters(affected);
        


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DisableFear()
    {
        GetComponent<Renderer>().material = inactiveMaterial;
        fearZone.DisableZone();
        warningZone.DisableZone();
        
    }

    public void EnableFear()
    {
        GetComponent<Renderer>().material = activeMaterial;
        fearZone.EnableZone();
        warningZone.EnableZone();
    }

    public void Activate()
    {
        DisableFear();
    }

    public void CancelActivation()
    {
        EnableFear();
    }
}
