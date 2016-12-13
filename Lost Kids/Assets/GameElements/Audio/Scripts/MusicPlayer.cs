using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

    public static MusicPlayer instance = null;

    public AudioClip[] soundClips;
    private int clipIndex = 0;
    private AudioSource audio;

    private bool initialized = false;
    public bool dontDestroy = false;

    public bool autoLoop = true;

    void OnDestroy()
    {
        if(initialized)
        {
            AudioManager.StopMusic(audio);
        }
    }

    void Awake()
    {

        if (instance == null)
        {
            instance = this;

        }
        else if (instance != this)
        {
            if (dontDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(instance);
                instance = this;
                initialized = false;
            }

        }
        
        if (dontDestroy)
        {
            DontDestroyOnLoad(gameObject);
        }

    }

    void Start()
    {
        if (!initialized)
        {
            audio = GetComponent<AudioSource>();
            StartCoroutine(PlayBackgroundMusic());
        }
        initialized = true;
    }

    IEnumerator PlayBackgroundMusic()
    {
        while (soundClips.Length > 1 && autoLoop)
        {
            audio.clip = soundClips[clipIndex];
            AudioManager.PlayMusic(audio,false, 0.4f);
            clipIndex++;
            if(clipIndex>=soundClips.Length)
            {
                clipIndex = 0;
            }
            yield return new WaitForSeconds(audio.clip.length);
        }
        audio.clip = soundClips[clipIndex];
        AudioManager.PlayMusic(audio, true, 0.4f);
    }


    public void ChangeBackGroundClip(int index)
    {
        if (soundClips.Length >= index)
        {
            AudioManager.StopMusic(audio);
            clipIndex = index;
            audio.clip = soundClips[clipIndex];
            AudioManager.PlayMusic(audio, true, 0.4f);
        }

    }

}


    

