/// <summary>
/// Implementa la habilidad del sprint. Al heredar de ‘CharacterAbility’ debe implementar los métodos EndExecution(), con el
/// que actualiza el valor de la variable ejecución y reestablece la velocidad originaldel personaje, y StartExecution(), que 
/// modifica la velocidad del personaje según speedModifier y decrementa la energía de la habilidad.
/// </summary>
public class SprintAbility : CharacterAbility {
	/// <summary>
	/// Modificador de velocidad sobre la establecida por defecto
	/// </summary>
	public float speedModifier = 2.0f;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        abilityName = AbilityName.Sprint;
    }

    /// <summary>
    /// Termina la ejecución de la habilidad, reestableciendo la velocidad del personaje
    /// </summary>
    /// <returns><c>true</c>, si la ejecución se terminó realmente, <c>false</c> en otro caso.</returns>
    public override bool EndExecution () {
		bool ended = execution;
		if (execution) {
			// Se para la ejecución de la habilidad
			execution = false;
			characterStatus.standingSpeed /= speedModifier;
		}

		return ended;
	}

	/// <summary>
	/// Comienza la ejecución de la habilidad, aumentando la velocidad del personaje
	/// </summary>
	/// <returns><c>true</c>, si la habilidad se inició con éxito, <c>false</c> si no fue posible.</returns>
	public override bool StartExecution () {
		bool started = false;
		if (!execution) {
			// Comienza la ejecución de la habilidad
			execution = true;
            started = true;
            AddEnergy(-initialConsumption);
			characterStatus.standingSpeed *= speedModifier;
        }

		return started;
	}
}