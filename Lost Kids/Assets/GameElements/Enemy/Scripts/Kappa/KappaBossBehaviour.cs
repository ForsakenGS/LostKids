using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Clase que modela el estado y comporamiento del boss Kappa
/// Aparece y desaparece en una serie de pozos disponibles, disparando a los jugadores que tenga cerca
/// Ataca cuerpo a cuerpo cuando un jugador se acerca al pozo
/// Para derrotarlo, deben bloquearse todos los pozos, los cuales se desbloquear pasado un tiempo
/// </summary>
public class KappaBossBehaviour : MonoBehaviour {

    //Posibles estados del boss
    //Diving = Se encuentra escondido bajo agua/tierra a la espera de aparecer
    //Idle = Se encuentra visible sin realizar ninguna accion concreta
    //Moving = Se encuentra en movimiento de ascenso/descenso
    //Attacing = Se encuentra realizando un ataque a melee
    //Breaking = Se encuentra rompiendo una de las rocas que bloquea un poozo
    //Shooting = Disparandoa algun personaje
    //Dead = Derrotado
    public enum States { Diving,Idle,Raising,Attacking,Breaking,Shooting,Dead};

    private States currentState = States.Diving;

    //Lista de charcas
    public GameObject[] poolList;

    public GameObject[] doorsList;

    public GameObject introCutscene;
    public GameObject defeatCutscene;

    //Estado de las charcas disponibles
    private List<Pool> availablePools;

    //Charca en la que se encuentra actualmente
    private Pool actualPool;

    //Tiempo que pasa bajo agua hasta aparecer de nuevo
    public float divingTime=2;

    //Tiempo que se mantiene sobre el agua para atacar
    public float activeTime=2;

    //Velocidad a la que se sumerje
    public float divingSpeed = 10;

    public float shootRange = 15f;
    //Tiempo de recarga del disparo
    public float shootCooldown=1;

    private bool onShootCooldown = false;

    //Tiempo de recarga para romper la piedra
    private float breakCooldown = 2;

    //Referencia al script de disparo
    private Shooter shooter;

    //Varible auxiliar con la distancia al personaje mas cercano
    private float closestDistance;

    //Variable auxiliar para obtener la distancia a un jugador
    private float playerDistance;
    //Variable auxiliar para obtener la distancia a un jugador
    private GameObject closestPlayer;

    [HideInInspector]
    public GameObject nextPool;

    //Variable auxiliar hacia donde apunta
    private Vector3 aimPosition;

    //Referencia al audio lodeader
    private AudioLoader audioLoader;

    //Altura maxima a la que sube el personaje
    private float maxHeight;

    //Altura minima a la que se sumerge el personaje
    private float minHeight;

    //Lista de jugadores detectados en su zona
    private List<GameObject> playersOnSight = new List<GameObject>();

    //Variable auxiliar para el jugador al que va a matar :)
    private GameObject playerUnderAttack;

    private bool initialized = false;

    private int currentRoom = 0;

    public MessageManager messageManager;
    // Listado con índices de los mensajes a mostrar
    public List<int> indexList;

    private KappaAnimationCotroller animationController;
    // Use this for initialization
    void Start () {

        animationController = GetComponent<KappaAnimationCotroller>();
        audioLoader = GetComponent<AudioLoader>();
        shooter = GetComponent<Shooter>();
        availablePools = new List<Pool>();
        //Se inicializa el estado de las charcas disponibles
        for (int i = 0; i < poolList.Length; i++)
        {
            availablePools.Add(poolList[i].GetComponent<Pool>());
            availablePools[i].SetKappa(this);
        }
        //Se inicializa la lista de jugadores a tiro
        currentRoom = 0;
        playersOnSight = CharacterManager.GetCharacterList();

	}

	
	// Update is called once per frame
	void Update () {
        
        //Gira para mirar al jugador mas cercano
        AimAtClosestPlayer();

        if (currentState.Equals(States.Idle))
        {

            if( !onShootCooldown && closestPlayer!=null)
            {
                ShootAtPlayer();               
            }
            
        }
	
	}


    public void BeginEncounter()
    {
        initialized = true;
        Debug.Log("COMENZANDO ENCUENTRO CONTRA EL KAPPA");
        actualPool = availablePools[0];
        transform.position = actualPool.kappaActivePosition.position;
        if (introCutscene != null)
        {
            MusicPlayer.instance.ChangeBackGroundClip(1);
            introCutscene.GetComponent<CutScene>().BeginCutScene(BeginConversation);
            MessageManager.ConversationEndEvent += StartDiving;
        }
        else
        {
            StartDiving();
        }

    }

    private void BeginConversation()
    {
        Joke();
        messageManager.ShowConversation(indexList);
    }

    /// <summary>
    /// Dispara al jugador seleccionada como mas cercano
    /// </summary>
    void ShootAtPlayer()
    {
        onShootCooldown = true;

        //Hay una posibilidad de que en lugar de disparar, haga un vacile
        if (Random.value <= 0.15f)
        {
            Joke();

        }
        else
        {
            //Actualiza su estado
            ChangeState(States.Shooting);

            shooter.ShowRock();
        }


        //Lanza el reseteo de su estado en base al cooldown
        Invoke("ResetShootCoolDown",shootCooldown);
    }

    /// <summary>
    /// Reinicia la posibilidad de dispararde nuevo
    /// </summary>
    void ResetShootCoolDown()
    {
        onShootCooldown = false;
    }

    public void ThrowRock()
    {
        if (closestPlayer != null)
        {
            shooter.ShootAtTarget(closestPlayer);
        }

        if (currentState == States.Shooting)
        {
            ChangeState(States.Idle);
        }

    }

    void Joke()
    {
        shooter.HideRock();
        AudioManager.Play(audioLoader.GetSound("Joke"), false, 1);
        animationController.JibeAnimation();
    }

    /// <summary>
    /// Mueve al personaje a un pozo concreto introducido por parametro
    /// </summary>
    /// <param name="pool"></param>
    public void ChangePool(GameObject pool)
    {
        if (!initialized)
        {
            BeginEncounter();
        }
        else
        {
            StartDiving();
            nextPool = pool;
        }
    }

    private void MoveToPool(GameObject pool)
    {
        transform.position = pool.GetComponent<Pool>().kappaInactivePosition.position;
        actualPool = pool.GetComponent<Pool>();
    }
    
    public void StartAttacking()
    {

    }

    /// <summary>
    /// Comienza la inmersion del personaje
    /// </summary>
    public void StartDiving()
    {
        ChangeState(States.Diving);
        StopAllCoroutines();
        //CancelInvoke();   
        StartCoroutine(Dive());
    }


    /// <summary>
    /// Comienza a emerger el personaje
    /// </summary>
    public void StartAppearing()
    {
 
        if (availablePools.Count < 1)
        {
            StartCoroutine(DestroyRock());
            Defeat();
        }
        else
        {
            if (availablePools.Contains(actualPool))
            {
                StartCoroutine(Appear());
            }
            else
            {
                doorsList[currentRoom].GetComponent<Door>().Activate();
                iTween.ShakePosition(Camera.main.gameObject, new Vector3(1, 1, 0), 2f);
                InputManagerTLK.BeginVibrationTimed(1, 1.5f, true);
                messageManager.ShowMessage(currentRoom + 3);
                currentRoom++;
                CancelInvoke();
                StartCoroutine(DestroyRock());
            }
        }
    }

    /// <summary>
    /// Corutina que mueve suavemente al personaje hacia abajo
    /// </summary>
    /// <param name="pos">Posicion final del objeto al terminar el movimiento</param>
    /// <returns></returns>
    public IEnumerator Dive()
    {

        //AudioManager.Play(audioLoader.GetSound("Splash"), false, 1);
        GetComponent<Collider>().enabled = false;
        
        //Se calcula la posicion destino y se comprueba que no exceda los limites
        Vector3 beginPosition = transform.position;
        Vector3 endPos = actualPool.kappaInactivePosition.position;
  
        //Se mueve hacia arriba a lo largo de varios frames
        float t = 0;
        while (t < 1f) 
        {
            t += Time.deltaTime * divingSpeed;
            transform.position = Vector3.Lerp(beginPosition, endPos, t); 
            yield return null;
        }

        yield return 0;

        //Si  esta cambiando de pozo, se transporta al destino
        if(nextPool!= null)
        {
            MoveToPool(nextPool);
            nextPool = null;
        }
        onShootCooldown = false;
        //Planifica la aparicion pasado el tiempo inmersion
        Invoke("StartAppearing", divingTime);


    }

    /// <summary>
    /// Corutina que hace que el enemigo vaya golpeando la roca hasta romperla
    /// </summary>
    /// <returns></returns>
    public IEnumerator DestroyRock()
    {
        
        ChangeState(States.Breaking);   
        while (true)
        {
           AudioManager.Play(audioLoader.GetSound("HitRock"),false,1);
           actualPool.HitRock();
           yield return new WaitForSeconds(breakCooldown);

        }
        yield return 0;
    }

    /// <summary>
    /// Corutina que mueve suavemente al personaje hacia abajo
    /// </summary>
    /// <param name="pos">Posicion final del objeto al terminar el movimiento</param>
    /// <returns></returns>
    public IEnumerator Appear()
    {
        //Si el pozo ha sido bloqueada, vuelve a intentar aparecer en otro pozo disponible
        if (!actualPool.available)
        {
            actualPool = null;
        }
        else
        {
            ChangeState(States.Raising);
            //AudioManager.Play(audioLoader.GetSound("Splash"), false, 1);

            //Se calcula la posicion destino y se comprueba que no exceda los limites
            Vector3 beginPosition = transform.position;
            Vector3 endPos = actualPool.kappaActivePosition.position;
           
            //Se mueve a lo largo de varios frames
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * divingSpeed;
                transform.position = Vector3.Lerp(beginPosition, endPos, t);
                yield return null;
            }
    
            GetComponent<Collider>().enabled = true;
            yield return new WaitForSeconds(0.5f);
            ChangeState(States.Idle);

            //Planifica su siguiente movimiento
            Invoke("StartDiving", activeTime);
        }

        yield return 0;
    }


    /// <summary>
    /// Metodo de derrota del personaje
    /// </summary>
    private void Defeat()
    {
        CancelInvoke();
        if (defeatCutscene != null)
        {
            indexList = new List<int> { 5, 6 };
            defeatCutscene.GetComponent<CutScene>().BeginCutScene(BeginConversation);
            MessageManager.ConversationEndEvent += DefeatReward;
        }

    }

    void DefeatReward()
    {
        InputManagerTLK.BeginVibrationTimed(1, 1.5f,true);
        iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("amount", new Vector3(1, 1, 0), "time", 1.5f, "delay", 0.5f));
        doorsList[2].GetComponent<Door>().Activate();
        
        ChangeState(States.Dead);
    }


    /// <summary>
    /// Notificacion del cambio de estado de uno de los pozos
    /// </summary>
    /// <param name="pool">Pozo que ha cambiado</param>
    /// <param name="status">Nuevo estado del pozo</param>
    public void WellDisabled(Pool pool)
    {
        
        if (availablePools.Contains(pool))
        {
            availablePools.Remove(pool);
            //Si se bloquea el pozo actual, se tiene que sumergir
            if(pool.Equals(actualPool))
            {
                //CancelInvoke();
                StartDiving();
            }
        }
        
    }

  
    /// <summary>
    /// Metodo que cambia la rotacion y devuelve el personaje mas cercano de los detectados
    /// </summary>
    /// <returns>GameObject correspondiente al personaje mas cercano</returns>
    public GameObject AimAtClosestPlayer()
    {
        if(playersOnSight.Count==0)
        {
            playersOnSight = CharacterManager.GetCharacterList();
        }

        closestDistance = shootRange;
        closestPlayer = null;
        foreach(GameObject player in playersOnSight)
        {
             if (player.GetComponent<CharacterStatus>().IsAlive())          
             {
                playerDistance = Vector3.Distance(transform.position, player.transform.position);
                if (playerDistance < closestDistance)
                {
                    closestPlayer = player;
                    closestDistance = playerDistance;
                }
            }
        }

      

        //Se gira para mirar al personaje mas cercano
        if(closestPlayer!=null)
        {
            //Debug.Log("KAPPA APUNTANDO A DISTANCIA DE :" + closestDistance);
            aimPosition = closestPlayer.transform.position;
            aimPosition.y = transform.position.y;
            transform.LookAt(aimPosition);
            transform.Rotate(0, 180, 0);
        }
        return closestPlayer;
    }


    /// <summary>
    /// Detecta al jugador para atacarlo cuerpo a cuerpo
    /// 
    /// </summary>
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            //Si el boss esta sumergido, emerge en el pozo para matar al jugador
            if (currentState.Equals(States.Diving))
            {
                CancelInvoke();
                StartAppearing();

            }
            playerUnderAttack = col.gameObject;
            KillPlayer();
        }


    }

    /// <summary>
    /// Bloquea al jugador marcandolo como asustado, para poder matarlo tras ejecutar la animacion
    /// </summary>
    /// <param name="player"></param>
    /*
    public void ScarePlayer(GameObject player)
    {
        playerUnderAttack = player;
        ChangeState(States.Attacking);
        player.GetComponent<CharacterStatus>().SetScared(true);
    }
    */

    /// <summary>
    /// Mata al jugador tras ejecutar una animacion de cuerpo a cuerpo
    /// </summary>
    public void KillPlayer()
    {
        CancelInvoke();
        GetComponent<Collider>().enabled = false;
        ChangeState(States.Attacking);
        playerUnderAttack.GetComponent<CharacterStatus>().SetScared(true);
        playerUnderAttack.GetComponent<CharacterStatus>().Invoke("Kill", 1);
        Invoke("EndAttack", 1.5f);
        playerUnderAttack = null;
    }

    /// <summary>
    /// Termina el ataquey vuelve a su estado normal
    /// </summary>
    public void EndAttack()
    {
        shooter.HideRock();
        ChangeState(States.Idle);
        StartDiving();
    }

    void ChangeState(States newState)
    {

        //Debug.Log("KAPPA CAMBIANDO A ESTADO: " + newState.ToString());
        currentState = newState;

        switch(newState)
        {
            case States.Diving:
                AudioManager.Play(audioLoader.GetSound("Dive"), false, 1f);
                animationController.HideAnimation();
                break;
            case States.Attacking:
                AudioManager.Play(audioLoader.GetSound("Attack"), false, 1f);
                animationController.SwipeAnimation();
                break;
            case States.Shooting:
                AudioManager.Play(audioLoader.GetSound("ThrowRock"), false, 1f);
                animationController.ThrowRockAnimation();
                break;
            case States.Raising:
                shooter.HideRock();
                AudioManager.Play(audioLoader.GetSound("Dive"), false, 1f);
                animationController.ShowAnimation();
                break;
            default:
                break;
        }
    }
}
