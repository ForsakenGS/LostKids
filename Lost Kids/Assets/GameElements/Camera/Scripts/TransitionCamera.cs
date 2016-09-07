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
    private CameraManager scCameraManager;
    private GameObject activePlayer;

    //Suavidad de la transición
    public float smooth = 8.0f;


    void Awake() {
        isSlerping = false;

        scCameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();

    }

    void Start() {
        RefreshPlayer(CharacterManager.GetActiveCharacter());
    }

    //Al activarse el script se añade la función RefreshPlayer
    void OnEnable() {
        RefreshPlayer(CharacterManager.GetActiveCharacter());
        CharacterManager.ActiveCharacterChangedEvent += RefreshPlayer;
    }

    //Al desactivarse el script se desuscriben las funciones
    void OnDisable() {
        CharacterManager.ActiveCharacterChangedEvent -= RefreshPlayer;
    }


    void FixedUpdate() {

        //Si está realizandose la transicion
        if (isSlerping) {

            //Se calcula el tiempo desde el inicio
            float timeSinceStarted = Time.time - timeStartedSlerping;

            //Se calcula el porcentaje completado
            float percentageComplete = timeSinceStarted / slerpingTime;

            Vector3 fit;

            //Ajuste provisional de la transición
            if (percentageComplete < 0.5f) {
                fit = Vector3.right * 5 * percentageComplete;
            } else {
                fit = Vector3.right * 5 * (1 - percentageComplete);
            }

            //Se actualiza la posicion dentro de la curva
            transform.position = Vector3.Slerp(startPosition, endPosition, percentageComplete) + fit;// + Vector3.right*10*percentageComplete;


            //Si se ha completado la transicion
            if (percentageComplete >= 1.0f) {

                isSlerping = false;
                //Se llama al fin del cambio de cámaras
                scCameraManager.FinishChangingCameras();
            }
        }

        //Se mira al jugador
        SmoothLookAt(activePlayer);
    }

    /*
         FUTURAS FORMULAS PARA LA CURVA DE BEZIER
         trayectoria.x = pos1.x * Math.pow(1.0-value,3) + 3 * pos2.x * value * Math.pow(1.0-value,2) + 3 * pos3.x * Math.pow(value,2) * (1.0-value) + Math.pow(value,3) * pos4.x;
         trayectoria.y = pos1.y * Math.pow(1.0-value,2) + 3 * pos2.y * value * Math.pow(1.0-value,2) + 3 * pos3.y * Math.pow(value,2) * (1.0-value) + Math.pow(value,3) * pos4.y;
         trayectoria.z = pos1.z * Math.pow(1.0-value,3) + 3 * pos2.z * value * Math.pow(1.0-value,2) + 3 * pos3.z * Math.pow(value,2) * (1.0-value) + Math.pow(value,3) * pos4.z;
    */

    //Función para iniciar la transición
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
    void SmoothLookAt(GameObject pos) {

        //Se calcula la posición relativa del jugador a la cámara
        Vector3 relPlayerPosition = pos.transform.position - transform.position;

        //Crea la rotación entre la posición relativa del jugador y el vector Up para bajar la cámara al jugador
        Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);

        //Actualiza la rotación de la cámara entre la actual y la nueva rotación
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
    }

    private void RefreshPlayer(GameObject character) {
        activePlayer = character;
    }

}
