using UnityEngine;
using System.Collections;
using System;

public class ArraysUtilities  {

    /// <summary>
    /// Metodo auxiliar que reordena de forma aleatoria el contenido de un array
    /// </summary>
    /// <typeparam name="T">Tipo de dato contenido en el array</typeparam>
    /// <param name="arr">array a desordenar</param>
    public static void ShuffleArray<T>(T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i);
            T tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }

    /// <summary>
    /// Metodo auxiliar para redimensionar un array manteniendo su contenido
    /// </summary>
    /// <param name="array">array a dimensionar</param>
    /// <param name="size">nuevo tamaño</param>
    /// <returns>array redimensionado manteniendo los valores anteriores</returns>
    public static  T[] CopyAndResize<T>(T[] array, int size)
    {
        T[] temp = new T[size];
        Array.Copy(array, temp, Math.Min(array.Length, size));
        array = temp;
        return array;

    }
}
