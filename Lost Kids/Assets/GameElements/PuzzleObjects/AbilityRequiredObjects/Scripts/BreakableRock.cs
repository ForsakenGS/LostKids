using UnityEngine;
using System.Collections;
/// <summary>
/// Trozo de madera rompible, hereda funcionalidad de BreakableObject
/// Puede recibir un numero de golpes antes de destruirse por completo
/// </summary>
public class BreakableRock : BreakableObject {



	// Use this for initialization
	void Start () {
        base.Start();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Metodo llamado al recibir un golpe del jugador
    /// Cuando los puntos de golpe lleguen a 0, se destruye el objeto
    /// </summary>
    public override void TakeHit()
    {
        GetComponent<Rigidbody>().velocity = Vector3.up * 2;

        //base.TakeHit();

        
       
    }

    /// <summary>
    /// Metodo de destruccion del objeto
    /// </summary>
    private new void Break()
    {
        //Animacion de romperse
        //Destroy(this.gameObject,tiempo de animacion);
        //Destroy(this.gameObject);
        //AudioManager.Play(audioLoader.GetSound("Break"), false, 1);



    }

}
