using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadingFloor : MonoBehaviour {


    private GameObject childFloor;
    Material material;
    Material childMaterial;
    Color originalColor;
    
    HashSet<GameObject> charactersInside;
    Color newColor;
    public float fadeTime=0.5f;
    private bool visible;
	// Use this for initialization
	void Start () {
        childFloor = transform.GetChild(0).gameObject;
        charactersInside = new HashSet<GameObject>();
        material = GetComponent<Renderer>().material;
        childMaterial =childFloor.GetComponent<Renderer>().material;
        originalColor = material.color;
        Color newColor = originalColor;
        newColor.a = 0.1f;
        material.color = newColor;
        childMaterial.color = newColor;

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if(!visible)
            {
                ShowFloor();
            }
            charactersInside.Add(col.gameObject);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            charactersInside.Remove(col.gameObject);
            if(charactersInside.Count<=0)
            {
                HideFloor();
            }
        }
    }

    void ShowFloor()
    {
        visible = true;
        modifyColor();
        iTween.FadeTo(gameObject, 1, fadeTime);
        iTween.FadeTo(childFloor, 1, fadeTime);
    }

    void HideFloor()
    {
        visible = false;
        iTween.FadeTo(gameObject, 0.15f, fadeTime);
        iTween.FadeTo(childFloor, 0.15f, fadeTime);
    }

    void modifyColor()
    {
        newColor = originalColor;
        newColor.a = material.color.a;
        newColor.r += Random.Range(-0.05f, 0.05f);
        newColor.g += Random.Range(-0.05f, 0.05f);
        newColor.b += Random.Range(-0.05f, 0.05f);
        material.color = newColor;
        childMaterial.color = newColor;
    }
}
