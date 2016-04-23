using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AudioLoader))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(TriangleExplosion))]
public abstract class BreakableObject : MonoBehaviour, IBreakable {


    public int maxHitPoints;
    protected int currentHitPoints;

    public Texture[] textures;

    private AudioLoader audioLoader;

    public void Break()
    {
        if (audioLoader != null && audioLoader.GetSound("Break") != null)
        {
            AudioManager.Play(audioLoader.GetSound("Break"), false, 1);
        }
        StartCoroutine(gameObject.GetComponent<TriangleExplosion>().SplitMesh(true));
    }

    public virtual void TakeHit()
    {
        if(audioLoader!=null && audioLoader.GetSound("Hit")!= null)
        {
            AudioManager.Play(audioLoader.GetSound("Hit"), false, 1);
        }
        currentHitPoints--;
        if(currentHitPoints <= 0)
        {
            Break();
        }
        else if(textures[currentHitPoints]!=null)
        {
            GetComponent<Renderer>().material.mainTexture=textures[currentHitPoints];
        }
    }

    // Use this for initialization
    public void Start () {
        currentHitPoints = maxHitPoints;
        if (textures[currentHitPoints] != null)
        {
            GetComponent<Renderer>().material.mainTexture = textures[currentHitPoints];
        }
        audioLoader = GetComponent<AudioLoader>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Metodo que se llama desde el inspector cuando se modifica algun valor
    /// Se utiliza para mantener el mismo tamaño entre los puntos, sus velocidades, y sus delays
    /// </summary>
    void OnValidate()
    {
        if (maxHitPoints > 0)
        {
            if (textures.Length != maxHitPoints+1)
            {
                textures = copyAndResize(textures, maxHitPoints+1);
            }
        }
        
    }

    private Texture[] copyAndResize(Texture[] array, int size)
    {
        Texture[] temp = new Texture[size];
        Array.Copy(array, temp, Math.Min(array.Length, size));
        array = temp;
        return array;

    }

    public int GetCurrentHitPoints()
    {
        return currentHitPoints;
    }
}
