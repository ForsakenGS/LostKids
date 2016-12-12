using UnityEngine;
using System.Collections;

public class PushAbility : CharacterAbility {
    public float pushDistance = 2.0f;
    public float height = 0.5f;

    private Transform targetTransform;
    private GameObject targetGameObject;
    private Vector3 pushNormal;
    private RaycastHit targetHitInfo;

    CharacterJoint joint;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        ready = false;
        height = GetComponent<Renderer>().bounds.size.y / 2;
        abilityName = AbilityName.Push;
    }

    /// <summary>
    /// Inicia la ejecución de la habilidad de empujar
    /// </summary>
    /// <returns><c>true</c>, si se pudo iniciar la ejecución, <c>false</c> si no fue posible.</returns>
    public override bool ActivateAbility() {
        if (!active) {
            // Consumo de energía inicial
            AddEnergy(-initialConsumption);
            //Ray detectRay = new Ray(transform.position + Vector3.up * height, transform.forward);
            //Ray detectRay = new Ray(this.transform.position + Vector3.up * height, this.transform.forward * pushDistance);
            // helper to visualise the ground check ray in the scene view
//#if UNITY_EDITOR
            //Debug.DrawRay(detectRay.origin, transform.forward, Color.green, 1);
//#endif
            // Detecta el objeto situado delante del personaje
            //RaycastHit hitInfo;
            //if (Physics.Raycast(detectRay, out hitInfo, pushDistance)) {
                //if (hitInfo.collider.tag.Equals("Pushable")) {
                if (ready) {
                    // El objeto se puede empujar
                    active = true;
                    CallEventActivateAbility();
                    //Se obtiene la normal de la direccion por donde se agarra el objeto
                    pushNormal = targetHitInfo.normal;
                    targetTransform = targetHitInfo.collider.transform;
                    //Se coloca el personaje alineado con el objeto y se rota para que mire a el
                    //Posicion
                    Vector3 newPosition = targetHitInfo.collider.transform.position + (targetHitInfo.collider.bounds.size.z / 2 + GetComponent<CapsuleCollider>().radius) * targetHitInfo.normal;
                    newPosition.y = transform.position.y;
                    newPosition -= 0.2f * transform.forward;
                    this.transform.position = newPosition;
                    //Rotacion
                    Vector3 lookPosition = targetHitInfo.collider.transform.position;
                    lookPosition.y = transform.position.y;
                    this.transform.LookAt(lookPosition);
                    //Se crea un joint fisico para enlazar los objetos
                    GrabObject(targetHitInfo.collider.gameObject, transform.InverseTransformPoint(transform.position + Vector3.up * height), targetTransform.InverseTransformPoint(targetHitInfo.point));
                }
            //}
            if (!active) {
                // Desactiva la habilidad en el CharacterStatus
                characterStatus.EndAbility(this);
            }
        }

        return active;
    }

    /// <summary>
    /// Finaliza la ejecución de la habilidad de empujar
    /// </summary>
    /// <returns><c>true</c>, si se pudo parar la ejecución, <c>false</c> si no fue posible.</returns>
    public override bool DeactivateAbility(bool force) {
        if (active) {
            active = false;
            ReleaseObject();
            characterStatus.EndAbility(this);
            pushNormal = Vector3.zero;
            CallEventDeactivateAbility();
        }

        return !active;
    }

    /// <summary>
    /// Devuelve la normal de agarre del objeto
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPushNormal() {
        return pushNormal;
    }

    void GrabObject(GameObject go, Vector3 origin, Vector3 target) {
        targetGameObject = go;
        targetGameObject.transform.position += Vector3.up * 0.1f;
        if(go.name.Contains("GiantRock"))
        {
            targetGameObject.transform.position += Vector3.up * 0.1f;
        }
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
        joint.breakForce = float.MaxValue;
        joint.connectedBody = targetRigidBody;

        //joint.connectedAnchor = target;

        targetGameObject.GetComponent<PushableObject>().Grab(this);

    }

    void ReleaseObject() {
        if (joint != null) {
            Destroy(joint);
            if (targetGameObject != null) {
                targetGameObject.GetComponent<PushableObject>().Release();
            }
        }
        targetGameObject = null;
    }

    public override bool SetReady(bool r, GameObject go = null, RaycastHit hitInfo = default(RaycastHit)) {
        if ((r) && (hitInfo.collider.tag.Equals("Pushable"))) {
            // La habilidad está lista para ser usada
            ready = true;
            targetHitInfo = hitInfo;
        } else {
            ready = false;
        }

        return ready;
    }
}
