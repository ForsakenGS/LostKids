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

    //"Radio" del jugador
    public float playerRadius = 0.5f;

    private CameraScroller cameraScroller;

    void Start() {

        cameraScroller = GameObject.FindGameObjectWithTag("CameraParallax").GetComponent<CameraScroller>();

        RefreshPlayer();
        UpdateParams();

    }

    //Al activarse el script se añade la función ChangeCamera
    void OnEnable()
    {
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

        transform.position = Vector3.Slerp(transform.position, standardPos, smooth * Time.deltaTime);

        cameraScroller.UpdateScrollSpeed(transform.position, standardPos);

    }



    private void RefreshPlayer()
    {

        if (CharacterManager.GetActiveCharacter() != null) {
            player = CharacterManager.GetActiveCharacter().transform;

            //UpdateParams();
        }
    }

    public void UpdateParams() {

        //Calcula la posición relativa de la cámara
        relCameraPos = transform.position - player.position;

        //Calcula la distancia del vector entre la cámara del jugador + un ajuste del tamaño del mismo
        relCameraPosMag = relCameraPos.magnitude - playerRadius;

    }
}
