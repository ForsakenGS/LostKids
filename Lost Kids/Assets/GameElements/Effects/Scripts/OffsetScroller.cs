using UnityEngine;
using System.Collections;

public class OffsetScroller : MonoBehaviour {


    public float scrollSpeed;
    private Vector2 savedOffset;

    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        savedOffset = renderer.material.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        float x = Mathf.Repeat(Time.time * scrollSpeed, 1);
        Vector2 offset = new Vector2(x, savedOffset.y);
        renderer.material.SetTextureOffset("_MainTex", offset);
    }

    void OnDisable()
    {
        if(renderer) {
            renderer.material.SetTextureOffset("_MainTex", savedOffset);
        }
    }

}
