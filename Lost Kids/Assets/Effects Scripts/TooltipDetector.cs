using UnityEngine;
using System.Collections;

/// <summary>
/// Componente que detecta al jugador en la zona activa de un objeto, permitiendo su uso
/// y mostrando un tooltip sobre el jugador
/// </summary>
public class TooltipDetector : MonoBehaviour {
    //Icono asociado al objeto
    public Sprite tooltipImage;

    //Habilidad requerida para poder usar el objeto
    public CharacterAbility requiredAbility;

    //Referencia al objeto usable del que depende
    private UsableObject usableParent;

    //Referencia al controlador del canvas del jugador
    private CharacterIcon icon;

    // Use this for initialization
    void Start() {
        usableParent = GetComponentInParent<UsableObject>();
    }


    void OnDestroy()
    {
        if(icon!=null)
        {
            icon.ActiveCanvas(false);
        }
    }


    /// <summary>
    /// Cuando el jugador entra en el detector, si dispone de las habilidades necesarias
    /// muestra el icono correspondiente y permite el uso del objeto
    /// </summary>
    /// <param name="other"></param>
	void OnTriggerEnter(Collider other) {
        if (CharacterManager.IsActiveCharacter(other.gameObject)) {
            if (requiredAbility == null || other.gameObject.GetComponent(requiredAbility.GetType()) != null) {
                //Si el objeto tiene un icono asociado, lo muestra y activa el canvas del jugador
                if (tooltipImage != null) {
                    icon = other.gameObject.GetComponentInChildren<CharacterIcon>();
                    icon.ActiveCanvas(true);
                    icon.SetImage(tooltipImage);
                }
                //Activa el uso del objeto
                if (usableParent != null) {
                    usableParent.canUse = true;
                } else if (requiredAbility != null) {
                    // Intenta preparar la habilidad 'AstralProjection'
                    AstralProjectionAbility ability = other.gameObject.GetComponent<AstralProjectionAbility>();
                    if (ability != null) {
                        ability.SetWall(GetComponentInParent<AstralProjectionWall>());
                    }
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (icon != null)
        {
            if (CharacterManager.IsActiveCharacter(other.gameObject))
            {
                icon.ActiveCanvas(false);
                GameObject character=other.gameObject;
            
                Ray usingRay = new Ray(character.transform.position + Vector3.up, character.transform.forward);

                RaycastHit hit;
                if (Physics.Raycast(usingRay, out hit, 2) && hit.collider.transform==this.transform.parent)
                {
                    icon.ActiveCanvas(true);
                }
            }
        }
    }

    /// <summary>
    /// Cuando el jugador sale de la zona, desactiva su icono y la posibilidad de usar el objeto
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other) {

        if (CharacterManager.IsActiveCharacter(other.gameObject)) {
            if (icon != null) {
                icon.ActiveCanvas(false);
                if (usableParent != null) {
                    usableParent.canUse = false;
                } else if (requiredAbility != null) {
                    // Intenta reiniciar la habilidad 'AstralProjection'
                    AstralProjectionAbility ability = other.gameObject.GetComponent<AstralProjectionAbility>();
                    if (ability != null) {
                        ability.SetWall(null);
                    }
                }
                icon = null;
            }
        }
    }
}