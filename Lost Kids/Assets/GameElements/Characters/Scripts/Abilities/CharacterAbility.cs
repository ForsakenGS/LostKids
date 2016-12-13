using UnityEngine;

/// <summary>
/// Clase para definir las diferentes habilidades de los personajes
/// </summary>
public enum AbilityName { AstralProjection, BigJump, Break, Push, Sprint, Telekinesis }

/// <summary>
/// Clase abstracta para definir el comportamiento general de cualquier habilidad de personaje. Además de los parámetros generales,
/// declara un par de métodos abstractos para activar/desactivar una habilidad y toda la funcionalidad necesaria para el control 
/// de la energía y el tiempo en el método Update.
/// </summary>
public abstract class CharacterAbility : MonoBehaviour {
    /// <summary>
	/// Evento para informar que la energía de la habilidad se ha visto modificada
    /// </summary>
    public static event AbilityController.AbilityChanged ModifiedAbilityEnergyEvent;
    /// <summary>
	/// Evento para informar que la habilidad ha sido activada
    /// </summary>
    public static event AbilityController.AbilityChanged ActivateAbilityEvent;
    /// <summary>
	/// Evento para informar que la habilidad ha sido desactivada
    /// </summary>
    public static event AbilityController.AbilityChanged DeactivateAbilityEvent;

    /// <summary>
    /// Nombre de la habilidad
    /// </summary>
    public AbilityName abilityName;
    /// <summary>
    /// Energía máxima de la habilidad
    /// </summary>
    public float maxEnergy = 10;
    /// <summary>
    /// Consumo de energía inicial necesario para la ejecución de la habilidad
    /// </summary>
    public float initialConsumption = 5;
    /// <summary>
    /// Multiplicador del consumo de energía durante la ejecución
    /// </summary>
    public float normalConsumption = 1;
    /// <summary>
    /// Tiempo necesario para restaurar por completo la energía, desde 0 a maxEnergy
    /// </summary>
    public float timeToRestoreEnergy = 5;

    // Parámetros básicos de la habilidad
    protected bool active;
    protected bool ready;
    private bool timeStopped = false;
    protected AudioLoader audioLoader;
    protected float energy;
    protected CharacterStatus characterStatus;
    protected CharacterMovement characterMovement;

    // Use this for references
    void Awake() {
        characterStatus = GetComponent<CharacterStatus>();
        characterMovement = GetComponent<CharacterMovement>();
        audioLoader = GetComponent<AudioLoader>();
    }

    /// <summary>
    /// Método para indicar que la animación de la habilidad ha terminado, por lo que la ejecución debe finalizar también
    /// </summary>
    public void AbilityAnimationEnded() {
        // Desbloquea el personaje y finaliza la ejecución de la habilidad
        characterStatus.UnlockByAnimation();
        GetComponent<AbilityController>().DeactivateActiveAbility(false);
    }

    /// <summary>
    /// Método para la inicialización de los parámetros de la habilidad
    /// </summary>
    protected void AbilityInitialization() {
        active = false;
        energy = 0.0f;
    }

    /// <summary>
    /// Método para añadir energía al total de la habilidad
    /// </summary>
    /// <param name="energyModif">Cantidad de energía a incrementar</param>
    protected void AddEnergy(float energyModif) {
        // Comprueba si el tiempo de habilidad está parado
        if (!timeStopped) {
            // Incrementa la energía, sin sobrepasar el límite máximo
            energy += energyModif;
            if (energy > maxEnergy) {
                energy = maxEnergy;
            }
            // Lanza el evento para informar que la energía de la habilidad ha sido modificada
            if (ModifiedAbilityEnergyEvent != null) {
                ModifiedAbilityEnergyEvent(this);
            }
        }
    }

    /// <summary>
    /// Activa la habilidad
    /// </summary>
    public abstract bool ActivateAbility();

    /// <summary>
    /// Método para llamar al evento ActivateAbility
    /// </summary>
    public void CallEventActivateAbility() {
        if (ActivateAbilityEvent != null) {
            ActivateAbilityEvent(this);
        }
    }

    /// <summary>
    /// Método para llamar al evento DeactivateAbility
    /// </summary>
    public void CallEventDeactivateAbility() {
        if (DeactivateAbilityEvent != null) {
            DeactivateAbilityEvent(this);
        }
    }

    /// <summary>
    /// Comprueba si existe suficiente energía para comenzar a usar una habilidad
    /// </summary>
    /// <returns><c>true</c> si es posible iniciarla, <c>false</c> si no es posible</returns>
    public bool CanBeStarted() {
        return ((energy >= initialConsumption) && (ready));
    }

    /// <summary>
    /// Desactiva la ejecución de la habilidad si es posible
    /// </summary>
	/// <returns><c>true</c> si se ha podido desactivar, <c>false</c> si no ha sido posible</returns>
    public abstract bool DeactivateAbility(bool force);

    /// <summary>
    /// Devuelve la cantidad de energía disponible de la habilidad
    /// </summary>
    /// <returns></returns>
    public float GetAvailableEnergy() {
        return energy;
    }

    /// <summary>
    /// Devuelve el máximo de energía que puede contener una habilidad
    /// </summary>
    /// <returns></returns>
    public float GetMaxEnergy() {
        return maxEnergy;
    }

    /// <summary>
    /// Permite conocer si la habilidad está activa o no
    /// </summary>
    /// <returns><c>true</c> si la habilidad está activa, <c>false</c> si no lo está</returns>
    public bool IsActive() {
        return active;
    }

    public void Reset() {
        AbilityInitialization();
    }

    public void ResumeTime() {
        timeStopped = false;
    }

    public abstract bool SetReady(bool r, GameObject go = null, RaycastHit hitInfo = default(RaycastHit));

    public void StopTime() {
        timeStopped = true;
    }

    // Update is called once per frame
    void Update() {
        // Control de la energía de habilidad
        if (active) {
            // Consumo durante ejecución
            if (normalConsumption > 0.0f) {
                // Se decrementa la energía
                AddEnergy(-(Time.deltaTime * normalConsumption));
            }
            // Comprueba si la habilidad debe terminar su ejecución
            if (energy <= 0.0f) {
                GetComponent<AbilityController>().DeactivateActiveAbility(false);
            }
        } else if (energy < maxEnergy) {
            // No en ejecución y la energía restante no está completa, luego se va recuperando
            if (timeToRestoreEnergy == 0.0f) {
                AddEnergy(maxEnergy);
            } else {
                AddEnergy((Time.deltaTime / timeToRestoreEnergy) * maxEnergy);
            }
        }
    }
}