﻿using UnityEngine;

/// <summary>
/// Implementa la habilidad del sprint. Al heredar de ‘CharacterAbility’ debe implementar los métodos EndExecution(), con el
/// que actualiza el valor de la variable ejecución y reestablece la velocidad originaldel personaje, y StartExecution(), que 
/// modifica la velocidad del personaje según speedModifier y decrementa la energía de la habilidad.
/// </summary>
public class AstralProjectionAbility : CharacterAbility {
    /// <summary>
    /// Modificador de velocidad sobre la establecida por defecto
    /// </summary>
    public float speedModifier = 0.8f;
    /// <summary>
    /// Modelo que ocupará el lugar del personaje durante la ejecución de la habilidad
    /// </summary>
    public GameObject characterDuringProjection;

    // Referencia a la posición del personaje
    private GameObject staticCharacter = null;
    // Pared astral sobre la que se ejecuta la habilidad
    private AstralProjectionWall wall = null;
    // Sonido de habilidad
    private AudioSource astralProjectionSound;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        astralProjectionSound = audioLoader.GetSound("AstralProjection");
        abilityName = AbilityName.AstralProjection;
    }

    /// <summary>
    /// Termina la ejecución de la habilidad, reestableciendo la velocidad del personaje
    /// </summary>
    /// <returns><c>true</c>, si la ejecución se terminó realmente, <c>false</c> en otro caso.</returns>
    public override bool DeactivateAbility() {
        bool ended = active;
        if (active) {
            // Se para la ejecución de la habilidad
            active = false;
            AudioManager.Stop(astralProjectionSound);
            CallEventDeactivateAbility();
            // Comprueba si la proyección astral se llevó a cabo
            if (staticCharacter != null) {
                // El personaje vuelve a la posición original con los parámetros originales y se elimina la copia
                characterStatus.astralSpeed = 0.0f;
                transform.position = staticCharacter.transform.localPosition;
                Destroy(staticCharacter);
                staticCharacter = null;
            } else {
                fixedExecutionTime = false;
            }
        }

        return ended;
    }

    /// <summary>
    /// Asigna el objeto de tipo 'AstralProjectionWall' sobre el que se ejecuta la habilidad en ese momento
    /// </summary>
    /// <param name="obj"></param>
    public void SetWall(AstralProjectionWall obj) {
        wall = obj;
    }

    /// <summary>
    /// Comienza la ejecución de la habilidad, duplicando al personaje al otro lado de la pared
    /// </summary>
    /// <returns><c>true</c>, si la habilidad se inició con éxito, <c>false</c> si no fue posible.</returns>
    public override bool ActivateAbility() {
        bool started = !active;
        if (!active) {
            // Comienza la ejecución de la habilidad
            active = true;
            AudioManager.Play(astralProjectionSound, true, 1);
            CallEventActivateAbility();
            // Comprueba si hay una pared sobre la que se pueda ejecutar
            if (wall != null) {
                // Crea un doble del personaje y la proyección astral
                staticCharacter = (GameObject) Instantiate(characterDuringProjection, transform.position, transform.rotation);
                transform.position = wall.GetAstralProjectionPosition();
                // Modifica los parámetros de la proyección astral
                characterStatus.astralSpeed = speedModifier * characterStatus.standingSpeed;
            } else {
                // Fija tiempo de ejecución para parar la habilidad
                fixedExecutionTime = true;
            }
        }
        // Realiza el consumo de energía aunque no haya activado ningún objeto
        AddEnergy(-initialConsumption);

        return started;
    }
}