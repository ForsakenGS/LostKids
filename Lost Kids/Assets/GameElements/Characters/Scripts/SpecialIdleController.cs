using UnityEngine;
using System.Collections;

public class SpecialIdleController : MonoBehaviour {
    public int specialIdleCount;
    public float minTimeToShow;
    public float minLoopTime;

    private CharacterStatus status;
    private float idleTime;
    private bool onAnimation;

    // Referencias
    void Awake() {
        status = GetComponent<CharacterStatus>();
    }

    void ActiveCharacterChanged(GameObject character) {
        if (character.Equals(gameObject)) {
            // Este personaje es el activo, luego se termina el SpecialIdle
            CharacterAnimationController.SetAnimatorTrigger(status.characterName, CharacterAnimationController.IDLE);
        }
    }

    // Se desactiva el controlador
    void OnDisable() {
        CharacterManager.ActiveCharacterChangedEvent -= ActiveCharacterChanged;
    }

    // Se activa el controlador
    void OnEnable() {
        idleTime = 0;
        onAnimation = false;
        CharacterManager.ActiveCharacterChangedEvent += ActiveCharacterChanged;
    }

    public void SpecialIdleFinished() {
        // Desbloquea al personaje y se deshabilita el controlador
        status.UnlockByAnimation();
        onAnimation = false;
        enabled = false;
    }

    void StopSpecialIdle() {
        if ((enabled) && (onAnimation)) {
            CharacterAnimationController.SetAnimatorTrigger(status.characterName, CharacterAnimationController.IDLE);
        }
    }

    // Update is called once per frame
    void Update() {
        if ((!CharacterManager.GetActiveCharacter().Equals(gameObject)) && (!onAnimation)) {
            // Comprueba que siga en estado Idle
            if (status.CurrentStateIs(CharacterStatus.State.Idle)) {
                // Tiempo en estado Idle
                idleTime += Time.deltaTime;
                if (idleTime > minTimeToShow) {
                    // Decide si mostrar animación especial de Idle
                    if (Random.Range(0.0f, 1.0f) > 0.95f) {
                        // Decide qué animación mostrar
                        CharacterAnimationController.SetAnimatorPropIdleNr(status.characterName, Random.Range(1, specialIdleCount + 1));
                        CharacterAnimationController.SetAnimatorTrigger(status.characterName, CharacterAnimationController.SPECIAL_IDLE);
                        onAnimation = true;
                        // Cuándo parar la animación
                        Invoke("StopSpecialIdle", minLoopTime + Random.Range(0.0f, 2.0f));
                    }
                }
            } else {
                // No está en estado Idle, luego se desactiva
                enabled = false;
            }
        }
    }
}
