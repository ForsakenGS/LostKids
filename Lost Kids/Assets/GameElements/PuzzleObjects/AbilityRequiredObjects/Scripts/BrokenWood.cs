using UnityEngine;
using System.Collections;

public class BrokenWood : MonoBehaviour {

    public float expForce;
    public float expRadius;

    public float pieceLifeSpan;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Break(Vector3 breakPoint)
    {
        Vector3 explosionPos = breakPoint;
        foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;

            rb.AddExplosionForce(expForce, explosionPos, expRadius);
            DestroyPiece(rb.gameObject);
        }
    }

    private void DestroyPiece(GameObject piece)
    {
        float destroyTime = pieceLifeSpan + (Random.Range(-0.2f, 0.2f));
        iTween.ScaleTo(piece,iTween.Hash("scale", Vector3.zero,"time",destroyTime,"delay",destroyTime,"easetype",iTween.EaseType.easeInOutSine ));
        
        Destroy(piece, 2*destroyTime);
    }
}
