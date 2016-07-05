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
    /// <summary>
    /// Tiempo de ejecución de la habilidad
    /// </summary>
    public float executionTime = 1;
    /// <summary>
    /// Determina si la habilidad posee un tiempo de ejecución fijo y no permite el cambio a otra habilidad.
    /// </summary>
    public bool fixedExecutionTime = false;

    // Parámetros básicos de la habilidad
    protected bool active;
    protected AudioLoader audioLoader;
    //protected bool execution;
    protected float energy;
    protected CharacterStatus characterStatus;
    protected CharacterMovement characterMovement;
    private float initExecutionTime;
    // Referencias a managers
    protected InputManagerTLK inputManager;

    // Use this for references
    void Awake() {
        characterStatus = GetComponent<CharacterStatus>();
        characterMovement = GetComponent<CharacterMovement>();
        audioLoader = GetComponent<AudioLoader>();
        inputManager = GameObject.Find("InputManagerTLK").GetComponent<InputManagerTLK>();
    }

    /// <summary>
    /// Método para la inicialización de los parámetros de la habilidad
    /// </summary>
    protected void AbilityInitialization() {
        active = false;
        energy = 0.0f;
        initExecutionTime = executionTime;
    }

    /// <summary>
    /// Método para añadir energía al total de la habilidad
    /// </summary>
    /// <param name="energyModif">Cantidad de energía a incrementar</param>
    protected void AddEnergy(float energyModif) {
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
        return (energy >= initialConsumption);
    }

    //    {
    //    bool res = active;
    //    if (execution) {
    //        if (!fixedExecutionTime) {
    //            characterStatus.EndAbility(this);
    //            res = EndExecution();
    //        } else {
    //            res = false;
    //        }
    //    }
    //    if (res) {
    //        active = false;
    //    }

    //    return res;
    //}

    /// <summary>
    /// Desactiva la ejecución de la habilidad si es posible
    /// </summary>
	/// <returns><c>true</c> si se ha podido desactivar, <c>false</c> si no ha sido posible</returns>
    public abstract bool DeactivateAbility();

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

    // Update is called once per frame
    void Update() {
        // Control de la energía de habilidad
        if (active) {
            // Consumo durante ejecución
            if (normalConsumption > 0.0f) {
                // Se decrementa la energía
                AddEnergy(-(Time.deltaTime * normalConsumption));
                if (energy <= 0.0) {
                    // La habilidad debe terminar su ejecución
                    GetComponent<AbilityController>().DeactivateActiveAbility();
                }
            }
            // Se decrementa el tiempo de ejecución
            if ((executionTime > 0.0f) && (initExecutionTime > 0.0f)) {
                executionTime -= Time.deltaTime;
                if (executionTime <= 0.0f) {
                    // Si la habilidad es de de tiempo fijo, debe terminar su ejecución
                    if (fixedExecutionTime) {
                        GetComponent<AbilityController>().DeactivateActiveAbility();
                    }
                    // Reinicia contador de tiempo
                    executionTime = initExecutionTime;
                }
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