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


    [System.Flags]
    public enum Characters { Aoi, Akai, Ki };


    [SerializeField]
    [Flags]
    public Characters affectedCharacters;

    public Characters affectedChar;

    //Material cuando el miedo esta activo
    public Material activeMaterial;

    //Material cuando el miedo esta desactivado
    public Material inactiveMaterial;

    //Lista de nombres a los que afecta el miedo
    private List<string> affected;

    //Referencia a la zona de aviso
    private WarningZone warningZone;

    //Referencia a la zona de miedo irreversible
    private FearZone fearZone;

    //Lista de personajes afectados por el miedo
    private HashSet<CharacterStatus> scaredCharacters;


    // Use this for initialization
    void Start () {
        scaredCharacters = new HashSet<CharacterStatus>();
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

        //Cambia el aspecto y desactiva las zonas
        GetComponent<Renderer>().material = inactiveMaterial;
        fearZone.DisableZone();
        warningZone.DisableZone();
        
    }


    /// <summary>
    /// Activa las zonas de miedo 
    /// </summary>
    public void EnableFear()
    {
        GetComponent<Renderer>().material = activeMaterial;
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
        if(inZone)
        if (!scaredCharacters.Contains(st))
        {
            st.SetScared(true);
            scaredCharacters.Add(st);
        }
        else
        {
            st.SetScared(false);
            scaredCharacters.Remove(st);
        }
    }

 }
