using UnityEngine;
using System.Collections;
using System;

public class Pool : MonoBehaviour,IActivable {

    [HideInInspector]
    public bool available=true;

    public GameObject rock;

    private GameObject tempRock;

    private Vector3 initialRockPosition;

    public float rockFallSpeed = 10;

    private KappaBossBehaviour kappa;

    [HideInInspector]
    public bool rockDestroyed=true;


    /// <summary>
    /// La activacion del pozo lanza una piedra sobre el para bloquearlo durante un tiempo
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

    // Use this for initialization
    void Start () {

        initialRockPosition = rock.transform.position;
        available = true;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            kappa.PlayerOnPool(this, col.gameObject);
        }
    }

    void OnColliderEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            kappa.PlayerOnPool(this, col.gameObject);
        }
    }

    public void SetKappa(KappaBossBehaviour k)
    {
        kappa = k;
    }
}
