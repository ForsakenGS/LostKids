using UnityEngine;
using System.Collections;
/// <summary>
/// Script basico para implementar la accion de "Usar" de un personaje
/// Lanza un rayo para detectar objetos usables (MARCADO CON TAG 'Usable')enfrente de el y usarlos.
/// </summary>
public class PlayerUse : MonoBehaviour {

    //Distancia de deteccion de objetos usables
    public float useDistance;

    //Tiempo necesario para volver a usar la habilidad
    public float useCooldown = 1;

    //Variable de control del estado
    private bool isUsing;

    private UsableObject objectInUse;

    private Vector3 rayOffset;

    //Mientras este a true, no permite usar 
    private bool onCooldown = false;



    // Use this for initialization
    void Start() {
        rayOffset = new Vector3(0, GetComponentInChildren<Renderer>().bounds.size.y / 2, 0);
    }

    // Update is called once per frame
    void Update() {

    }

    public bool CanUse() {
        bool res = false;
        //Se lanza un rayo hacia delante, sumando cierta altura para no lanzarlo desde el suelo
        Ray usingRay = new Ray(this.transform.position + rayOffset, this.transform.forward);

        //Debug para poder visualizar el rayo en el inspector
        Debug.DrawLine(this.transform.position + rayOffset, this.transform.position + rayOffset + this.transform.forward * useDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(usingRay, out hit, useDistance)) {
            if (!hit.collider.isTrigger) {
                objectInUse = hit.collider.gameObject.GetComponent<UsableObject>();
                if (objectInUse != null) {
                    // Comprueba qeu siel personaje se encuentra en la zona correcta y el objeto no está en uso
                    res = (objectInUse.canUse && !objectInUse.onUse);
                }
            }
        }

        return res;
    }

    public bool Use() {
        //Se lanza un rayo hacia delante, sumando cierta altura para no lanzarlo desde el suelo
        Ray usingRay = new Ray(this.transform.position + rayOffset, this.transform.forward);

        //Debug para poder visualizar el rayo en el inspector
        Debug.DrawLine(this.transform.position + rayOffset, this.transform.position + rayOffset + this.transform.forward * useDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(usingRay, out hit, useDistance)) {
            if (!hit.collider.isTrigger) {
                objectInUse = hit.collider.gameObject.GetComponent<UsableObject>();
                if (objectInUse != null) {
                    onCooldown = true;
                    Invoke("ResetCooldown", useCooldown);

                    //Se obtiene la normal del HIT para saber que parte del objeto ha encontrado el usable.
                    //Solo se permite usar los objetos desde la parte de atras ( un angulo de 180 respecto al frente )
                    Vector3 normalLocal = hit.transform.TransformDirection(hit.normal);
                    float angle = Vector3.Angle(hit.normal, hit.transform.forward);

                    if (objectInUse.canUse) {

                        objectInUse.Use();
                        if (objectInUse.type.Equals(UsableTypes.Hold)) {
                            isUsing = true;
 
                            if(objectInUse.usePosition!=null)
                            {
                                this.transform.position = objectInUse.usePosition.position;
                                Vector3 look = objectInUse.transform.position;
                                look.y = transform.position.y;
                                this.transform.LookAt(look);
                            }
                            else
                            {
                                Vector3 frontPosition = hit.point + GetComponent<CapsuleCollider>().radius * hit.normal;
                                Vector3 lookPosition = hit.point;
                                lookPosition.y = transform.position.y;
                                frontPosition.y = transform.position.y;
                                this.transform.position = frontPosition;

                                this.transform.LookAt(lookPosition);
                            }
                        }
                    }
                }
            }
        }
        return isUsing;
    }

    public bool IsNPC() {
        bool res = false;
        //Se lanza un rayo hacia delante, sumando cierta altura para no lanzarlo desde el suelo
        Ray usingRay = new Ray(this.transform.position + rayOffset, this.transform.forward);

        //Debug para poder visualizar el rayo en el inspector
        Debug.DrawLine(this.transform.position + rayOffset, this.transform.position + rayOffset + this.transform.forward * useDistance, Color.red);
        // Comprueba si se trata de un NPC (Kodama o Tanuki)
        RaycastHit hit;
        if (Physics.Raycast(usingRay, out hit, useDistance)) {
            if (!hit.collider.isTrigger) {
                objectInUse = hit.collider.gameObject.GetComponent<Kodama>();
                if (objectInUse == null) {
                    objectInUse = hit.collider.gameObject.GetComponent<Tanuki>();
                }
                if (objectInUse != null) {
                    res = objectInUse.canUse;
                }
            }
        }

        return res;
    }

    public bool UsingLever()
    {
        return objectInUse != null && objectInUse.GetComponent<Lever>() != null;
    }

    /// <summary>
    /// Devuelve true si el jugador esta usando algun objeto
    /// </summary>
    /// <returns></returns>
    public bool IsUsing() {
        return isUsing;
    }


    /// <summary>
    /// Detiene el uso del objeto
    /// </summary>
    public void StopUsing() {
        if (isUsing) {
            isUsing = false;
            objectInUse.CancelUse();
            objectInUse = null;
        }

    }

    void ResetCooldown() {
        onCooldown = false;
    }
}
