using UnityEngine;
using System.Collections;

public class PushAbility : CharacterAbility {
	public float pushDistance = 2.0f;
	public float altura = 0.5f;

	private Transform targetTransform;
    private GameObject targetGameObject;
	private Vector3 pushNormal;

    CharacterJoint joint;


    void Start()
    {
        altura = GetComponent<Renderer>().bounds.size.y / 2;
    }
    /// <summary>
    /// Finaliza la ejecución de la habilidad de empujar
    /// </summary>
    /// <returns><c>true</c>, si se pudo parar la ejecución, <c>false</c> si no fue posible.</returns>
    public override bool EndExecution () {
		if (execution) {
			execution = false;
            //targetTransform.parent = null; //Vesrion antigua
            ReleaseObject();
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
            AddEnergy(-initialConsumption);
            
            Ray detectRay = new Ray(this.transform.position + Vector3.up*altura, this.transform.forward * pushDistance);
			// helper to visualise the ground check ray in the scene view
			#if UNITY_EDITOR
			Debug.DrawRay(detectRay.origin, detectRay.direction, Color.green, pushDistance);
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
					//Se coloca el personaje alineado con el objeto y se rota para que mire a el
                    //Posicion
                    Vector3 newPosition= hitInfo.collider.transform.position + (hitInfo.collider.bounds.size.z/2 +GetComponent<CapsuleCollider>().radius) * hitInfo.normal;
                    newPosition.y = transform.position.y;
                    this.transform.position = newPosition;
                    //Rotacion
                    Vector3 lookPosition = hitInfo.collider.transform.position;
                    lookPosition.y = transform.position.y;
                    this.transform.LookAt(lookPosition);


                    //Se crea un joint fisico para enlazar los objetos
                    GrabObject(hitInfo.collider.gameObject,transform.InverseTransformPoint(detectRay.origin),targetTransform.InverseTransformPoint(hitInfo.point));

                } else {
					// Desactiva la habilidad en el CharacterStatus
					characterStatus.EndAbility(this);
				}
			}
		}

		return execution;
	}

    public void GrabObject(GameObject go,Vector3 origin, Vector3 target)
    {
        targetGameObject = go;

        joint = gameObject.AddComponent<CharacterJoint>();

        Rigidbody targetRigidBody = targetGameObject.GetComponent<Rigidbody>();

        joint.autoConfigureConnectedAnchor = true;
        //Al final todo esto ha sio pa mierda porque ha funcionado el autoconfigure
        //No seran necesarios los parametros de la funcion, pero hasta que se confirme que es estable se mantiene

        //origin= this.transform.position + Vector3.up * altura;
        //origin.z += GetComponent<CapsuleCollider>().radius;
        //origin.y = +0.3f;
        //target.y=0.35f;
        //joint.anchor = origin; //Probablemente no sea necesario cuando se improten bien los modelos

        joint.connectedBody = targetRigidBody;

        //joint.connectedAnchor = target;

        targetGameObject.GetComponent<PushableObject>().Grab(this);

        Debug.Log("Punto origen: " + origin + " Punto objetivo: " + target);

    }

    public void ReleaseObject()
    {
        Destroy(joint);
        if (targetGameObject != null)
        {
            targetGameObject.GetComponent<PushableObject>().Release();
            EndExecution();
            characterStatus.EndAbility(this);
        }

        targetGameObject = null;
    }
}
