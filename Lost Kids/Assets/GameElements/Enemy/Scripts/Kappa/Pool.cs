using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Clase que representa un pozo del enfrentamiento contra el Kappa.
/// Se encarga de controlar su estado y notificarlo al boss Kappa
/// Permite activarlos y desactivarlos, dejando caer una roca que bloquea el pozo
/// </summary>
public class Pool : MonoBehaviour,IActivable {

    [HideInInspector]
    public bool available=true;

    //Referencia a la roca del pozo
    public GameObject rock;

    //Referencia temporal a una nueva roca que se crea en sustitucion de la destruida
    private GameObject tempRock;

    //Posicion donde se instancaran las nuevas rocas
    private Vector3 initialRockPosition;

    //Velocidad adicional para la caida de la roca
    public float rockFallSpeed = 10;

    //Referencia al script del boss
    private KappaBossBehaviour kappa;

    [HideInInspector]
    public bool rockDestroyed=true;


    // Use this for initialization
    void Start()
    {

        initialRockPosition = rock.transform.position;
        available = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// La activacion del pozo lanza una piedra sobre el para bloquearlo durante un tiempo, cuando no exista
    /// </summary>
    public void Activate()
    {
        if (rockDestroyed)
        {
            tempRock= Instantiate(rock, initialRockPosition, Quaternion.identity) as GameObject;
            rock.GetComponent<Rigidbody>().isKinematic = false;
            rock.GetComponent<Rigidbody>().velocity = Vector3.down * rockFallSpeed;
            available = false;
            rockDestroyed = false;
            kappa.PoolStatusChanged(this, false);
            
        }
    }


    /// <summary>
    /// La cancelacion marca el pozo como disponible y el enemigo rompera la roca 
    /// </summary>
    public void CancelActivation()
    {
        //rock.transform.position = initialRockPosition;
        //rock.GetComponent<Rigidbody>().isKinematic = true;
        available = true;
        kappa.PoolStatusChanged(this, true);

    }


    /// <summary>
    /// Se destruye la roca, creando una nueva en su lugar inicial para poder ser activada de nuevo
    /// </summary>
    public void HitRock()
    {
        rock.GetComponent<BreakableRock>().TakeHit();
        if (rock.GetComponent<BreakableRock>().GetCurrentHitPoints() <= 0)
        {
            rock = tempRock;
            rock.GetComponent<Rigidbody>().isKinematic = true;
            rockDestroyed = true;
        }
    }



    /// <summary>
    /// Detecta al jugador al acercarse a un pozo y lo notifica al boss 
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            kappa.PlayerOnPool(this, col.gameObject);
        }
    }

    /// <summary>
    /// Detecta al jugador al acercarse a un pozo y lo notifica al boss 
    /// </summary>
    /// <param name="col"></param>
    void OnColliderEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            kappa.PlayerOnPool(this, col.gameObject);
        }
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
