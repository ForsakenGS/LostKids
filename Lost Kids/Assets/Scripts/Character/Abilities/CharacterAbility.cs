using UnityEngine;

/// <summary>
/// Clase abstracta para definir el comportamiento general de cualquier habilidad de personaje. Además de los parámetros generales,
/// declara un par de métodos para activar/desactivar una habilidad, dos métodos abstractos para el comienzo y fin de la habilidad
/// y toda la funcionalidad necesaria para el control de la energía y el tiempo en el método Update.
/// </summary>
public abstract class CharacterAbility : MonoBehaviour {
    /// <summary>
	/// Evento para informar que la energía de la habilidad se ha visto modificada
    /// </summary>
    public static event AbilityController.AbilityChanged ModifiedAbilityEnergyEvent;

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
    /// <summary>
    /// Tiempo de ejecución de la habilidad
    /// </summary>
    public float executionTime = 1;
	/// <summary>
	/// Determina si la ejecución de la habilidad es bloqueante; es decir, no permite el cambio a otra habilidad.
	/// </summary>
	public bool blockingExecution;

    // Parámetros básicos de la habilidad
    public bool active;
    protected bool execution;
    protected float energy;
    protected CharacterStatus characterStatus;
    protected CharacterMovement characterMovement;

    private float initExecutionTime;

    // Use this for references
    void Awake() {
        characterStatus = GetComponent<CharacterStatus>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    // Use this for initialization
    void Start() {
        energy = 0.0f;
        initExecutionTime = executionTime;
    }

    protected void AddEnergy(float energyModif) {
        energy += energyModif;
        if (energy > maxEnergy) {
            energy = maxEnergy;
        }
        if (ModifiedAbilityEnergyEvent != null) {
            ModifiedAbilityEnergyEvent(this);
        }
    }

    /// <summary>
    /// Activa la habilidad
    /// </summary>
    public void ActivateAbility() {
        active = true;
    }

    /// <summary>
    /// Comprueba si existe suficiente energía para comenzar a usar una habilidad
    /// </summary>
    /// <returns><c>true</c> si es posible iniciarla, <c>false</c> si no es posible</returns>
    public bool CanBeStarted() {
        return (energy >= initialConsumption);
    }

    /// <summary>
    /// Desactiva la habilidad si es posible, terminando su ejecución en caso de ser necesario
    /// </summary>
	/// <returns><c>true</c> si se ha podido desactivar, <c>false</c> si no ha sido posible</returns>
    public bool DeactivateAbility() {
		bool res = active;
		if (execution) {
			if (!blockingExecution) {
				characterStatus.EndAbility (this);
				res = EndExecution ();
			} else {
				res = false;
			}
		} 
		if (res) {
			active = false;
		}

		return res;
    }

    public abstract bool EndExecution();

    public float GetAvailableEnergy() {
        return energy;
    }

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

    /// <summary>
    /// Permite conocer si la habilidad está en ejecución o no
    /// </summary>
    /// <returns><c>true</c> si la habilidad está en ejecución, <c>false</c> si no lo está</returns>
    public bool IsExecuting() {
        return execution;
    }

    public abstract bool StartExecution();

    // Update is called once per frame
    void Update() {
        // Control del tiempo de habilidad
        if (execution) {
            // En ejecución
            if (normalConsumption > 0) {
                // Se decrementa la energía
                AddEnergy(-(Time.deltaTime * normalConsumption));
                if (energy <= 0.0) {
                    // La habilidad debe terminar su ejecución
                    GetComponent<AbilityController>().UseAbility();
                }
            } else {
                // Se decrementa el tiempo de ejecución
                if (initExecutionTime > 0) {
                    executionTime -= Time.deltaTime;
                    if (executionTime <= 0.0) {
                        // La habilidad debe terminar su ejecución
                        GetComponent<AbilityController>().UseAbility();
                        // Reinicia contador de tiempo
                        executionTime = initExecutionTime;
                    }
                }
            }
        } else if (energy < maxEnergy) {
            // No en ejecución y el tiempo restante no está completo, luego se va recuperando
            if (timeToRestoreEnergy == 0) {
                AddEnergy(maxEnergy);
            } else {
                AddEnergy((Time.deltaTime / timeToRestoreEnergy) * maxEnergy);
            }
        }
    }
}