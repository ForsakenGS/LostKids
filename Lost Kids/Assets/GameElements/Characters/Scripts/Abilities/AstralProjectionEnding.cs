using UnityEngine;
using System.Collections;

public class AstralProjectionEnding : MonoBehaviour {
    public AstralProjectionAbility ability;

    /// <summary>
    /// Aviso del Animator para indicar que la animación de proyección astral se ha terminado
    /// </summary>
    public void EndProjectionAnimationPoint() {
        ability.EndProjectionAnimationPoint();
    }
}
