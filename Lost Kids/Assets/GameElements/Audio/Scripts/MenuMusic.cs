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

        DontDestroyOnLoad(gameObject);

    }

    
}
