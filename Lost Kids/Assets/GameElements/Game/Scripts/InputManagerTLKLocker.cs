using UnityEngine;
using System.Collections;

public class InputManagerTLKLocker : MonoBehaviour, ILockeable {

    //Al activarse el script se añade la función Lock
    void OnEnable() {
        MessageManager.LockEvent += Lock;
        CameraManager.LockEvent += Lock;
        CameraManager.UnlockEvent += Unlock;
    }

    //Al desactivarse el script se desuscriben las funciones
    void OnDisable() {
        MessageManager.UnlockEvent -= Unlock;
        MessageManager.LockEvent -= Lock;
        CameraManager.LockEvent -= Unlock;
        CameraManager.UnlockEvent -= Unlock;
    }

    /// <summary>
    /// Funcion de bloqueo de los objetos para los mensajes generalmente
    /// </summary>
    public void Lock() {
        //Es necesario incluir el metodo dentro dentro de Lock, para poder referenciar de manera generica al script
        InputManagerTLK.SetLock(true);
        MessageManager.LockEvent -= Lock;
        MessageManager.UnlockEvent += Unlock;
        CameraManager.LockEvent -= Lock;
        CameraManager.UnlockEvent += Unlock;
    }

    /// <summary>
    /// Funcion de de desbloqueo de los objetos
    /// </summary>
    public void Unlock() {
        //Es necesario incluir el metodo dentro dentro de Unlock, para poder referenciar de manera generica al script
        InputManagerTLK.SetLock(false);
        MessageManager.UnlockEvent -= Unlock;
        MessageManager.LockEvent += Lock;
        CameraManager.UnlockEvent -= Unlock;
        CameraManager.LockEvent += Lock;
    }
}
