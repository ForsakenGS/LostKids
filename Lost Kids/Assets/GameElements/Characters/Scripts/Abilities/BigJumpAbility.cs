﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Implementa la habilidad del salto doble. Al heredar de ‘CharacterAbility’ debe implementar los métodos EndExecution(), con el
/// que simplemente pone la variable ejecución a falso, y StartExecution(), que ejecuta el salto con una llamada al método Jump 
/// del ‘CharacterStatus’ del personaje en cuestión
/// </summary>
public class BigJumpAbility : CharacterAbility {
    /// <summary>
    /// Indica la modificación sobre el salto original que se desea implementar
    /// </summary>
    public float jumpImpulseModifier = 2.0f;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        abilityName = AbilityName.BigJump;
    }

    /// <summary>
    /// Cambia el estado de la habilidad para que no esté en ejecución
    /// </summary>
    /// <returns><c>true</c> si se modificó el estado de la habilidad, o <c>false</c> si la habilidad ya no estaba en ejecución</returns>
    public override bool EndExecution () {
        bool changed = execution;
        execution = false;

		return changed;
	}

    /// <summary>
    /// Ejecuta el salto si no está en ejecución, dando por hecho que existe suficiente energía para ello
    /// </summary>
    /// <returns><c>true</c> si se ejecutó el salto, o <c>false</c> si la habilidad ya estaba en ejecución</returns>
    public override bool StartExecution () {
		bool started = false;
		if (!execution) {
			execution = true;
            started = true;
            AddEnergy(-initialConsumption);
			characterMovement.Jump(jumpImpulseModifier * characterStatus.maxJumpImpulse, true);
		}

		return started;
	}
}