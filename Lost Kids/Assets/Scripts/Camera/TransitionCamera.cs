using UnityEngine;
using System.Collections;

public class TransitionCamera : MonoBehaviour {

    //Tiempo que dura la transición
    public float slerpingTime;

    //Tiempo desde que se empezó la transición
    private float timeStartedSlerping;

    //Indica si se encuentra realizando la transición
    private bool isSlerping;

    //Posiciones inicial y final de la transición
    private Vector3 startPosition;
    private Vector3 endPosition;

    //Controlador de la cámara y posición del jugador objetivo
    private CameraController cameraController;
    private Transform player;

    //Suavidad de la transición
    public float smooth = 1.5f;

    void Awake() {

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
            transform.position = Vector3.Slerp(startPosition, endPosition, percentageComplete);// + Vector3.right*10*percentageComplete;
            

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

        //Se actualiza el tiempo en el que se empezó la transición
        timeStartedSlerping = Time.time;

        //Se indica que se está realizando la transición
        isSlerping = true;
    }

    //Función para mirar suavemente al jugador
    void SmoothLookAt(Transform pos)
    {

        //Se calcula la posición relativa del jugador a la cámara
        Vector3 relPlayerPosition = pos.position - transform.position;

        //Crea la rotación entre la posición relativa del jugador y el vector Up para bajar la cámara al jugador
        Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);

        //Actualiza la rotación de la cámara entre la actual y la nueva rotación
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }

}
