using UnityEngine;
/// <summary>
/// Implementa la habilidad de telekinesis. Al heredar de ‘CharacterAbility’ debe implementar los métodos EndExecution(), con el
/// que simplemente pone la variable ejecución a falso, y StartExecution(), que ejecuta el salto con una llamada al método Jump 
/// del ‘CharacterStatus’ del personaje en cuestión
/// </summary>
public class TelekinesisAbility : CharacterAbility {
    // Objeto activable mediante la habilidad
    private UsableObject usableObj = null;
    private bool isUsing;
    // Sonido de habilidad
    private AudioSource telekinesisSound;
    // La animación terminó
    private bool animationEnded;

    // Use this for initialization
    void Start() {
        AbilityInitialization();
        telekinesisSound = audioLoader.GetSound("Telekinesis");
        abilityName = AbilityName.Telekinesis;
        animationEnded = false;
        isUsing = false;
    }

    /// <summary>
    /// Ejecuta la habilidad, dando por hecho que existe suficiente energía para ello
    /// </summary>
    /// <returns><c>true</c> si se ejecutó la habilidad, o <c>false</c> si no se ha podido ejecutar</returns>
    public override bool ActivateAbility() {
        bool started = false;
        if (!active) {
            // Comienza la ejecución de la habilidad
            active = true;
            started = true;
            AudioManager.Play(telekinesisSound, true, 1);
            CallEventActivateAbility();
            // Comprueba si hay un objeto que se pueda ejecutar
            if (usableObj != null) {
                usableObj.Use();
                // Si el objeto no permanece en uso, se elimina referencia a él
                if (!usableObj.onUse) {
                    //usableObj = null;
                } else {
                    isUsing = true;
                    // Se modifica el componente Animator de Ki
                    CharacterAnimationController.SetAnimatorPropUsing(CharacterName.Ki, true);
                }
            }
        }
        // Realiza el consumo de energía aunque no haya activado ningún objeto
        AddEnergy(-initialConsumption);

        return started;
    }

    /// <summary>
    /// Cambia el estado de la habilidad para que no esté en ejecución
    /// </summary>
    /// <returns><c>true</c> si se modificó el estado de la habilidad, o <c>false</c> si la habilidad ya no estaba en ejecución</returns>
    public override bool DeactivateAbility(bool force) {
        bool changed = active;
        if (active) {
            // Comprueba si la animación terminó o si se fuerza el final
            if ((animationEnded) || (force)) {
                active = false;
                // Deja de usar el objeto, si procede
                if (isUsing) {
                    usableObj.CancelUse();
                }
                // Desactiva resto parámetros
                AudioManager.Stop(telekinesisSound);
                animationEnded = false;
                //usableObj = null;
                isUsing = false;
                // Se modifica el componente Animator de Ki
                CharacterAnimationController.SetAnimatorPropUsing(CharacterName.Ki, false);
                CallEventDeactivateAbility();
            } else {
                changed = false;
            }
        }

        return changed;
    }

    public override bool SetReady(bool r, GameObject go = null, RaycastHit hitInfo = default(RaycastHit)) {
        ready = r;
        if (r) {
            usableObj = go.GetComponent<UsableObject>();
        }

        return ready;
    }

    /// <summary>
    /// Asigna el objeto de tipo 'Usable' que se puede activar con la habilidad en ese momento
    /// </summary>
    /// <param name="obj"></param>
    public void SetUsableObject(UsableObject obj) {
        usableObj = obj;
    }

    public void EndTelekinesisAnimationPoint() {
        // Indica que la animación terminó
        animationEnded = true;
        // Comprueba si se está utilizando algún objeto
        //if (usableObj == null) {
        if (!isUsing) {
            // Se da por finalizada la habilidad
            GetComponent<AbilityController>().DeactivateActiveAbility(false);
        }
    }
}