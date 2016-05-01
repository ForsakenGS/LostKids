using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
	public int width;
	public int height;
	public int rowNumber;
	public int columNumber;

	public GameObject[] brick;
	private GameObject brickSelected;

		void Start() {
			//for (int i = 0; i < rowNumber; i++) {
				for (int j = 0; j < columNumber; j++) {
				float x = transform.position.x + j * width;
				float y = transform.position.y;
				float z = transform.position.z;
			Instantiate(brick[Random.Range(0,brick.Length)], new Vector3(x, y, z), Quaternion.identity);
			//}
		}
	}
}