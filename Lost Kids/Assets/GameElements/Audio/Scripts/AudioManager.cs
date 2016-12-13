using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance = null;

    public AudioMixer audioMixer;
    private HashSet<AudioSource> playingMusics;
    private HashSet<AudioSource> playingSounds;

    //Al activarse el script se añade la función Lock
    void OnEnable()
    {

        GameManager.PauseEvent += PauseAllSounds;
        GameManager.UnPauseEvent +=ResumeAllSounds;
    }

    //Al desactivarse el script se desuscriben las funciones
    void OnDisable()
    {

        GameManager.PauseEvent -= PauseAllSounds;
        GameManager.UnPauseEvent -= ResumeAllSounds;
    }
    void Awake() {

        if (instance == null) {
            playingMusics = new HashSet<AudioSource>();
            playingSounds = new HashSet<AudioSource>();
            instance = this;

        } else if (instance != this){

            Destroy (gameObject);

        }
            
        DontDestroyOnLoad (gameObject);

    }

    void Restart()
    {
        playingMusics = new HashSet<AudioSource>();
        playingSounds = new HashSet<AudioSource>();
    }

    /// <summary>
    /// Reproduce un audio simple o en bucle
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    public static void Play(AudioSource source, bool loop, float volume) {

        source.loop = loop;
        source.volume = volume;
        source.Play();
        Instance.playingSounds.Add(source);

    }

    /// <summary>
    /// Reproduce un audio simple o en bucle
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    public static void Play(AudioSource source, bool loop, float volume,float minPitch,float maxPitch)
    {
        source.pitch = Random.Range(minPitch, maxPitch);
        source.loop = loop;
        source.volume = volume;
        source.Play();
        Instance.playingSounds.Add(source);

    }

    /// <summary>
    /// Reproduce un clip de musica en loop
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    public static void PlayMusic(AudioSource source,bool loop, float volume)
    {

        source.loop = loop;
        source.volume = volume;
        source.Play();
        Instance.playingMusics.Add(source);

    }

    /// <summary>
    /// Reproduce un audio simple o en bucle tras un tiempo determinado
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    /// <param name="delay"> Tiempo de espera hasta reproducir el sonido</param>
    /// <param name="volume">Volumen del audio</param>
    public static IEnumerator PlayDelayed(AudioSource source, bool loop, float delay, float volume) {

        //Tiempo desde el inicio
        float timeSinceStarted = Time.time;

        //Tiempo de fin
        float endTime = timeSinceStarted + delay;

        //Tiempo actual
        float currentTime = timeSinceStarted;

        //Mientras no se llegue al tiempo de fin se actualiza el tiempo actual y se libera
        while( currentTime < endTime ) {
            currentTime += Time.deltaTime;
            yield return null;
        }

        //Cuando haya pasado el tiempo se reproduce el sonido
        if(currentTime >= endTime) {
            Play(source, loop, volume);
            Instance.playingSounds.Add(source);
        }

        yield return 0;

    }

    /// <summary>
    /// Reproduce un audio simple o en bucle durante un tiempo determinado
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    /// <param name="time"> Tiempo de duración de la reproducción del sonido</param>
    /// <param name="volume">Volumen del audio</param>
    public static IEnumerator PlayDuringTime(AudioSource source, bool loop, float time, float volume) {

        float timeSinceStarted = Time.time;
        float endTime = timeSinceStarted + time;
        float currentTime = timeSinceStarted;

        Play(source, loop, volume);
        Instance.playingSounds.Add(source);

        while (currentTime < endTime) {
            currentTime += Time.deltaTime;
            yield return null;
        }

        if(currentTime >= endTime) {
            Stop(source);
        }

        yield return 0;

    }

    /// <summary>
    /// Reproduce un audio simple o en bucle con un volumen incremental a su inicio durante un tiempo
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    /// <param name="deltaVolume">Incremento de volumen</param>
    public static IEnumerator FadePlay(AudioSource source, bool loop, float deltaVolume) {

        source.volume = 0;

        Play(source, loop, 0);
        Instance.playingSounds.Add(source);

        while ( source.volume < 1) {
            source.volume += deltaVolume * Time.deltaTime;
            yield return null;
        }

        yield return 0;

    }

    /// <summary>
    /// Reproduce un efecto de sonido randomizado entre varios
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="clips">Array con los distintos efectos de sonido entre los que se randomizará el sonido</param>
    public static void PlayRandomizeSfx(ArrayList sources) {

        //Genera un índice dentro de los distintos efectos de sonido
        int randomIndex = Random.Range(0, sources.Count);

        AudioSource source = (AudioSource) sources[randomIndex];
        source.volume = 0.3f;
        source.Play();
        Instance.playingSounds.Add(source);

    }

    /// <summary>
    /// Pausa un sonido
    /// </summary>
    /// <param name="source">AudioSource que se desea pausar</param>
    public static void Pause(AudioSource source) {

        if (source.isPlaying) {
            source.Pause();
        }

    }

    /// <summary>
    /// Pausa un sonido tras un tiempo determinado
    /// </summary>
    /// <param name="source">AudioSource que se desea pausar</param>
    /// <param name="delay">Tiempo de espera hasta pausar el sonido</param>
    public static IEnumerator Pause(AudioSource source, float delay) {

        if (!source.isPlaying) {

            float timeSinceStarted = Time.time;
            float endTime = timeSinceStarted + delay;
            float currentTime = timeSinceStarted;

            while (currentTime < endTime) {
                currentTime += Time.deltaTime;
                yield return null;
            }

            if (currentTime >= endTime) {
                Pause(source);
            }
        }

        yield return 0;
    }

    /// <summary>
    /// Pausa un audio con un volumen decremental durante un tiempo
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="deltaVolume">Decremento de volumen</param>
    public static IEnumerator FadePause(AudioSource source, float deltaVolume) {

        while (source.volume > 0) {
            source.volume -= deltaVolume * Time.deltaTime;
            yield return null;
        }

        if(source.volume <= 0) {
            Pause(source);
        }

        yield return 0;

    }

    /// <summary>
    /// Reanuda un sonido
    /// </summary>
    /// <param name="source">AudioSource que se desea reanudar</param>
    public static void Resume(AudioSource source) {

        if (!source.isPlaying) {
            source.UnPause();
        }
        
    }

    /// <summary>
    /// Reanuda un sonido tras un tiempo determinado
    /// </summary>
    /// <param name="source">AudioSource que se desea reanudar</param>
    /// <param name="delay">Tiempo de espera hasta reanudar el sonido</param>
    public static IEnumerator Resume(AudioSource source, float delay) {

        if (!source.isPlaying) {

            float timeSinceStarted = Time.time;
            float endTime = timeSinceStarted + delay;
            float currentTime = timeSinceStarted;

            while (currentTime < endTime) {
                currentTime += Time.deltaTime;
                yield return null;
            }

            if (currentTime >= endTime) {
                Resume(source);
            }
        }

        yield return 0;
    }

    /// <summary>
    /// Reanuda un audio con un volumen incremental durante un tiempo
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="deltaVolume">Decremento de volumen</param>
    public static IEnumerator FadeResume(AudioSource source, float deltaVolume) {

        source.volume = 0;

        Resume(source);

        while (source.volume < 1) {
            source.volume += deltaVolume * Time.deltaTime;
            yield return null;
        }

        yield return 0;

    }

    /// <summary>
    /// Para un sonido
    /// </summary>
    /// <param name="source">AudioSource que se desea parar</param>
    public static void Stop(AudioSource source) {

        if(source.isPlaying) {
            source.Stop();
            Instance.playingSounds.Remove(source);
        }

    }

    /// <summary>
    /// Para un sonido
    /// </summary>
    /// <param name="source">AudioSource que se desea parar</param>
    public static void StopMusic(AudioSource source)
    {

        if (source.isPlaying)
        {
            source.Stop();
            Instance.playingMusics.Remove(source);
        }

    }

    /// <summary>
    /// Para un sonido tras un tiempo determinado
    /// </summary>
    /// <param name="source">AudioSource que se desea pausar</param>
    /// <param name="delay">Tiempo de espera hasta parar el sonido</param>
    public static IEnumerator Stop(AudioSource source, float delay) {

        if (!source.isPlaying) {

            float timeSinceStarted = Time.time;
            float endTime = timeSinceStarted + delay;
            float currentTime = timeSinceStarted;

            while (currentTime < endTime) {
                currentTime += Time.deltaTime;
                yield return null;
            }

            if (currentTime >= endTime) {
                Stop(source);
                Instance.playingSounds.Remove(source);
            }
        }

        yield return 0;
    }


    /// <summary>
    /// Para un audio con un volumen decremental durante un tiempo
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="deltaVolume">Decremento de volumen</param>
    public static IEnumerator FadeStop(AudioSource source, float deltaVolume) {

        while (source.volume > 0) {
            source.volume -= deltaVolume * Time.deltaTime;
            yield return null;
        }

        if (source.volume <= 0) {
            Stop(source);
            Instance.playingSounds.Remove(source);
        }

        yield return 0;

    }
    
    public static void PauseAllSounds()
    {
        foreach (AudioSource sound in Instance.playingSounds)
        {
            Pause(sound);
        }
    }

    public static void ResumeAllSounds()
    {
        foreach (AudioSource sound in Instance.playingSounds)
        {
            Resume(sound);
        }
    }

    public static AudioManager Instance
    {
        //Devuelve la instancia actual o crea una nueva si aun no existe
        get { return instance ?? (instance = new GameObject("AudioManager").AddComponent<AudioManager>()); }
    }


    public void LoadAudioMixer()
    {
        audioMixer = Resources.Load("AudioMixerTLK") as AudioMixer;
    }


    public static AudioMixer GetAudioMixer()
    {
        if(Instance.audioMixer==null)
        {
            Instance.LoadAudioMixer();
        }
        return Instance.audioMixer;
    }




}
