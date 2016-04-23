﻿using UnityEngine;
/// <summary>
/// Implementa la habilidad de telekinesis. Al heredar de ‘CharacterAbility’ debe implementar los métodos EndExecution(), con el
/// que simplemente pone la variable ejecución a falso, y StartExecution(), que ejecuta el salto con una llamada al método Jump 
/// del ‘CharacterStatus’ del personaje en cuestión
/// </summary>
public class TelekinesisAbility : CharacterAbility {
    // Objeto activable mediante la habilidad
    private UsableObject usableObj = null;
    // Sonido de habilidad
    private AudioSource telekinesisSound;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        telekinesisSound = audioLoader.GetSound("Telekinesis");
    }

    /// <summary>
    /// Cambia el estado de la habilidad para que no esté en ejecución
    /// </summary>
    /// <returns><c>true</c> si se modificó el estado de la habilidad, o <c>false</c> si la habilidad ya no estaba en ejecución</returns>
    public override bool EndExecution() {
        bool changed = execution;
        if (execution) {
            if ((fixedExecutionTime) && (executionTime <= 0)) {
                // Tiempo de ejecución fijado y terminado
                execution = false;
                fixedExecutionTime = false;
            } else if (!fixedExecutionTime) {
                // No tiene tiempo de ejecución fijado
                execution = false;
                // Deja de usar el objeto, si procede
                if (usableObj != null) {
                    usableObj.CancelUse();
                }
            }
            // Desactiva sonido
            AudioManager.Stop(telekinesisSound);
        }

        return changed;
    }

    /// <summary>
    /// Ejecuta la habilidad, dando por hecho que existe suficiente energía para ello
    /// </summary>
    /// <returns><c>true</c> si se ejecutó la habilidad, o <c>false</c> si no se ha podido ejecutar</returns>
    public override bool StartExecution() {
        bool started = false;
        if (!execution) {
            // Comienza la ejecución de la habilidad
            execution = true;
            started = true;
            AudioManager.Play(telekinesisSound, true, 1);
            // Comprueba si hay un objeto que se pueda ejecutar
            if (usableObj != null) {
                usableObj.Use();
                // Si el objeto no permanece en uso, se fija el tiempo de ejecución 
                if (!usableObj.onUse) {
                    fixedExecutionTime = true;
                }
            } else {
                // Fija tiempo de ejecución para parar la habilidad
                fixedExecutionTime = true;
            }
        }
        // Realiza el consumo de energía aunque no haya activado ningún objeto
        AddEnergy(-initialConsumption);

        return started;
    }

    /// <summary>
    /// Asigna el objeto de tipo 'Usable' que se puede activar con la habilidad en ese momento
    /// </summary>
    /// <param name="obj"></param>
    public void SetUsableObject(UsableObject obj) {
        usableObj = obj;
    }
}