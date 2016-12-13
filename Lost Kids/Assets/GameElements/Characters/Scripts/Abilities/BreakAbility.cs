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
    public override bool DeactivateAbility(bool force) {
        bool res = ((active && breakPoint) || (force));
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
        if (!active) {
            // Consumo de energía inicial
            AddEnergy(-initialConsumption);
            if (ready) {
                active = true;
                started = true;
                CallEventActivateAbility();
                AudioManager.Play(audioLoader.GetSound("Break"),false,1);
            } else {
                started = false;
            }
        }

        return started;
    }

    public override bool SetReady(bool r, GameObject go = null, RaycastHit hitInfo = default(RaycastHit)) {
        if ((r) && (hitInfo.collider.tag.Equals("Breakable"))) {
            // La habilidad está lista para ser usada
            ready = true;
            objectToBreak = hitInfo.collider.GetComponent<BreakableObject>();
            objectToBreak.SetBreakPoint(hitInfo.point);
        } else {
            ready = false;
        }

        return ready;
    }

    public void BreakAnimationEnded() {
        // Se ha terminado la animación, la habilidad debe finalizar
        GetComponent<AbilityController>().DeactivateActiveAbility(false);
    }
}