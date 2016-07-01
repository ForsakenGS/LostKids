using UnityEngine;
using System.Collections;

public class CameraScroller : MonoBehaviour {

    private GameObject camera;

    [HideInInspector]
    public float scrollSpeed = 0;

    public Vector2 savedOffset;

    private Renderer renderer;

    private float displacement = 0;

	// Use this for initialization
	void Start () {
        CameraManager cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        camera = cameraManager.CurrentCamera();

        renderer = GetComponent<Renderer>();
        savedOffset = renderer.material.GetTextureOffset("_MainTex");
	}
	
	// Update is called once per frame
	void Update () {



	}

    void OnDisable() {
        renderer.material.SetTextureOffset("_MainTex", savedOffset);
    }

    public void UpdateScrollSpeed(Vector3 initPos, Vector3 finishPos) {

        float aux = Mathf.Abs(finishPos.x - initPos.x);

        if(aux > 0.05f) {

            if(initPos.x < finishPos.x) {
                scrollSpeed = aux * 0.05f;
            }
            else if(initPos.x > finishPos.x){
                scrollSpeed = -aux * 0.05f;
            }

            displacement += Time.deltaTime * scrollSpeed;
            Vector2 offset = new Vector2(displacement, savedOffset.y);
            renderer.material.SetTextureOffset("_MainTex", offset);


            scrollSpeed = 0;
        }
    }
}
