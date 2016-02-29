using UnityEngine;
using System.Collections;


/// <summary>
/// Script basico para implementar la accion de "Agarrar" un objeto para moverlo
/// Lanza un rayo para detectar objetos empujables (MARCADO CON TAG 'Pushable') enfrente de el y agarrarlos.
/// Una vez agarrado, el objeto se coloca como hijo del jugador, y solo se debe permitir moverlo en sus ejes
/// </summary>
public class PlayerPush : MonoBehaviour {

    //Distancia de deteccion de objetos usables
    public float useDistance;

    //Variable de control del estado
    private bool pushing;

    public Vector3 pushNormal;

    private Transform targetTransform;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //PROVISIONAL, DEBE CONTROLARSE EN EL INPUTMANAGER
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!pushing)
            {
                Grab();
            }
            else
            {
                Release();
            }
        }
    }


    /// <summary>
    /// Funcion que agarra el objeto movible y ajusta la posicion y el movimiento del jugador respecto al objeto
    /// </summary>
    public void Grab()
    {
        
        //Se lanza un rayo hacia delante, sumando cierta altura para no lanzarlo desde el suelo
        Ray usingRay = new Ray(this.transform.position + Vector3.up, this.transform.forward);

        //Debug para poder visualizar el rayo en el inspector
        #if UNITY_EDITOR
        Debug.DrawLine(this.transform.position + Vector3.up, this.transform.position + Vector3.up + this.transform.forward * useDistance, Color.red);
        #endif

        RaycastHit hit;
        if (Physics.Raycast(usingRay, out hit, useDistance))
        {
            if (hit.collider.tag.Equals("Pushable"))
            {
                
                pushing = true;
                //Se obtiene la normal de la direccion por donde se agarra el objeto
                pushNormal = hit.normal;
                targetTransform = hit.collider.transform;
                //Se coloca el personaje alineado con el objeto y el objeto se marca como hijo del jugador
                this.transform.position = targetTransform.position + 2 * hit.normal;
                targetTransform.parent = this.transform;


            }
        }
    }

    /// <summary>
    /// Libera el objeto y devuelve el control normal al jugador
    /// </summary>
    public void Release()
    {
        pushing = false;
        targetTransform.parent = null;
    }

    /// <summary>
    /// Devuelve si el objeto esta siendo agarrado
    /// </summary>
    /// <returns></returns>
    public bool IsPhushing()
    {
        return pushing;
    }

    /// <summary>
    /// Devuelve la normal de agarre del objeto
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPushNormal()
    {
        return pushNormal;
    }
}
