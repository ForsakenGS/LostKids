using UnityEngine;

public class CameraController : MonoBehaviour {

    //Array con las cámaras de la escena
    public GameObject[] cameras;

    //Habitación de inicio y actual
    public int currentRoom = 0;

    //Habitación a la que se está cambiando
    private int nextRoom;

    //Cámara que realizará la transición
    public GameObject transitionCamera;

    //Script de transición de la cámara
    private TransitionCamera scTransitionCamera;

    //Indica si se está realizando la transición
    private bool isChangingCameras;

    private CharacterStatus characterStatus;


   void Start() {

        isChangingCameras = false;
        scTransitionCamera = transitionCamera.GetComponent<TransitionCamera>();
        characterStatus = GameObject.FindGameObjectWithTag("CharacterStatus").GetComponent<CharacterStatus>();


    }

    //Al activarse el script se añade la función ChangeCamera
    void OnEnable()
    {
        CharacterManager.ActiveCharacterChangedEvent += CameraToActivePlayer;
    }

    //Al desactivarse el script se desuscriben las funciones
    void OnDisable()
    {
        CharacterManager.ActiveCharacterChangedEvent -= CameraToActivePlayer;
    }

    /// <summary>
    /// Cambia la camara activa por la correspondiente a la habitacion pasada como parámetro
    /// </summary>
    /// <param name="nextRoom">indice de la habitación a activar</param>
    public void ChangeCamera(int nextRoom) {

        //Si pasamos de habitación se desactiva la camara de la habitación actual y se activa la de transicion
        if(currentRoom != nextRoom) {

            cameras[currentRoom].SetActive(false);
            transitionCamera.SetActive(true);
            
            //Si no se están cambiando las cámaras. Se coloca la transicion en la cámara actual
            if(!isChangingCameras) {
                transitionCamera.transform.position = cameras[currentRoom].transform.position;
                transitionCamera.transform.rotation = cameras[currentRoom].transform.rotation;
                isChangingCameras = true;
            }

            //Se guarda el valor de la siguiente habitacion
            this.nextRoom = nextRoom;

            //Actualizamos la nueva habitación
            currentRoom = nextRoom;

            //Se comienza la transicion
            scTransitionCamera.StartSlerping(cameras[nextRoom].transform.position);
            
        }

        
    }

    //Funcion para detectar el cambio de transicion a camara de habitación
    public void FinishChangingCameras() {

        //Ya no se están cambiando cámaras
        isChangingCameras = false;

        //Se desactiva la camara de transición y se actualiza la posicion de la nueva cámara
        transitionCamera.SetActive(false);
        cameras[nextRoom].transform.position = transitionCamera.transform.position;
        cameras[nextRoom].transform.rotation = transitionCamera.transform.rotation;

        //Se activa la nueva camara
        cameras[nextRoom].SetActive(true);
        
    }

    /// <summary>
    /// Devuelve la cámara activa
    /// </summary>
    public GameObject CurrentCamera()
    {
        if(isChangingCameras)
        {
            return transitionCamera;
        }
        return cameras[currentRoom];
    }

    //Funcion que se activa con el evento de cambio de personaje
    private void CameraToActivePlayer(GameObject character) {
        ChangeCamera(characterStatus.currentRoom);
    }


}
