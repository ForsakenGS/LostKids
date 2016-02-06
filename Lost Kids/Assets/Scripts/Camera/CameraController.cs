using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject[] cameras;

    public int currentRoom = 0;

    //private IDictionary<string, GameObject> roomCameras;


    void FixedUpdate() {

    }

    public void ChangeCamera(int nextRoom) {

        //Si pasamos de habitación se desactiva la camara de la habitación actual
        if(currentRoom != nextRoom) {

            GameObject cameraClone = Instantiate(cameras[currentRoom]);
            

            
            /*cameras[currentRoom].SetActive(false);

            //Se activa la cámara de la nueva habitación
            cameras[nextRoom].SetActive(!cameras[nextRoom].activeSelf);

            //Actualizamos la nueva habitación
            currentRoom = nextRoom;*/
        }

        
    }


}
