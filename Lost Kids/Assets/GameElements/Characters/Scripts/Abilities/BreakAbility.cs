using UnityEngine;


public class BreakAbility : CharacterAbility {
    /// <summary>
    /// Distancia máxima a la que detecta el objeto a romper
    /// </summary>
	public float breakDistance = 2.0f;
    /// <summary>
    /// Altura sobre la posición del personaje desde la que se lanza el rayo
    /// </summary>
	public float height = -0.5f;
    /// <summary>
    /// Retraso para ejecutar el daño
    /// </summary>
    public float delay = 0.4f;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        abilityName = AbilityName.Break;
    }

    /// <summary>
    /// Finaliza la ejecución de la habilidad de romper
    /// </summary>
    /// <returns><c>true</c> si se pudo parar la ejecución, <c>false</c> si no fue posible.</returns>
    public override bool DeactivateAbility() {
        bool res = (active && (executionTime <= 0));
        if (res) {
            // Termina ejecución y bloque brevemente al jugador
            active = false;
            inputManager.LockTime(0.5f);
            CallEventDeactivateAbility();
        }

        return res;
    }

    /// <summary>
    /// Inicia la ejecución de la habilidad de romper
    /// </summary>
    /// <returns><c>true</c>, si se pudo iniciar la ejecución, <c>false</c> si no fue posible.</returns>
    public override bool ActivateAbility() {
        bool started = !active;
        if (!active){
            // Consumo de energía inicial
            AddEnergy(-initialConsumption);
            active = true;
            CallEventActivateAbility();
            Ray detectRay = new Ray(this.transform.position + Vector3.up * height, this.transform.forward * breakDistance);
            // helper to visualise the ground check ray in the scene view
            #if UNITY_EDITOR
            Debug.DrawRay(detectRay.origin, detectRay.direction, Color.green, 1);
            #endif
            // Detecta el objeto situado delante del personaje
            RaycastHit hitInfo;
            if (Physics.Raycast(detectRay, out hitInfo)) {
                // Si el objeto se puede romper, le da un golpe
                if (hitInfo.collider.tag.Equals("Breakable")) {
                    hitInfo.collider.GetComponent<BreakableObject>().TakeHit(delay);
                }
            }
        }

        return started;
    }
}
