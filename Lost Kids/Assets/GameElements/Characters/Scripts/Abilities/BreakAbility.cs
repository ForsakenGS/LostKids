using UnityEngine;


public class BreakAbility : CharacterAbility {
    /// <summary>
    /// Distancia máxima a la que detecta el objeto a romper
    /// </summary>
	public float breakDistance = 2.0f;
    /// <summary>
    /// Altura sobre la posición del personaje desde la que se lanza el rayo
    /// </summary>
	public float height = 1f;

    private BreakableObject objectToBreak;
    private bool breakPoint;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        objectToBreak = null;
        breakPoint = false;
        abilityName = AbilityName.Break;
    }

    /// <summary>
    /// Finaliza la ejecución de la habilidad de romper
    /// </summary>
    /// <returns><c>true</c> si se pudo parar la ejecución, <c>false</c> si no fue posible.</returns>
    public override bool DeactivateAbility() {
        bool res = (active && breakPoint);
        if (res) {
            // Termina ejecución y bloquea al jugador para evitar que se mueva durante la animación
            active = false;
            objectToBreak = null;
            breakPoint = false;
            CallEventDeactivateAbility();
        }

        return res;
    }

    /// <summary>
    /// Indica que la animación de romper ha alcanzado el punto en que debe romperse el objeto determinado
    /// </summary>
    public void BreakAnimationPoint() {
        if (objectToBreak != null) {
            objectToBreak.TakeHit();
        }
        breakPoint = true;
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
            Ray detectRay = new Ray(transform.position + Vector3.up * height, transform.forward);
            // helper to visualise the ground check ray in the scene view
            #if UNITY_EDITOR
            Debug.DrawRay(detectRay.origin, transform.forward, Color.green, 1);
            #endif
            // Detecta el objeto situado delante del personaje
            RaycastHit hitInfo;
            if (Physics.Raycast(detectRay, out hitInfo, breakDistance)) {
                // Si el objeto se puede romper, le da un golpe
                if (hitInfo.collider.tag.Equals("Breakable")) {
                    objectToBreak = hitInfo.collider.GetComponent<BreakableObject>();
                }
            }
        }

        return started;
    }
}
