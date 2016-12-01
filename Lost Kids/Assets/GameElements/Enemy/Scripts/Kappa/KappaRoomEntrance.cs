using UnityEngine;
using System.Collections;

public class KappaRoomEntrance : MonoBehaviour {

    public GameObject Kappa;

    public GameObject roomPool;

	void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            Kappa.GetComponent<KappaBossBehaviour>().ChangePool(roomPool);
            GetComponent<Collider>().enabled = false;
        }

    }
}
