using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour {
    //Ficheros que contienen los mensajes
    public TextAsset englishMessageFile;
    public TextAsset spanishMessageFile;
    private TextAsset messageFile;
    // Máximo de líneas por mensaje
    public int maxLines = 3;
    // Máximo de caracteres por línea
    public int maxCharacterLine = 85;
    //Velocidad de muestra de las letras
    public float normalLetterSpeed;
    public float fastLetterSpeed;

    //Texto que se muestra en pantalla
    public Text text;
    //Marco del texto en pantalla
    public Image frame;
    // Imagen que se está mostrando
    public Image shownImg;

    // Imagen y etiqueta Kodama
    [Header("Kodama Settings")]
    public string kodamaTag = "kodama";
    public Sprite kodamaImg;
    // Imagen y etiqueta Tanuki
    [Header("Tanuki Settings")]
    public string tanukiTag = "tanuki";
    public Sprite tanukiImg;
    // Imagen y etiqueta Kappa
    [Header("Kappa Settings")]
    public string kappaTag = "kappa";
    public Sprite kappaImg;
    // Imagen y etiqueta Aoi
    [Header("Aoi Settings")]
    public string aoiTag = "aoi";
    public Sprite aoiImg;
    // Imagen y etiqueta Akai
    [Header("Akai Settings")]
    public string akaiTag = "akai";
    public Sprite akaiImg;
    // Imagen y etiqueta Ki
    [Header("Ki Settings")]
    public string kiTag = "ki";
    public Sprite kiImg;

    //Eventos delegados para lanzar el evento de bloqueo y desbloqueo del juego mientras se muestran mensajes
    public delegate void LockUnlockAction();
    public static event LockUnlockAction LockEvent;
    public static event LockUnlockAction UnlockEvent;
    public static event LockUnlockAction ConversationStartEvent;
    public static event LockUnlockAction ConversationEndEvent;


    //Array de mensajes del juego
    private ArrayList messages;
    //Lineas del mensaje
    private string[] lines;
    //Indices para recorrer las lineas del mensaje
    private int startIndex;
    private int endIndex;
    //Velocidad de muestra de las letras
    private float letterSpeed;
    // Indica que se está mostrando una conversación
    private bool isConversation;
    private AudioLoader audioLoader;
    //Array con los efectos de sonido
    private ArrayList messagesSfxs;


    /// <summary>
    /// Maquina de estados para el MessageManager
    /// FastMessage: Es el estado por defecto, se activa cuando el jugador pulsa la tecla por primera vez
    /// EndMessage: Se activa cuando no hay mas mensajes para mostrar
    /// NextMessage: Se activa cuando se han mostrado las 4 lineas en pantalla y quedan más lineas por mostrar
    /// </summary>
    private enum State { NoMessage, FastMessage, EndMessage, NextMessage, NextConversationMessage };
    private State messageState;

    // Use this for references
    void Awake() {
        // Elige el fichero de texto acorde al idioma
        if ((LocalizationManager.language == null) || (LocalizationManager.language.Equals("es"))) {
            messageFile = spanishMessageFile;
        } else {
            messageFile = englishMessageFile;
        }
    }

    // Use this for initialization
    void Start() {

        audioLoader = GetComponent<AudioLoader>();
        messagesSfxs = new ArrayList();

        //Rellenamos el array de efectos de sonido
        messagesSfxs.Add(audioLoader.GetSound("Message1"));
        messagesSfxs.Add(audioLoader.GetSound("Message2"));
        //Estado inicial
        messageState = State.NoMessage;

        messages = new ArrayList();

        //Se inicializa la velocidad a la velocidad normal
        letterSpeed = normalLetterSpeed;
        isConversation = false;
        //shownImg = null;

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
        for (int i = 0 ; i < formatText.Length ; i++) {
            messages.Add((string) formatText[i]);
        }

    }

    /// <summary>
    /// Funcion para mostrar el mensaje perteneciente al trigger por pantalla
    /// </summary>
    /// <param name="index">Indice del mensaje que se desea mostrar</param>
    /// <returns></returns>
    public void ShowMessage(int index) {
        // Cambia el estado
        messageState = State.FastMessage;
        //Se activan el marco y el texto
        frame.gameObject.SetActive(true);
        text.gameObject.SetActive(true);


        //Se bloquea el resto del juego
        if (LockEvent != null) {
            LockEvent();
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

        // Se muestra la imagen del personaje al que pertenece el mensaje
        shownImg.sprite = CharacterImage(msg);
        shownImg.gameObject.SetActive(true);

        //Se separa en lineas
        lines = SeparateInLines(msg);

        //Se inicializan los índices
        startIndex = 0;

        if (lines.Length <= maxLines) {
            endIndex = lines.Length;
        } else {
            endIndex = maxLines;
        }

        //Se inicia la corrutina para ir mostrando el mensaje letra por letra
        StartCoroutine(TypeText());
    }

    // Recibe un mensaje y devuelve la imagen representativa del interlocutor
    Sprite CharacterImage(string message) {
        // Kodama es el personaje por defecto
        Sprite img = shownImg.sprite;
        string tag = message.Substring(1, message.IndexOf('>') - 1);
        if (tag.Equals(kodamaTag)) {
            img = kodamaImg;
        } else if (tag.Equals(tanukiTag)) {
            img = tanukiImg;
        } else if (tag.Equals(aoiTag)) {
            img = aoiImg;
        } else if (tag.Equals(akaiTag)) {
            img = akaiImg;
        } else if (tag.Equals(kiTag)) {
            img = kiImg;
        } else if (tag.Equals(kappaTag)) {
            img = kappaImg;
        }



        return img;
    }

    // Divide un mensaje en distintas líneas atendiendo al número máximo de caracteres por línea
    string[] SeparateInLines(string msg) {
        List<string> lines = new List<string>();

        // Divide el mensaje en palabras
        string[] words = msg.Split(' ');
        // Inserta las palabras una a una para formar las distintas líneas
        lines.Insert(0, "");
        for (int i = 1 ; i < words.Length ; ++i) {
            if (lines[lines.Count - 1].Length + words[i].Length >= maxCharacterLine) {
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
        for (int i = startIndex ; i < endIndex ; i++) {
            //Se extrae la linea como array de caracteres
            char[] line = lines[i].ToCharArray();

            //Se recorre la linea y se va añadiendo letra a letra con un retraso de la velocidad de letra
            for (int j = 0 ; j < line.Length ; j++) {
                text.text += line[j];
                //Se reconocen algunos caracteres especiales (. , ) que modifiquen la velocidad del texto
                switch (line[j]) {
                    case '.':
                        yield return new WaitForSeconds(4 * letterSpeed);
                        break;
                    case '!':
                        yield return new WaitForSeconds(4 * letterSpeed);
                        break;
                    case '?':
                        yield return new WaitForSeconds(4 * letterSpeed);
                        break;
                    case ',':
                        yield return new WaitForSeconds(2 * letterSpeed);
                        break;
                    default:
                        yield return new WaitForSeconds(letterSpeed);
                        break;
                }

            }
            //Se añade el salto de linea
            text.text += "\n";
        }

        //Si el indice final es menor que el numero de lineas del mensaje se cambia el estado al siguiente mensaje
        if (endIndex < lines.Length) {
            messageState = State.NextMessage;
        } else if (isConversation) {// Si es una conversación, viene otro mensaje nuevo
            messageState = State.NextConversationMessage;
        } else {//Si no, se cambia al estado del mensaje final
            messageState = State.EndMessage;
        }

        yield return 0;
    }

    /// <summary>
    /// Función para mostrar en pantalla una conversación
    /// </summary>
    /// <param name="conversation">Listado con los índices de los mensajes que conforman la conversación</param>
    public void ShowConversation(List<int> conversation) {
        if (ConversationStartEvent != null) {
            ConversationStartEvent();
        }
        StartCoroutine(ShowConversationRoutine(conversation));
    }

    /// <summary>
    /// Corrutina para mostrar una conversación con varios mensajes
    /// </summary>
    /// <param name="conversation">Listado con los índices de los mensajes que conforman la conversación</param>
    /// <returns></returns>
    IEnumerator ShowConversationRoutine(List<int> conversation) {
        isConversation = true;
        for (int mesIndex = 0 ; mesIndex < conversation.Count ; ++mesIndex) {
            // Espera a que el mensaje anterior termine de mostrarse por pantalla
            if (ShowingMessage()) {
                while (!(MessageEnded())) {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            // Si se trata del último mensaje de la conversación, desactiva el flag 
            if (mesIndex == conversation.Count - 1) {
                isConversation = false;
            }
            // Muestra el mensaje
            ShowMessage(conversation[mesIndex]);
        }
        yield return 0;
    }

    /// <summary>
    /// Funcion para pasar el mensaje en pantalla rápidamente
    /// </summary>
    public void SkipText() {
        switch (messageState) {
            //Si está en el estado por defecto, se cambia la velocidad de letra
            case State.FastMessage:
                letterSpeed = fastLetterSpeed;
                break;
            //Si está en el estado de siguiente mensaje
            case State.NextMessage:
                //Se cambia al estado por defecto
                messageState = State.FastMessage;

                //Se recupera la velocidad de letra
                letterSpeed = normalLetterSpeed;

                //Se incrementan los índicas
                startIndex += maxLines;
                endIndex += maxLines;

                if (endIndex > lines.Length) {
                    endIndex = lines.Length;
                }
                //Se inicia la corrutina para mostrar el mensaje letra por letra
                StartCoroutine(TypeText());
                break;
            // Si está en el estado de siguiente mensaje de conversación
            case State.NextConversationMessage:
                //Se cambia al estado final
                messageState = State.EndMessage;
                //Se recupera la velocidad de letra
                letterSpeed = normalLetterSpeed;
                //Oculta la imagen del mensaje anterior
                shownImg.gameObject.SetActive(false);
                break;
            //Si está en el estado de fin de mensaje
            case State.EndMessage:
                //Se cambia al estado por defecto
                //messageState = State.FastMessage;
                //Se recupera la velocidad de letra
                letterSpeed = normalLetterSpeed;
                //Se desbloquea el resto del juego
                if (UnlockEvent != null) {
                    UnlockEvent();
                }

                if (ConversationEndEvent != null) {
                    ConversationEndEvent();
                    ConversationEndEvent = null;
                }
                //Se oculta la interfaz de mensajes
                frame.gameObject.SetActive(false);
                text.gameObject.SetActive(false);
                shownImg.gameObject.SetActive(false);
                //shownImg = null;
                messageState = State.NoMessage;
                break;
        }
    }

    /// <summary>
    /// Funcion que devuelve si el mensaje ha terminado de mostrarse
    /// </summary>
    /// <returns></returns>
    public bool MessageEnded() {
        return (messageState.Equals(State.EndMessage));
    }

    /// <summary>
    /// Funcion que devuelve si se esta mostrando un mensaje
    /// </summary>
    /// <returns></returns>
    public bool ShowingMessage() {
        return (!messageState.Equals(State.NoMessage));
    }
}
