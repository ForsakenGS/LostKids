using UnityEngine;
using System.Collections;

public class PlayerPush : MonoBehaviour {

    //Distancia de deteccion de objetos usables
    public float useDistance;

    //Variable de control del estado
    private bool use;

    public Vector3 pushNormal;

    private Transform targetTransform;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!use)
            {
                Use();
            }
            else
            {
                use = false;
                targetTransform.parent = null;
            }
        }
    }

    public void Use()
    {
        
        //Se lanza un rayo hacia delante, sumando cierta altura para no lanzarlo desde el suelo
        Ray usingRay = new Ray(this.transform.position + Vector3.up, this.transform.forward);

        //Debug para poder visualizar el rayo en el inspector
        Debug.DrawLine(this.transform.position + Vector3.up, this.transform.position + Vector3.up + this.transform.forward * useDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(usingRay, out hit, useDistance))
        {
            if (hit.collider.tag.Equals("Pushable"))
            {
                //Se obtiene la normal del HIT para saber que parte del objeto ha encontrado el usable.
                //Solo se permite usar los objetos desde la parte de atras ( la normal en z es negativa )
                use = true;
                pushNormal = hit.normal;
                targetTransform = hit.collider.transform;
                this.transform.position = targetTransform.position + 2 * hit.normal;
                targetTransform.parent = this.transform;
                
            }
        }
    }

    public bool GetUse()
    {
        return use;
    }

    public Vector3 GetPushNormal()
    {
        return pushNormal;
    }
}
