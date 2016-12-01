using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Clase que representa un pozo del enfrentamiento contra el Kappa.
/// Se encarga de controlar su estado y notificarlo al boss Kappa
/// Permite activarlos y desactivarlos, dejando caer una roca que bloquea el pozo
/// </summary>
public class Pool : MonoBehaviour {

    [HideInInspector]
    public bool available=true;

    public GameObject rock;

    public Transform finalRockPosition;

    public Transform kappaActivePosition;
    public Transform kappaInactivePosition;

    //Referencia al script del boss
    private KappaBossBehaviour kappa;

    // Use this for initialization
    void Start()
    {

        available = true;

    }

    /// <summary>
    /// La activacion del pozo lanza una piedra sobre el para bloquearlo durante un tiempo, cuando no exista
    /// </summary>
    public void OnTriggerEnter(Collider col)
    {
        if(col.gameObject==rock)
        { 
            kappa.WellDisabled(this);
            rock.GetComponent<PushableObject>().Release();
            Destroy(rock.GetComponent<PushableObject>());
            rock.transform.position = finalRockPosition.position;
        }
    }



    /// <summary>
    /// Se destruye la roca, creando una nueva en su lugar inicial para poder ser activada de nuevo
    /// </summary>
    public void HitRock()
    {
        rock.GetComponent<BreakableRock>().TakeHit();
    }

    /// <summary>
    /// Guarda la referencia del boss para enviarle las notificaciones
    /// </summary>
    /// <param name="k"></param>
    public void SetKappa(KappaBossBehaviour k)
    {
        kappa = k;
    }
}
