using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour {

    public static MenuMusic instance = null;

    void Awake()
    {

        if (instance == null)
        {

            instance = this;

        }
        else if (instance != this)
        {

            Destroy(gameObject);

        }
        AudioManager.PlayMusic(GetComponent<AudioSource>(), 1);
        DontDestroyOnLoad(gameObject);

    }

    
}
