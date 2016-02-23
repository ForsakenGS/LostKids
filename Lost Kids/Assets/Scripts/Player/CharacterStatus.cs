using UnityEngine;
using System.Collections;

public class CharacterStatus : MonoBehaviour {

    public GameObject characterStatusManager;
    private CharacterStatusManager statusManager;

    public int actualRoom = 0;

    private bool isAlive
    {
        set; get;
    }

	// Use this for initialization
	void Start () {
        isAlive = true;
        if(characterStatusManager==null)
        {
            characterStatusManager = GameObject.FindGameObjectWithTag("CharacterStatusManager");
        }
        statusManager = characterStatusManager.GetComponent<CharacterStatusManager>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Mata al personaje y notifica su nuevo estado al manager, que desactivara su control y lo movera al checkpoint
    /// </summary>
    public void Kill()
    {
        Debug.Log("Muerto");
        isAlive = false;
        GetComponent<Renderer>().enabled = false;
        //Animacion, Efectos, Cambio de imagen.....
        GetComponent<Renderer>().enabled = false; //Temporal
        statusManager.CharacterKilled(this);    
    }

    public void Ressurect()
    {
        isAlive = true;
        GetComponent<Renderer>().enabled = true;
        //Animacion, Efectos, Cambio de imagen.....
    }

    public bool IsAvailable()
    {
        return isAlive;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

}
