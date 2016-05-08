using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioLoader : MonoBehaviour {

    //Diccionario que almacena la colección de nombres de sonidos y su AudioSource asociado
    private Dictionary<string, AudioSource> sounds;



    //Array con los nombres de los sonidos
    public string[] audioNames;

    //Array con los audioSource del GameObject
    private AudioSource[] audioSources;

    void Awake() {

        sounds = new Dictionary<string, AudioSource>();
        audioSources = GetComponents<AudioSource>();
 
        //Se carga el diccionario con los sonidos
        for(int i = 0; i < audioNames.Length; i++) {
            sounds.Add(audioNames[i], audioSources[i]);
        }

    }

    /// <summary>
    /// Obtiene el sonido a partir de su nombre
    /// </summary>
    /// <param name="name">Nombre del AudioSource que se desea extraer</param>
    /// <returns>Devuelve el AudioSource solicitado</returns>
    public AudioSource GetSound(string name) {

        AudioSource src;

        //Obtiene el AudioSource a partir de su clave
        sounds.TryGetValue(name, out src);

        return src;
    }

}
