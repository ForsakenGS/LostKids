﻿using UnityEngine;
using System.Collections;
/// <summary>
/// Script basico para implementar la accion de "Usar" de un personaje
/// Lanza un rayo para detectar objetos usables (MARCADO CON TAG 'Usable')enfrente de el y usarlos.
/// </summary>
public class PlayerUse : MonoBehaviour {

    //Distancia de deteccion de objetos usables
    public float useDistance;

    //Variable de control del estado
    private bool isUsing;

    private UsableObject objectInUse;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool Use()
    {
		//Se lanza un rayo hacia delante, sumando cierta altura para no lanzarlo desde el suelo
        Ray usingRay = new Ray(this.transform.position - Vector3.up*0.5f, this.transform.forward);

        //Debug para poder visualizar el rayo en el inspector
        Debug.DrawLine(this.transform.position + Vector3.up, this.transform.position + Vector3.up*0.2f +this.transform.forward*useDistance,Color.red);

        RaycastHit hit;
        if (Physics.Raycast(usingRay, out hit, useDistance))
        {
            if (hit.collider.tag.Equals("Usable"))
            {
                //Se obtiene la normal del HIT para saber que parte del objeto ha encontrado el usable.
                //Solo se permite usar los objetos desde la parte de atras ( la normal en z es negativa )
                if (hit.normal.z < 0)
                {
                    objectInUse = hit.collider.gameObject.GetComponent<UsableObject>();
                    objectInUse.Use();
                    if (objectInUse.type.Equals(UsableObject.Usables.Hold))
                    {
                        isUsing = true;
                        this.transform.position = hit.collider.transform.position + 2 * hit.normal;
                    }
                }
            }
        }
		return isUsing;
    }

    /// <summary>
    /// Devuelve true si el jugador esta usando algun objeto
    /// </summary>
    /// <returns></returns>
    public bool IsUsing()
    {
        return isUsing;
    }


    /// <summary>
    /// Detiene el uso del objeto
    /// </summary>
    public void StopUsing()
    {
        if (isUsing)
        {
            isUsing = false;
            objectInUse.CancelUse();
            objectInUse = null;
        }
        
    }
}
