using UnityEngine;


public class BreakAbility : CharacterAbility {
    /// <summary>
    /// Distancia máxima a la que detecta el objeto a romper
    /// </summary>
	public float breakDistance = 2.0f;
    /// <summary>
    /// Altura sobre la posición del personaje desde la que se lanza el rayo
    /// </summary>
	public float height = -0.5f;

    /// <summary>
    /// Finaliza la ejecución de la habilidad de romper
    /// </summary>
    /// <returns><c>true</c> si se pudo parar la ejecución, <c>false</c> si no fue posible.</returns>
    public override bool EndExecution() {
        bool res = (execution && (executionTime <= 0));
        if (res) {
            execution = false;
        }

        return res;
    }

    /// <summary>
    /// Inicia la ejecución de la habilidad de romper
    /// </summary>
    /// <returns><c>true</c>, si se pudo iniciar la ejecución, <c>false</c> si no fue posible.</returns>
    public override bool StartExecution() {
        bool started = !execution;
        if (!execution){
            // Consumo de energía inicial
            AddEnergy(-initialConsumption);
            execution = true;
            Ray detectRay = new Ray(this.transform.position + Vector3.up * height, this.transform.forward * breakDistance);
            // helper to visualise the ground check ray in the scene view
            #if UNITY_EDITOR
            Debug.DrawRay(detectRay.origin, detectRay.direction, Color.green, 1);
            #endif
            // Detecta el objeto situado delante del personaje
            RaycastHit hitInfo;
            if (Physics.Raycast(detectRay, out hitInfo)) {
                // Si el objeto se puede romper, le da un golpe
                if (hitInfo.collider.tag.Equals("Breakable")) {
                    hitInfo.collider.GetComponent<BreakableObject>().TakeHit();
                }
            }
        }

        return started;
    }
}
