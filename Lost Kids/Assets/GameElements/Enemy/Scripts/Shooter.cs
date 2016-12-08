using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shooter : MonoBehaviour {



    //Referencia al lugar donde aparecen los proyectiles
    public Transform shooterPosition;

    //Velocidad del proyectil
    public float projectileSpeed = 10;

    //Prefab del proyectil que dispara
    public GameObject projectilePrefab;

    //Pool de proyectiles
    private List<GameObject> projectiles;

    //Cantidad del pool
    public int projectilePoolSize=5;

    //Varable para disparar en linea recta o hacia el target exacto
    public bool straightShot = true;

    public Transform KappaShooter;


    // Use this for initialization
    void Start () {

        projectiles = new List<GameObject>();
        for(int i=0;i< projectilePoolSize;i++)
        {
            GameObject proj = (GameObject)Instantiate(projectilePrefab);
            proj.SetActive(false);
            projectiles.Add(proj);
        }
        shooterPosition = transform;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

   public void  ShootAtTarget(GameObject target)
    {
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (!projectiles[i].activeInHierarchy)
            {
                projectiles[i].transform.position = KappaShooter.position;
                
                projectiles[i].transform.LookAt(target.transform.position+Vector3.up);
                if (!straightShot)
                {
                    projectiles[i].GetComponent<Rigidbody>().velocity = projectiles[i].transform.forward * projectileSpeed;
                }
                else
                {
                    projectiles[i].GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;
                }
                projectiles[i].SetActive(true);
                break;
          }
        }
    }
}
