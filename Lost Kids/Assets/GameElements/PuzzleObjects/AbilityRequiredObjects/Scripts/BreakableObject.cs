using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AudioLoader))]
[RequireComponent(typeof(AudioSource))]
public abstract class BreakableObject : MonoBehaviour, IBreakable {


    public int maxHitPoints;
    protected int currentHitPoints;

    public Texture[] textures;

    private AudioLoader audioLoader;

    public GameObject brokenObject;

    public Vector3 breakPoint;

    public void Break() {
        if (audioLoader != null && audioLoader.GetSound("Break") != null) {
            AudioManager.Play(audioLoader.GetSound("Break"), false, 1);
        }
        if (brokenObject == null && gameObject.GetComponent<TriangleExplosion>()!=null)
        {
            StartCoroutine(gameObject.GetComponent<TriangleExplosion>().SplitMesh(true));
        }
        else
        {
            GameObject broken = Instantiate(brokenObject, transform.position, transform.rotation) as GameObject;
            broken.GetComponent<BrokenWood>().Break(breakPoint);
            Destroy(gameObject);
        }

    }

    public virtual void TakeHit() {
        if (audioLoader != null && audioLoader.GetSound("Hit") != null) {
            AudioManager.Play(audioLoader.GetSound("Hit"), false, 1);
        }
        currentHitPoints--;
        if (currentHitPoints <= 0) {
            Break();
        } else if (textures[currentHitPoints] != null) {
            GetComponent<Renderer>().material.mainTexture = textures[currentHitPoints];
        }
    }

    public virtual void TakeHit(float delay) {
        Invoke("TakeHit", delay);
    }

    // Use this for initialization
    public void Start() {
        currentHitPoints = maxHitPoints;
        if (currentHitPoints > 0) {
            if (textures[currentHitPoints] != null) {
                GetComponent<Renderer>().material.mainTexture = textures[currentHitPoints];
            }
        }
        audioLoader = GetComponent<AudioLoader>();
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// Metodo que se llama desde el inspector cuando se modifica algun valor
    /// Se utiliza para mantener el mismo tamaño entre los puntos, sus velocidades, y sus delays
    /// </summary>
    void OnValidate() {
        if (maxHitPoints > 0) {
            if (textures.Length != maxHitPoints + 1) {
                textures = copyAndResize(textures, maxHitPoints + 1);
            }
        }

    }

    private Texture[] copyAndResize(Texture[] array, int size) {
        Texture[] temp = new Texture[size];
        Array.Copy(array, temp, Math.Min(array.Length, size));
        array = temp;
        return array;

    }

    public int GetCurrentHitPoints() {
        return currentHitPoints;
    }

    public void SetBreakPoint(Vector3 point)
    {
        breakPoint = point;
    }
}
