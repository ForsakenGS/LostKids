using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KappaBossBehaviour : MonoBehaviour {


    public enum States { Diving,Idle,Moving,Attacking,Breaking,Shooting,Dead};

    private States currentState = States.Diving;

    //Lista de charcas
    public GameObject[] poolList;

    [HideInInspector]
    //Estado de las charcas disponibles
    public List<Pool> availablePools;

    //Charca en la que se encuentra actualmente
    private Pool actualPool;

    //Profundidad a la que se sumerje
    public float divingDepth = 3;

    //Tiempo que pasa bajo agua hasta aparecer de nuevo
    public float divingTime=2;

    //Tiempo que se mantiene sobre el agua para atacar
    public float activeTime=2;

    //Velocidad a la que se sumerje
    public float divingSpeed = 10;

    //Tiempo de recarga del disparo
    public float shootCooldown=1;

    //Tiempo de recarga para romper la piedra
    public float breakCooldown = 2;

    //Referencia al script de disparo
    private Shooter shooter;

    //Varible auxiliar con la distancia al personaje mas cercano
    private float closestDistance;

    //Variable auxiliar para obtener la distancia a un jugador
    private float playerDistance;
    //Variable auxiliar para obtener la distancia a un jugador
    private GameObject closestPlayer;

    //Variable auxiliar hacia donde apunta
    private Vector3 aimPosition;

    //Referencia al audio lodeader
    private AudioLoader audioLoader;

    //Altura maxima a la que sube el personaje
    private float maxHeight;

    //Altura minima a la que se sumerge el personaje
    private float minHeight;

    //Lista de jugadores detectados en su zona
    private List<GameObject> playersOnSight;

    //Variable auxiliar para el jugador al que va a matar :)
    private GameObject playerUnderAttack;


    //Recompensa que se activa al derrotar al enemigo
    public GameObject defeatReward;

	// Use this for initialization
	void Start () {

        audioLoader = GetComponent<AudioLoader>();
        shooter = GetComponent<Shooter>();
        //Se inicializa el estado de las charcas disponibles
        for (int i = 0; i < poolList.Length; i++)
        {
            availablePools.Add(poolList[i].GetComponent<Pool>());
            availablePools[i].SetKappa(this);
        }
        //Se inicializa la lista de jugadores a tiro
        playersOnSight = new List<GameObject>();

        //Se inicializa la posicion y los limites de altura
        maxHeight = transform.position.y;
        transform.position -= new Vector3(0, divingDepth, 0);
        minHeight = transform.position.y;

        Invoke("StartAppearing", divingTime);

	
	}
	
	// Update is called once per frame
	void Update () {

        AimAtClosestPlayer();
        if (currentState.Equals(States.Idle))
        {
            if(closestPlayer!=null)
            {
                ShootAtPlayer();               
            }
        }
	
	}

    void ShootAtPlayer()
    {
        currentState = States.Shooting;
        AudioManager.Play(audioLoader.GetSound("Spit"), false, 1);
        shooter.ShootAtTarget(closestPlayer);
        Invoke("ShootCoolDown",shootCooldown);
    }

    /// <summary>
    /// Reinicia la posibilidad de dispararde nuevo
    /// </summary>
    void ShootCoolDown()
    {
        currentState = States.Idle;
    }

    /// <summary>
    /// Mueve al personaje a un pozo concreto introducido por parametro
    /// </summary>
    /// <param name="pool"></param>
    private void MoveToPool(Pool pool)
    {
        actualPool = pool;
        Vector3 newPosition = actualPool.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

    }
    /// <summary>
    /// Mueve al personaje a un pozo aleatorio de los disponibles en el momento
    /// </summary>
    private void MoveToNextPool()
    {
        //Busca un pozo disponible aleatorio, dando prioridad a los que tengan una piedra para romper
        if(availablePools.Count>1)
        {
            //Busca el primer pozo que tenga una piedra sin romper
            for (int i = 0; i < availablePools.Count;i++)
            {
                if(!availablePools[i].rockDestroyed)
                {
                    actualPool = availablePools[i];
                    break;
                }
            }
            //Si no lo encuentra, se mueve a una aleatoria
            if (actualPool == null)
            {
                actualPool = availablePools[Random.Range(0, availablePools.Count)];
            }
        }
        else
        {
            actualPool = availablePools[0];
        }

        //Se mueve a la posicion de la nueva charca
        MoveToPool(actualPool);
        
        
    }

    public void StartAttacking()
    {

    }

    /// <summary>
    /// Comienza la inmersion del personaje
    /// </summary>
    public void StartDiving()
    {
        CancelInvoke();
        
        StartCoroutine(Dive());
    }


    /// <summary>
    /// Comienza a emerger el personaje
    /// </summary>
    public void StartAppearing()
    {
        CancelInvoke();
        if (availablePools.Count < 1)
        {
            Defeat();
        }
        else
        {
            if (actualPool == null)
            {
                MoveToNextPool();
            }
            StartCoroutine(Appear());
        }
    }

    /// <summary>
    /// Corutina que mueve suavemente al personaje hacia abajo
    /// </summary>
    /// <param name="pos">Posicion final del objeto al terminar el movimiento</param>
    /// <returns></returns>
    public IEnumerator Dive()
    {
        AudioManager.Play(audioLoader.GetSound("Splash"), false, 1);
        GetComponent<Collider>().enabled = false;
        playersOnSight.Clear();
        currentState = States.Diving;
        
        //Se calcula la posicion destino y se comprueba que no exceda los limites
        Vector3 beginPosition = transform.position;
        Vector3 endPos = beginPosition;
        endPos.y -= divingDepth;
        if(endPos.y<minHeight)
        {
            endPos.y = minHeight;
        }

        float t = 0;
        while (t < 1f) // Hasta que no acabe el frame no permite otro movimiento
        {
            t += Time.deltaTime * divingSpeed;
            transform.position = Vector3.Lerp(beginPosition, endPos, t); // interpola el movimiento entre dos puntos
            yield return null;
        }
        actualPool = null;
        yield return 0;
        Invoke("StartAppearing", divingTime);


    }

    /// <summary>
    /// Corutina que hace que el enemigo vaya golpeando la roca hasta romperla
    /// </summary>
    /// <returns></returns>
    public IEnumerator DestroyRock()
    {
        currentState = States.Breaking;
        while (!actualPool.rockDestroyed)
        {
            actualPool.HitRock();
            if (!actualPool.rockDestroyed)
            {
                yield return new WaitForSeconds(breakCooldown);
            }
            else
            {
                yield return 0;
            }
        }
        currentState = States.Idle;
        yield return new WaitForSeconds(breakCooldown);
        StartCoroutine(Appear());
    }

    /// <summary>
    /// Corutina que mueve suavemente al personaje hacia abajo
    /// </summary>
    /// <param name="pos">Posicion final del objeto al terminar el movimiento</param>
    /// <returns></returns>
    public IEnumerator Appear()
    {

            if (!actualPool.available)
            {
                actualPool = null;
                StartAppearing();
            }
            else
            {
                if (!actualPool.rockDestroyed)
                {
                    StopAllCoroutines();
                    CancelInvoke();
                    StartCoroutine(DestroyRock());
                }
                else
                {
                    AudioManager.Play(audioLoader.GetSound("Splash"), false, 1);

                    //Se calcula la posicion destino y se comprueba que no exceda los limites
                    Vector3 beginPosition = transform.position;
                    Vector3 endPos = beginPosition;
                    endPos.y += divingDepth;
                    if (endPos.y > maxHeight)
                    {
                        endPos.y = maxHeight;
                    }

                    float t = 0;
                    while (t < 1f) // Hasta que no acabe el frame no permite otro movimiento
                    {
                        t += Time.deltaTime * divingSpeed;
                        transform.position = Vector3.Lerp(beginPosition, endPos, t); // interpola el movimiento entre dos puntos
                        yield return null;
                    }

                    yield return 0;
                    GetComponent<Collider>().enabled = true;

                    //Si esta apareciendo para atacar al jugador, lo mata al llegar arriba
                    if (currentState.Equals(States.Attacking))
                    {
                        KillPlayer();
                    }
                    else
                    {
                        currentState = States.Idle;
                    }


                    Invoke("StartDiving", activeTime);
                }
            }
        

    }


    /// <summary>
    /// Metodo de derrota del personaje
    /// </summary>
    private void Defeat()
    {
        CancelInvoke();
        currentState = States.Dead;

        defeatReward.SetActive(true);
        
    }


    /// <summary>
    /// Notificacion del cambio de estado de uno de los pozos
    /// </summary>
    /// <param name="pool">Pozo que ha cambiado</param>
    /// <param name="status">Nuevo estado del pozo</param>
    public void PoolStatusChanged(Pool pool,bool status)
    {
        if(status)
        {
            if(!availablePools.Contains(pool))
            {
                availablePools.Add(pool);
            }
        }
        else
        {
            if (availablePools.Contains(pool))
            {
                availablePools.Remove(pool);
                if(pool.Equals(actualPool))
                {
                    CancelInvoke();
                    Invoke("StartDiving", 0);
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            
            if (!playersOnSight.Contains(col.gameObject))
            {
                playersOnSight.Add(col.gameObject);
            }
        }
    }

    /// <summary>
    /// Metodo que cambia la rotacion y devuelve el personaje mas cercano de los detectados
    /// </summary>
    /// <returns>GameObject correspondiente al personaje mas cercano</returns>
    public GameObject AimAtClosestPlayer()
    {
        closestDistance = float.MaxValue;
        closestPlayer = null;
        foreach(GameObject player in playersOnSight)
        {
             if (player.GetComponent<CharacterStatus>().IsAlive())
               
             {
                playerDistance = Vector3.Distance(transform.position, player.transform.position);
                if (playerDistance < closestDistance)
                {
                    closestPlayer = player;
                }
            }
        }

        if(closestPlayer!=null)
        {
            aimPosition = closestPlayer.transform.position;
            aimPosition.y = transform.position.y;
            transform.LookAt(aimPosition);
        }
        return closestPlayer;
    }


    /// <summary>
    /// Notifica por parte de un pozo de que el personaje esta en su radio
    /// 
    /// </summary>
    /// <param name="pool">Pozo donde se encuentra el jugador</param>
    /// <param name="player">GameObject correspondente al personaje</param>
    public void PlayerOnPool(Pool pool,GameObject player)
    {
        //Si el boss esta sumergido, emerge en el pozo para matar al jugador
        if(currentState.Equals(States.Diving))
        {
            CancelInvoke();
            MoveToPool(pool);
            StartAppearing();
            //Animacion y sonido al matar al jugador

            ScarePlayer(player);

        }
        //Si el boss se encuentra ya en la piscina, mata al jugador
        else if(currentState.Equals(States.Shooting)&& pool.Equals(actualPool))
        {
            //Animacion y sonido al matar al jugador
            ScarePlayer(player);

        }

    }

    public void ScarePlayer(GameObject player)
    {
        playerUnderAttack = player;
        currentState = States.Attacking;
        player.GetComponent<CharacterStatus>().SetScared(true);
    }

    public void KillPlayer()
    {

        playerUnderAttack.GetComponent<CharacterStatus>().Invoke("Kill", 1);
        Invoke("EndAttack", 1.5f);
        playerUnderAttack = null;
    }

    public void EndAttack()
    {
        currentState = States.Idle;
    }
}
