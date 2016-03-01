using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    //Velocidad con la que la cámara seguirá al jugador. Más es más rápido 
    public float smooth = 1.5f;

    //Referencia al jugador
    private Transform player;

    //Posición relativa de la camara desde el jugador
    private Vector3 relCameraPos;

    //Distancia cámara-jugador
    private float relCameraPosMag;

    //Nueva posición que tendrá la cámara
    private Vector3 newPos;

    //"Radio" del jugador
    public float playerRadius = 0.5f;

    //Vector de las posiciones de la cámara para seguir al jugador.
    private Vector3[] cameraPoints;

    //Numero de puntos de cámara para seguir al jugador.
    public int numCameraPoints = 11;

    //Incremento relativo entre las posiciones de las cámaras.
    private float deltaCameraPoints;

    void Awake()
    {

    }

    void Start()
    {

        RefreshPlayer();

        //SE HA MOVIDO DEL AWAKE PARA PODER TENER EL PLAYER
        //Calcula la posición relativa de la cámara
        relCameraPos = transform.position - player.position;

        //Calcula la distancia del vector entre la cámara del jugador + un ajuste del tamaño del mismo
        relCameraPosMag = relCameraPos.magnitude - playerRadius;

        //Inicializa el vector de puntos de cámara
        cameraPoints = new Vector3[numCameraPoints];

        //Incremento entre puntos de cámara
        deltaCameraPoints = 1.0f / (numCameraPoints - 1);

    }

    //Al activarse el script se añade la función ChangeCamera
    void OnEnable()
    {
        RefreshPlayer();
        CharacterManager.ActiveCharacterChangedEvent += RefreshPlayer;
    }

    //Al desactivarse el script se desuscriben las funciones
    void OnDisable()
    {
        CharacterManager.ActiveCharacterChangedEvent -= RefreshPlayer;
    }


    void FixedUpdate()
    {

        //Posición actual de la cámara
        Vector3 standardPos = player.position + relCameraPos;

        //Posición encima del jugador de la cámara
        Vector3 abovePos = player.position + Vector3.up * relCameraPosMag;

        //Iterador para rellenar el vector de puntos de cámara
        float ite = 0.0f;

        //Recorremos el vector de los puntos de cámara y se comprueba si se puede ver al jugador desde cada punto mediante rayos
        for (int i = 0; i < cameraPoints.Length; i++)
        {

            //Se va desplazando la cámara siguiendo la curva
            cameraPoints[i] = Vector3.Slerp(standardPos, abovePos, ite += deltaCameraPoints);

            //Si se puede ver al jugador terminamos el bucle
            if (ViewingPosCheck(cameraPoints[i])) {

                //Si ha chocado con el jugador se actualiza la nueva posición de la cámara
                newPos = cameraPoints[i];
                break;
            }
        }

        //Actualizamos la posición de la cámara entre su posición actual y la nueva posición
        transform.position = Vector3.Slerp(transform.position, newPos, smooth * Time.deltaTime);

        //Hacemos que la cámara mire al jugador
        SmoothLookAt(player);
    }

    //Función para comprobar si la posición de la cámara con respecto al jugador es válida mediante el lanzamiento de rayos
    bool ViewingPosCheck(Vector3 checkPos)
    {
        RaycastHit hit;

        //Si el rayo entre la cámara y el jugador choca con algo
        if (Physics.Raycast(checkPos, player.position - checkPos, out hit, relCameraPosMag)) {

            //Si no es el jugador la posición es inválida y devuelve false
            if (hit.transform != player) {
                
                return false;

            }

        }

        //Si se encuentra una posición válida se devuelve true
        return true;
    }

    //Función para mirar suavemente al jugador
    void SmoothLookAt(Transform pos){

        //Se calcula la posición relativa del jugador a la cámara
        Vector3 relPlayerPosition = pos.position - transform.position;

        //Crea la rotación entre la posición relativa del jugador y el vector Up para bajar la cámara al jugador
        Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);

        //Actualiza la rotación de la cámara entre la actual y la nueva rotación
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }

    private void RefreshPlayer()
    {

        if (CharacterManager.GetActiveCharacter() != null) {
        player = CharacterManager.GetActiveCharacter().transform;
        }
    }
}
