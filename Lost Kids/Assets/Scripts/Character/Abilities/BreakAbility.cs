using UnityEngine;
using System.Collections;

public class BreakAbility : CharacterAbility {
	public float breakDistance = 2.0f;
	public float altura = -0.5f;

	/// <summary>
	/// Finaliza la ejecución de la habilidad de romper
	/// </summary>
	/// <returns><c>true</c>, si se pudo parar la ejecución, <c>false</c> si no fue posible.</returns>
	public override bool EndExecution () {
		bool res = (execution && (executionTimeLeft <= 0));
		if (res) {
			execution = false;
		}

		return res;
	}

	/// <summary>
	/// Inicia la ejecución de la habilidad de romper
	/// </summary>
	/// <returns><c>true</c>, si se pudo iniciar la ejecución, <c>false</c> si no fue posible.</returns>
	public override bool StartExecution () {
		bool started = !execution;
		if (!execution) {
			execution = true;
			Ray detectRay = new Ray(this.transform.position + Vector3.up*altura, this.transform.forward * breakDistance);
			// helper to visualise the ground check ray in the scene view
			#if UNITY_EDITOR
				Debug.DrawRay(detectRay.origin, detectRay.direction, Color.green, 1);
			#endif
			// Detecta el objeto situado delante del personaje
			RaycastHit hitInfo;
			Debug.Log("rayo");
			if (Physics.Raycast(detectRay, out hitInfo)) {
				Debug.Log("toca");
				// Si el objeto se puede romper, le ordena romperse
				if (hitInfo.collider.tag.Equals("Breakable")) {
					Debug.Log("Rompe");
					hitInfo.collider.GetComponent<BreakableObject>().TakeHit();
				}
			}
		}

		return started;
	}
}
