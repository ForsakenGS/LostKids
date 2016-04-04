using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour {

    //Array de mensajes del juego
    private ArrayList messages;

    //Fichero que contiene los mensajes
    public TextAsset messageFile;

    //Texto que se muestra en pantalla
    public Text text;

    //Marco del texto en pantalla
    public Image frame;

    //Imagen del kodama
    public Image kodama;

    //Lineas del mensaje
    private string[] lines;

    // Máximo de caracteres por línea
    public int maxCharacterLine = 100;

    //Indices para recorrer las lineas del mensaje
    private int startIndex;
    private int endIndex;

    //Velocidad de muestra de las letras
    public float normalLetterSpeed;
    public float fastLetterSpeed;
    private float letterSpeed;

    //Evento delegado para lanzar el evento de bloqueo y desbloqueo del juego mientras se muestran mensajes
    public delegate void LockUnlockAction();
    public static event LockUnlockAction LockUnlockEvent;

    private AudioLoader audioLoader;
    //Array con los efectos de sonido
    private ArrayList messagesSfxs;

    /// <summary>
    /// Maquina de estados para el MessageManager
    /// FastMessage: Es el estado por defecto, se activa cuando el jugador pulsa la tecla por primera vez
    /// EndMessage: Se activa cuando no hay mas mensajes para mostrar
    /// NextMessage: Se activa cuando se han mostrado las 4 lineas en pantalla y quedan más lineas por mostrar
    /// </summary>
    private enum State { FastMessage, EndMessage, NextMessage };
    private State messageState;

    // Use this for initialization
    void Start() {

        audioLoader = GetComponent<AudioLoader>();
        messagesSfxs = new ArrayList();

        //Rellenamos el array de efectos de sonido
        messagesSfxs.Add(audioLoader.GetSound("Message1"));
        messagesSfxs.Add(audioLoader.GetSound("Message2"));
        messagesSfxs.Add(audioLoader.GetSound("Message3"));
        messagesSfxs.Add(audioLoader.GetSound("Message4"));
        messagesSfxs.Add(audioLoader.GetSound("Message5"));

        //Estado inicial
        messageState = State.FastMessage;

        messages = new ArrayList();

        //Se inicializa la velocidad a la velocidad normal
        letterSpeed = normalLetterSpeed;

        //Se cargan los mensajes desde fichero
        FillMessages();
    }

    /// <summary>
    /// Funcion para llenar los mensajes desde el fichero de texto
    /// </summary>
    private void FillMessages() {

        //Se extrae todo el texto del fichero
        string fullText = messageFile.text;

        //Se formatea por lineas y se almacena en el vector
        string[] formatText = fullText.Split("\n"[0]);

        //Se añaden las lineas al vector de mensajes
        for (int i = 0; i < formatText.Length; i++) {
            messages.Add((string) formatText[i]);
        }

    }

    /// <summary>
    /// Funcion para mostrar el mensaje perteneciente al trigger por pantalla
    /// </summary>
    /// <param name="index">Indice del mensaje que se desea mostrar</param>
    /// <returns></returns>
    public void ShowMessage(int index) {

        //Se activan el marco y el texto
        frame.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        kodama.gameObject.SetActive(true);

        //Se bloquea el resto del juego
        if (LockUnlockEvent != null) {
            LockUnlockEvent();
        }

        //Se llama a la función para extraer el mensaje
        getMessage(index);

    }


    /// <summary>
    /// Extrae el mensaje de la línea pasada por defecto
    /// </summary>
    private void getMessage(int index) {

        //Carga el mensaje pasado por indice
        string msg = (string) messages[index];

        //Se separa en lineas
        lines = SeparateInLines(msg);

        //Se inicializan los índices
        startIndex = 0;

        if (lines.Length <= 4) {
            endIndex = lines.Length;
        } else {
            endIndex = 4;
        }

        //Se inicia la corrutina para ir mostrando el mensaje letra por letra
        StartCoroutine(TypeText());
    }

    // Divide un mensaje en distintas líneas atendiendo al número máximo de caracteres por línea
    string[] SeparateInLines (string msg) {
        List<string> lines = new List<string>();

        // Divide el mensaje en palabras
        string[] words = msg.Split(' ');
        // Inserta las palabras una a una para formar las distintas líneas
        lines.Insert(0, words[0]);
        for (int i = 1; i < words.Length; ++i) {
            if (lines[lines.Count-1].Length + words[i].Length >= maxCharacterLine) {
                lines.Insert(lines.Count, words[i]);
            } else {
                lines[lines.Count - 1] += (" " + words[i]);
            }
        }

        return lines.ToArray();
    }

    /// <summary>
    /// Corrutina para mostrar el texto letra por letra
    /// </summary>
    /// <returns></returns>
    IEnumerator TypeText() {

        //Se reproduce un sonido de mensaje aleatorio
        AudioManager.PlayRandomizeSfx(messagesSfxs);

        //Se limpia el mensaje en pantalla
        text.text = string.Empty;

        //Se recorren los indices para mostrar los mensajes en pantalla
        for (int i = startIndex; i < endIndex; i++) {
            //Se extrae la linea como array de caracteres
            char[] line = lines[i].ToCharArray();

            //Se recorre la linea y se va añadiendo letra a letra con un retraso de la velocidad de letra
            for (int j = 0; j < line.Length; j++) {
                text.text += line[j];
                yield return new WaitForSeconds(letterSpeed);
            }

            //Se añade el salto de linea
            text.text += "\n";

        }

        //Si el indice final es menor que el numero de lineas del mensaje se cambia el estado al siguiente mensaje
        if (endIndex < lines.Length) {
            messageState = State.NextMessage;
        } else {//Si no, se cambia al estado del mensaje final
            messageState = State.EndMessage;
        }

        yield return 0;
    }

    /// <summary>
    /// Funcion para pasar el mensaje en pantalla rápidamente
    /// </summary>
    /// <returns></returns>
    public void SkipText() {

        switch (messageState) {
            //Si está en el estado por defecto, se cambia la velocidad de letra
            case State.FastMessage:
                letterSpeed = fastLetterSpeed;
                break;
            //Si está en el estado de fin de mensaje
            case State.EndMessage:

                //Se cambia al estado por defecto
                messageState = State.FastMessage;

                //Se recupera la velocidad de letra
                letterSpeed = normalLetterSpeed;

                //Se desbloquea el resto del juego
                if (LockUnlockEvent != null) {
                    LockUnlockEvent();
                }

                //Se oculta la interfaz de mensajes
                frame.gameObject.SetActive(false);
                text.gameObject.SetActive(false);
                kodama.gameObject.SetActive(false);
                break;
            //Si está en el estado de siguiente mensaje
            case State.NextMessage:
                //Se cambia al estado por defecto
                messageState = State.FastMessage;

                //Se recupera la velocidad de letra
                letterSpeed = normalLetterSpeed;

                //Se incrementan los índicas
                startIndex += 4;
                endIndex += 4;

                if (endIndex > lines.Length) {
                    endIndex = lines.Length;
                }

                //Se inicia la corrutina para mostrar el mensaje letra por letra
                StartCoroutine(TypeText());
                break;
        }

    }

    /// <summary>
    /// Funcion que devuelve si el mensaje ha terminado de mostrarse
    /// </summary>
    /// <returns></returns>
    public bool MessageEnded() {
        return messageState.Equals(State.EndMessage);
    }


}
