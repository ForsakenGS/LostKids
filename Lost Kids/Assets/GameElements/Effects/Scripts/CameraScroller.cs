using UnityEngine;
using System.Collections;

public class CameraScroller : MonoBehaviour {

    private GameObject camera;

    public float speed = 0.05f;

    [HideInInspector]
    public float scrollSpeed = 0;

    public Vector2 savedOffset;

    private Renderer renderer;

    private float displacement = 0;

    public enum Directions {Positive,Negative}
    public Directions direction;

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
                scrollSpeed = aux * speed;
            }
            else if(initPos.x > finishPos.x){
                scrollSpeed = -aux * speed;
            }

            displacement += Time.deltaTime * scrollSpeed;
            Vector2 offset = Vector3.zero;

            switch (direction) {
                case Directions.Positive:
                    offset = new Vector2(displacement, savedOffset.y);
                    break;
                case Directions.Negative:
                    offset = new Vector2(-displacement, savedOffset.y);
                    break;
            }
            
            
            renderer.material.SetTextureOffset("_MainTex", offset);


            scrollSpeed = 0;
        }
    }
}
