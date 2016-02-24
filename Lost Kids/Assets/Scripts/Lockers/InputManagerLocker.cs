using UnityEngine;
using System.Collections;

public class InputManagerLocker : MonoBehaviour, ILockeable {

    private InputManager scInputManager;

    void Start() {
        scInputManager = GetComponent<InputManager>();
    }

    void OnEnable()
    {
        MessageManager.LockUnlockEvent += Lock;
    }

    void OnDisable()
    {
        MessageManager.LockUnlockEvent -= Unlock;
        MessageManager.LockUnlockEvent -= Lock;
    }

    /// <summary>
    /// Funcion de bloqueo de los objetos para los mensajes generalmente
    /// </summary>
    public void Lock()
    {
        //Es necesario incluir el metodo dentro dentro de Lock, para poder referenciar de manera generica al script
        scInputManager.locked = true;
        MessageManager.LockUnlockEvent -= Lock;
        MessageManager.LockUnlockEvent += Unlock;
    }

    /// <summary>
    /// Funcion de de desbloqueo de los objetos
    /// </summary>
    public void Unlock()
    {
        //Es necesario incluir el metodo dentro dentro de Unlock, para poder referenciar de manera generica al script
        scInputManager.locked = false;
        MessageManager.LockUnlockEvent -= Unlock;
        MessageManager.LockUnlockEvent += Lock;
    }
}
