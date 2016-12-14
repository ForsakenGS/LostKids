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

    private GameObject activeRock = null;

    // Use this for initialization
    void Start () {

        projectiles = new List<GameObject>();
        for(int i=0;i< projectilePoolSize;i++)
        {
            GameObject proj = (GameObject)Instantiate(projectilePrefab);
            proj.SetActive(false);
            projectiles.Add(proj);
        }
        shooterPosition = KappaShooter.transform;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HideRock()
    {
        if(activeRock!=null)
        {
            activeRock.SetActive(false);
            activeRock = null;
        }
    }

    public void ShowRock()
    {
        if (activeRock == null)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (!projectiles[i].activeInHierarchy)
                {
                    activeRock = projectiles[i];
                    activeRock.transform.parent = shooterPosition;
                    activeRock.transform.localPosition = Vector3.zero;
                    activeRock.SetActive(true);
                    break;
                }
            }
        }

    }

   public void  ShootAtTarget(GameObject target)
    {
        if (activeRock !=null)
        {
            activeRock.transform.LookAt(target.transform.position + Vector3.up*1.1f);
            if (!straightShot)
            {
                activeRock.GetComponent<Rigidbody>().velocity = activeRock.transform.forward * projectileSpeed;
            }
            else
            {
                activeRock.GetComponent<Rigidbody>().velocity = transform.forward * projectileSpeed;
            }
            activeRock.GetComponent<KappaProjectile>().Activate();
            activeRock = null;
        }
    }
}
