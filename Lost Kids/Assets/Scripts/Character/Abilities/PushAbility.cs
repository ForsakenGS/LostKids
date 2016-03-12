using UnityEngine;
using System.Collections;

public class PushAbility : CharacterAbility {
	public float pushDistance = 2.0f;
	public float altura = -0.5f;

	private Transform targetTransform;
	private Vector3 pushNormal;

	/// <summary>
	/// Finaliza la ejecución de la habilidad de empujar
	/// </summary>
	/// <returns><c>true</c>, si se pudo parar la ejecución, <c>false</c> si no fue posible.</returns>
	public override bool EndExecution () {
		if (execution) {
			execution = false;
			targetTransform.parent = null;
			pushNormal = Vector3.zero;
		}

		return !execution;
	}

	/// <summary>
	/// Devuelve la normal de agarre del objeto
	/// </summary>
	/// <returns></returns>
	public Vector3 GetPushNormal()
	{
		return pushNormal;
	}

	/// <summary>
	/// Inicia la ejecución de la habilidad de empujar
	/// </summary>
	/// <returns><c>true</c>, si se pudo iniciar la ejecución, <c>false</c> si no fue posible.</returns>
	public override bool StartExecution () {
		if (!execution) {
            // Consumo de energía inicial
            energy -= initialConsumption;
            Ray detectRay = new Ray(this.transform.position + Vector3.up*altura, this.transform.forward * pushDistance);
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
				if (hitInfo.collider.tag.Equals("Pushable")) {
					Debug.Log("Coge");
					execution = true;
					//Se obtiene la normal de la direccion por donde se agarra el objeto
					pushNormal = hitInfo.normal;
					targetTransform = hitInfo.collider.transform;
					//Se coloca el personaje alineado con el objeto y el objeto se marca como hijo del jugador
					this.transform.position = targetTransform.position + 2 * hitInfo.normal;
					targetTransform.parent = this.transform;
				} else {
					// Desactiva la habilidad en el CharacterStatus
					characterStatus.EndAbility(this);
				}
			}
		}

		return execution;
	}
}
