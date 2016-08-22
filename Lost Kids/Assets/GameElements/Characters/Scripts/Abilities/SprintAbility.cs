﻿using UnityEngine;
/// <summary>
/// Implementa la habilidad del sprint. Al heredar de ‘CharacterAbility’ debe implementar los métodos Endactive(), con el
/// que actualiza el valor de la variable ejecución y reestablece la velocidad originaldel personaje, y StartExecution(), que 
/// modifica la velocidad del personaje según speedModifier y decrementa la energía de la habilidad.
/// </summary>
public class SprintAbility : CharacterAbility {
	/// <summary>
	/// Modificador de velocidad sobre la establecida por defecto
	/// </summary>
	public float speedModifier = 2.0f;

    // Referencias
    private Rigidbody rig;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        abilityName = AbilityName.Sprint;
        rig = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Termina la ejecución de la habilidad, reestableciendo la velocidad del personaje
    /// </summary>
    /// <returns><c>true</c>, si la ejecución se terminó realmente, <c>false</c> en otro caso.</returns>
    public override bool DeactivateAbility () {
		bool ended = active;
		if (active) {
			// Se para la ejecución de la habilidad
			active = false;
			//characterStatus.standingSpeed /= speedModifier;
            CallEventDeactivateAbility();
		}

		return ended;
	}

	/// <summary>
	/// Comienza la ejecución de la habilidad, aumentando la velocidad del personaje
	/// </summary>
	/// <returns><c>true</c>, si la habilidad se inició con éxito, <c>false</c> si no fue posible.</returns>
	public override bool ActivateAbility () {
		bool started = false;
		if (!active) {
			// Comienza la ejecución de la habilidad
			active = true;
            started = true;
            AddEnergy(-initialConsumption);
            //characterStatus.standingSpeed *= speedModifier;
            CallEventActivateAbility();
        }

		return started;
	}

    void FixedUpdate() {
        // Si la habilidad está activa, reproduce la fuerza
        // HAY QUE CAMBIAR porque no debería hacerse la comprobación tantas veces
        if (active) {
            rig.AddForce(speedModifier * transform.forward, ForceMode.Acceleration);
        }
    }
}