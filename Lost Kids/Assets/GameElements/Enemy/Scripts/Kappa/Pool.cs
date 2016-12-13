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
            if (rock.GetComponent<PushableObject>() != null)
            {
                rock.GetComponent<PushableObject>().Release();
                Destroy(rock.GetComponent<PushableObject>());

            }

            rock.GetComponent<SphereCollider>().enabled = true;
            if (rock.GetComponent<BoxCollider>() != null)
            {
                rock.GetComponent<BoxCollider>().enabled = false;
            }
            rock.tag = "Rock";
            //rock.GetComponent<Rigidbody>().isKinematic = true;
            rock.GetComponent<Rigidbody>().velocity = Vector3.zero;
            rock.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            rock.transform.position = finalRockPosition.position;

            GetComponents<BoxCollider>()[0].enabled = false;
            GetComponents<BoxCollider>()[1].size = GetComponents<BoxCollider>()[1].size + new Vector3(0.5f, 5, 0.5f);
            gameObject.layer = LayerMask.NameToLayer("PlayerDetection");
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
