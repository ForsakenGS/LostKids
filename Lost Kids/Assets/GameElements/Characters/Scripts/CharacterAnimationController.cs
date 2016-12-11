using UnityEngine;
using System.Collections;

public class CharacterAnimationController {
    // Referencias a hash de triggers
    public static readonly int ASTRAL_PROJECTION = Animator.StringToHash("AstralProjection");
    public static readonly int BIG_JUMP = Animator.StringToHash("BigJump");
    public static readonly int BREAK = Animator.StringToHash("Break");
    public static readonly int DEAD = Animator.StringToHash("Dead");
    public static readonly int FALL = Animator.StringToHash("Fall");
    public static readonly int IDLE = Animator.StringToHash("Idle");
    public static readonly int JUMP = Animator.StringToHash("Jump");
    public static readonly int LAND = Animator.StringToHash("Land");
    public static readonly int NEGATION = Animator.StringToHash("Negation");
    public static readonly int PUSH = Animator.StringToHash("Push");
    public static readonly int SACRIFICE = Animator.StringToHash("Sacrifice");
    public static readonly int SCARED = Animator.StringToHash("Scared");
    public static readonly int SPECIAL_IDLE = Animator.StringToHash("SpecialIdle");
    public static readonly int SPRINT = Animator.StringToHash("Sprint");
    public static readonly int TELEKINESIS = Animator.StringToHash("Telekinesis");
    public static readonly int USE = Animator.StringToHash("Use");
    public static readonly int VICTORY = Animator.StringToHash("Victory");
    public static readonly int WALK = Animator.StringToHash("Walk");
    // Referencias a hash de propiedades
    public static readonly int PROP_IDLE_NR = Animator.StringToHash("idleNr");
    public static readonly int PROP_IN_AIR = Animator.StringToHash("inAir");
    public static readonly int PROP_IS_PUSHING = Animator.StringToHash("isPushing");
    public static readonly int PROP_SPEED = Animator.StringToHash("speed");
    public static readonly int PROP_USING = Animator.StringToHash("using");
    public static readonly int PROP_SLOW = Animator.StringToHash("slow");

    // Referencias a componentes Animator
    private static Animator aoiAnimator;
    private static Animator akaiAnimator;
    private static Animator kiAnimator;

    /// <summary>
    /// Almacena referencia al componente Animator del personaje especificado
    /// </summary>
    /// <param name="characterName">Nombre del personaje</param>
    /// <param name="animator">Componente Animator</param>
    public static void SetAnimatorReference(CharacterName characterName, Animator animator) {
        switch (characterName) {
            case CharacterName.Aoi:
                aoiAnimator = animator;
                break;
            case CharacterName.Akai:
                akaiAnimator = animator;
                break;
            case CharacterName.Ki:
                kiAnimator = animator;
                break;
        }
    }

    public static void SetAnimatorPropIdleNr(CharacterName characterName, int value) {
        switch (characterName) {
            case CharacterName.Aoi:
                aoiAnimator.SetInteger(PROP_IDLE_NR, value);
                break;
            case CharacterName.Akai:
                akaiAnimator.SetInteger(PROP_IDLE_NR, value);
                break;
            case CharacterName.Ki:
                kiAnimator.SetInteger(PROP_IDLE_NR, value);
                break;
        }
    }

    public static void SetAnimatorPropInAir(bool value) {
        kiAnimator.SetBool(PROP_IN_AIR, value);
    }

    public static void SetAnimatorPropIsPushing(bool value) {
        akaiAnimator.SetBool(PROP_IS_PUSHING, value);
    }

    public static void SetAnimatorPropSpeed(CharacterName characterName, float value) {
        switch (characterName) {
            case CharacterName.Aoi:
                aoiAnimator.SetFloat(PROP_SPEED, value);
                break;
            case CharacterName.Akai:
                akaiAnimator.SetFloat(PROP_SPEED, value);
                break;
            case CharacterName.Ki:
                kiAnimator.SetFloat(PROP_SPEED, value);
                break;
        }
    }

    public static void SetAnimatorPropUsing(CharacterName characterName, bool value) {
        switch (characterName) {
            case CharacterName.Aoi:
                aoiAnimator.SetBool(PROP_USING, value);
                break;
            case CharacterName.Akai:
                akaiAnimator.SetBool(PROP_USING, value);
                break;
            case CharacterName.Ki:
                kiAnimator.SetBool(PROP_USING, value);
                break;
        }
    }

    public static void SetAnimatorPropSlow(CharacterName characterName, bool value) {
        switch (characterName) {
            case CharacterName.Aoi:
                aoiAnimator.SetBool(PROP_SLOW, value);
                break;
            case CharacterName.Akai:
                akaiAnimator.SetBool(PROP_SLOW, value);
                break;
            case CharacterName.Ki:
                kiAnimator.SetBool(PROP_SLOW, value);
                break;
        }
    }

    /// <summary>
    /// Activa el trigger deseado en el componente Animator del personaje, asegurándose que no queda otro trigger activo
    /// </summary>
    /// <param name="characterName">Nombre del personaje</param>
    /// <param name="trigger">Trigger a activar</param>
    public static void SetAnimatorTrigger(CharacterName characterName, int trigger) {
        // Selecciona el Animator del personaje y reinicia triggers habilidades
        Animator characterAnimator;
        if (characterName.Equals(CharacterName.Aoi)) {
            characterAnimator = aoiAnimator;
            characterAnimator.ResetTrigger(BIG_JUMP);
            characterAnimator.ResetTrigger(SPRINT);
        } else if (characterName.Equals(CharacterName.Akai)) {
            characterAnimator = akaiAnimator;
            characterAnimator.ResetTrigger(BREAK);
            characterAnimator.ResetTrigger(PUSH);
        } else {
            characterAnimator = kiAnimator;
            characterAnimator.ResetTrigger(TELEKINESIS);
            characterAnimator.ResetTrigger(ASTRAL_PROJECTION);
        }
        // Reinicia cualquier otro posible trigger
        characterAnimator.ResetTrigger(IDLE);
        characterAnimator.ResetTrigger(WALK);
        characterAnimator.ResetTrigger(JUMP);
        characterAnimator.ResetTrigger(FALL);
        characterAnimator.ResetTrigger(LAND);
        characterAnimator.ResetTrigger(DEAD);
        characterAnimator.ResetTrigger(SCARED);
        characterAnimator.ResetTrigger(SACRIFICE);
        characterAnimator.ResetTrigger(USE);
        characterAnimator.ResetTrigger(VICTORY);
        characterAnimator.ResetTrigger(NEGATION);
        characterAnimator.ResetTrigger(SPECIAL_IDLE);
        // Activa el trigger correspondiente
        characterAnimator.SetTrigger(trigger);
    }

    public static void CheckFallAnimation(CharacterName characterName, bool characterIsgrounded) {
        if (characterIsgrounded) {
            // Selecciona el Animator del personaje
            Animator characterAnimator;
            if (characterName.Equals(CharacterName.Aoi)) {
                characterAnimator = aoiAnimator;
            } else if (characterName.Equals(CharacterName.Akai)) {
                characterAnimator = akaiAnimator;
            } else {
                characterAnimator = kiAnimator;
            }
            // Comprueba si está en animación de caída
            if (characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fall")) {
                SetAnimatorTrigger(characterName,LAND);
            }
        }
    }
}