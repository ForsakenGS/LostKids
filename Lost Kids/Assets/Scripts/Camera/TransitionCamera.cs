using UnityEngine;
using System.Collections;

public class TransitionCamera : MonoBehaviour {

    public float slerpingTime;

    private float timeStartedSlerping;

    private bool isSlerping;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private CameraController cameraController;
    private Transform player;

    public float smooth = 1.5f;

    void Start() {
        isSlerping = false;
        cameraController = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraController>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        this.gameObject.SetActive(false);
    }


    void Update(){

        //Si está realizandose la transicion
        if (isSlerping) {
            
            //Se calcula el tiempo desde el inicio
            float timeSinceStarted = Time.time - timeStartedSlerping;

            //Se calcula el porcentaje completado
            float percentageComplete = timeSinceStarted / slerpingTime;

            //Se actualiza la posicion dentro de la curva
            transform.position = Vector3.Slerp(startPosition, endPosition, percentageComplete);
            

            //Si se ha completado la transicion
            if (percentageComplete >= 1.0f) {
                
                isSlerping = false;
                //Se llama al fin del cambio de cámaras
                cameraController.FinishChangingCameras();
            }
        }

        //Se mira al jugador
        SmoothLookAt(player);
    }

    public void StartSlerping(Vector3 endPos) {
        
        //Actualizamos las posiciones inicial y final
        startPosition = transform.position;
        endPosition = endPos;


        timeStartedSlerping = Time.time;
        isSlerping = true;
    }

    void SmoothLookAt(Transform pos) {
    
        // Create a vector from the camera towards the player.
        Vector3 relPlayerPosition = pos.position - transform.position;

        // Create a rotation based on the relative position of the player being the forward vector.
        Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);

        // Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }
}
