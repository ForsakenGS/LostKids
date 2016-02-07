using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject[] cameras;

    public int currentRoom = 0;
    private int nextRoom;

    public GameObject transitionCamera;

    private bool isChangingCameras;
    private TransitionCamera scTransitionCamera;


   void Start() {
        isChangingCameras = false;
        scTransitionCamera = transitionCamera.GetComponent<TransitionCamera>();
    }


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
            

            
            /*cameras[currentRoom].SetActive(false);

            //Se activa la cámara de la nueva habitación
            cameras[nextRoom].SetActive(!cameras[nextRoom].activeSelf);

            //Actualizamos la nueva habitación
            currentRoom = nextRoom;*/
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


}
