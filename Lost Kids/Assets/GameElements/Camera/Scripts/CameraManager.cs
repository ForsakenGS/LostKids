using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    //Array con las cámaras de la escena
    public GameObject[] cameras;

    //Habitación de inicio y actual
    public int currentRoom = 0;

    //Habitación a la que se está cambiando
    private int nextRoom;

    //Cámara que realizará la transición
    //public GameObject transitionCamera;

    //Script de transición de la cámara
    private TransitionCamera scTransitionCamera;

    //Indica si se está realizando la transición
    private bool isChangingCameras;

    //Evento delegado para lanzar el evento de bloqueo y desbloqueo del juego mientras se cambian las cámaras
    public delegate void LockUnlockAction();
    public static event LockUnlockAction LockEvent;
    public static event LockUnlockAction UnlockEvent;

    public delegate void CutSceneAction();
    public static event CutSceneAction CutSceneEvent;

    //Variables estaticas para manejar el zoom en conversaciones
    public float zoomOutFoV=60;
    public float zoomInFoV = 25;
    public float zoomSpeed = 1;
    private bool zoomed = false;
    public static CameraManager instance = null;


    void Awake() {

        instance = this;
        isChangingCameras = false;

        //if(transitionCamera) {
        //    scTransitionCamera = transitionCamera.GetComponent<TransitionCamera>();
        //}

    }

    //Al activarse el script se añade la función ChangeCamera
    void OnEnable()
    {
        CharacterManager.ActiveCharacterChangedEvent += CameraToActivePlayer;
        /*
        MessageManager.ConversationStartEvent += BeginSmoothZoomIn;
        MessageManager.ConversationEndEvent += BeginSmoothZoomOut;
        */
    }

    //Al desactivarse el script se desuscriben las funciones
    void OnDisable()
    {
        CharacterManager.ActiveCharacterChangedEvent -= CameraToActivePlayer;
        /*
        MessageManager.ConversationStartEvent -= BeginSmoothZoomIn;
        MessageManager.ConversationEndEvent -= BeginSmoothZoomOut;
        */

    }

    /// <summary>
    /// Cambia la camara activa por la correspondiente a la habitacion pasada como parámetro
    /// </summary>
    /// <param name="nextRoom">indice de la habitación a activar</param>
    public void ChangeCamera(int nextRoom) {

        //Si pasamos de habitación se desactiva la camara de la habitación actual y se activa la de transicion
        if(currentRoom != nextRoom) {

            Debug.Log("Bloqueo");
            LockEvent();

            cameras[currentRoom].SetActive(false);
            //transitionCamera.SetActive(true);
            
            //Si no se están cambiando las cámaras. Se coloca la transicion en la cámara actual
            //if(!isChangingCameras) {
            //    transitionCamera.transform.position = cameras[currentRoom].transform.position;
            //    transitionCamera.transform.rotation = cameras[currentRoom].transform.rotation;
            //    isChangingCameras = true;
            //}

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
        //transitionCamera.SetActive(false);
        //cameras[nextRoom].transform.position = transitionCamera.transform.position;
        //cameras[nextRoom].transform.rotation = transitionCamera.transform.rotation;

        //Se activa la nueva camara
        cameras[nextRoom].SetActive(true);

        Debug.Log("Desbloqueo");
        UnlockEvent();
        
    }

    /// <summary>
    /// Devuelve la cámara activa
    /// </summary>
    public GameObject CurrentCamera()
    {
        //if(isChangingCameras)
        //{
        //    return transitionCamera;
        //}
        return cameras[currentRoom];
    }

    //Funcion que se activa con el evento de cambio de personaje
    private void CameraToActivePlayer(GameObject character) {
        ChangeCamera(character.GetComponent<CharacterStatus>().currentRoom);
    }


    public void ChangeCameraFade(GameObject cam, float timeCutScene) {
        if (LockEvent != null)
        {
            LockEvent();
        }
        cameras[currentRoom].SetActive(false);
        cam.SetActive(true);
        Invoke("FinishCutScene", timeCutScene);
    }

    /// <summary>
    /// Activa una camara secundaria desactivando la principal
    /// </summary>
    /// <param name="cam"></param>
    public void ChangeCameraFade(GameObject cam)
    {
        if (LockEvent != null)
        {
            LockEvent();
        }
        cameras[currentRoom].SetActive(false);
        cam.SetActive(true);
    }

    /// <summary>
    /// Llava al evento de finalizacion de cutscene
    /// </summary>
    private void FinishCutScene() {
        CutSceneEvent();
    }

    /// <summary>
    /// Reactiva la camara principal y desactiva la secundaria
    /// </summary>
    /// <param name="cam"></param>
    public void RestoreCamera(GameObject cam) {
        cam.SetActive(false);
        cameras[currentRoom].SetActive(true);
        if (UnlockEvent != null)
        {
            UnlockEvent();
        }
    }


    public void BeginSmoothZoomIn()
    {
        if (!zoomed)
        {
            zoomed = true;
            instance.StartCoroutine(instance.ZoomIn(zoomInFoV, zoomSpeed));
        }

    }

    public void BeginSmoothZoomOut()
    {
        if(zoomed)
        {
            zoomed = false;
            instance.StartCoroutine(instance.ZoomOut(zoomOutFoV, zoomSpeed));
        }
        

    }

    public IEnumerator ZoomIn(float endZoom,float speed)
    {
        float t = 0;
        while (t < 1f) 
        {
            t += Time.deltaTime * speed;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, endZoom, t);

            yield return null;
        }
        if (Camera.main.fieldOfView < endZoom)
        {
            Camera.main.fieldOfView = endZoom;
        }
        yield return 0;
        
    }

    public IEnumerator ZoomOut(float endZoom, float speed)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, endZoom, t);

            yield return null;
        }
        if (Camera.main.fieldOfView > endZoom)
        {
            Camera.main.fieldOfView = endZoom;
        }
        yield return 0;
        zoomed = false;
        yield return 0;
    }



}
