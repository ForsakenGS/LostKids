using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}

    /// <summary>
    /// Reproduce un audio simple o en bucle
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    public static void Play(AudioSource source, bool loop) {

        source.loop = loop;
        source.Play();

    }

    /// <summary>
    /// Reproduce un audio simple o en bucle tras un tiempo determinado
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    /// <param name="delay"> Tiempo de espera hasta reproducir el sonido</param>
    public static IEnumerator PlayDelayed(AudioSource source, bool loop, float delay) {

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
            Play(source, loop);
        }

        yield return 0;

    }

    /// <summary>
    /// Reproduce un audio simple o en bucle durante un tiempo determinado
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="loop">Indica si se trata de un bucle</param>
    /// <param name="time"> Tiempo de duración de la reproducción del sonido</param>
    public static IEnumerator PlayDuringTime(AudioSource source, bool loop, float time) {

        float timeSinceStarted = Time.time;
        float endTime = timeSinceStarted + time;
        float currentTime = timeSinceStarted;

        Play(source, loop);

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
    /// Reproduce un efecto de sonido randomizado entre varios
    /// </summary>
    /// <param name="source">AudioSource que se desea reproducir</param>
    /// <param name="clips">Array con los distintos efectos de sonido entre los que se randomizará el sonido</param>
    public static void PlayRandomizeSfx(AudioSource source, AudioClip[] clips) {

        //Genera un índice dentro de los distintos efectos de sonido
        int randomIndex = Random.Range(0, clips.Length);

        source.clip = clips[randomIndex];
        source.Play();

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
    public static IEnumerator Resume(AudioSource source, float delay)
    {
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
    /// Para un sonido
    /// </summary>
    /// <param name="source">AudioSource que se desea parar</param>
    public static void Stop(AudioSource source) {

        if(source.isPlaying) {
            source.Stop();
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
            }
        }

        yield return 0;
    }

}
