using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Representa una zona que produce algun tipo de miedo. Contiene dos zonas, una de aviso 
/// y una de miedo irreversible donde el jugador queda bloqueado.
/// Permite parametrizar el personaje o personajes a los que afecta el miedo.
/// Se puede activar o desactivar como resultado de un puzzle o accion de NPCs
/// </summary>
public class FearSource : MonoBehaviour,IActivable {


    //Personajes a los que afecta el miedo
    public List<CharacterName> affectedCharacters;

    //Objetos para desactivar
    public GameObject[] ObjectsToDisable;

    //Material cuando el miedo esta desactivado
    public GameObject inactiveObject;

    //Referencia a la zona de aviso
    private WarningZone warningZone;

    //Referencia a la zona de miedo irreversible
    private FearZone fearZone;

    //Lista de personajes afectados por el miedo
    private HashSet<CharacterStatus> scaredCharacters;


    // Use this for initialization
    void Start () {
        scaredCharacters = new HashSet<CharacterStatus>();

        warningZone = GetComponentInChildren<WarningZone>();
        warningZone.SetAffectedCharacters(affectedCharacters);

        fearZone =GetComponentInChildren<FearZone>();
        fearZone.SetAffectedCharacters(affectedCharacters);
        


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Desactiva las zonas de miedo, liberando a los personajes que se encontrasen asustados en ese momento
    /// </summary>
    public void DisableFear()
    {
        //Cancela el estado de miedo de los personajes asustados
        foreach(CharacterStatus st in scaredCharacters)
        {
            st.SetScared(false);
        }
        scaredCharacters.Clear();


        fearZone.DisableZone();
        warningZone.DisableZone();

        //Cambia el aspecto y desactiva las zonas
        inactiveObject.SetActive(true);
        inactiveObject.transform.parent = null;
        for(int i = 0; i < ObjectsToDisable.Length; i++) {
            ObjectsToDisable[i].SetActive(false);
        }
    }


    /// <summary>
    /// Activa las zonas de miedo 
    /// </summary>
    public void EnableFear()
    {
        this.gameObject.SetActive(true);

        for (int i = 0; i < ObjectsToDisable.Length; i++)
        {
            ObjectsToDisable[i].SetActive(true);
        }

        fearZone.EnableZone();
        warningZone.EnableZone();
    }


    /// <summary>
    /// Activacion del conjunto, se ejecuta como resultadode un puzzle o NPC
    /// Desactiva la fuente del miedo
    /// </summary>
    public void Activate()
    {

        DisableFear();
    }


    /// <summary>
    /// Al cancelar su activacion, vuelve a activar la fuente del miedo
    /// </summary>
    public void CancelActivation()
    {
        EnableFear();
    }


    /// <summary>
    /// Notifica la entrada o salida de un personaje en la zona de miedo irreversible
    /// </summary>
    /// <param name="character">GameObject del personaje</param>
    /// <param name="inZone">true cuando el personaje entra, false cuando sale</param>
    public void CharacterOnFearZone(GameObject character, bool inZone)
    {
        
        CharacterStatus st = character.GetComponent<CharacterStatus>();
        if (inZone)
        {
            if (!scaredCharacters.Contains(st))
            {
                st.SetScared(true);
                scaredCharacters.Add(st);
            }
        }
        else
        {
            st.SetScared(false);
            scaredCharacters.Remove(st);
        }
    }

 }
