using UnityEngine;
using System.Collections;

public class LockeableObject : MonoBehaviour, ILockeable {

    /// <summary>
    /// Funcion de bloqueo de los objetos para los mensajes generalmente
    /// </summary>
    public void Lock()
    {
        //Es necesario incluir el metodo dentro dentro de Lock, para poder referenciar de manera generica al script
        
    }

    /// <summary>
    /// Funcion de de desbloqueo de los objetos
    /// </summary>
    public void Unlock()
    {
        //Es necesario incluir el metodo dentro dentro de Unlock, para poder referenciar de manera generica al script

    }
}
