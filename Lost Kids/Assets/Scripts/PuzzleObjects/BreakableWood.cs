using UnityEngine;
using System.Collections;
/// <summary>
/// Trozo de madera rompible, hereda funcionalidad de BreakableObject
/// Puede recibir un numero de golpes antes de destruirse por completo
/// </summary>
public class BreakableWood : BreakableObject {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Metodo llamado al recibir un golpe del jugador
    /// Cuando los puntos de golpe lleguen a 0, se destruye el objeto
    /// </summary>
    public new void TakeHit()
    {

        //Animacion, cambio de aspecto
        currentHitPoints--;
        if (currentHitPoints<=0)
        {
            Break();
        }
    }

    /// <summary>
    /// Metodo de destruccion del objeto
    /// </summary>
    private new void Break()
    {
        //Animacion de romperse
        //Destroy(this.gameObject,tiempo de animacion);
        //Destroy(this.gameObject);
        StartCoroutine(gameObject.GetComponent<TriangleExplosion>().SplitMesh(true));
    }

}
