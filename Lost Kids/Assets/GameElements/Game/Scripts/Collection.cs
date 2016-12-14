using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Collections { Kodama, Kappa, Tanuki }
public enum CollectionPieces {Image,Description,Text1,Text2,Text3}

/// <summary>
/// Clase que representa una coleccion concreta sobre alguno de os Yokai del juego
/// </summary>
[System.Serializable]
public class Collection {

    [SerializeField]
    public Collections collection;
    [SerializeField]
    public List<CollectionPieces> collectedPieces=new List<CollectionPieces>();

    /// <summary>
    /// Crea una nueva coleccion del tipo indicado
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Collection(Collections name)
    {
        collection = name;
        collectedPieces = new List<CollectionPieces>();
    }
    /// <summary>
    /// Agrega una pieza a la coleccion
    /// </summary>
    /// <param name="piece"></param>
    public void AddPiece(CollectionPieces piece)
    {
        if(!collectedPieces.Contains(piece))
        {
            collectedPieces.Add(piece);
        }
    }

    /// <summary>
    /// Elimina una pieza de la coleccion
    /// </summary>
    /// <param name="piece"></param>
    public void RemovePiece(CollectionPieces piece)
    {
        if (collectedPieces.Contains(piece))
        {
            collectedPieces.Remove(piece);
        }
    }

    /// <summary>
    /// Devuelve el numero total de colecciones
    /// </summary>
    /// <returns></returns>
    public static int GetCollectionsCount()
    {
        return Enum.GetNames(typeof(Collections)).Length;
    }

    /// <summary>
    /// Devueve si una parte de la coleccion ya se ha conseguido
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public bool CollectedPiece(CollectionPieces piece)
    {
        return collectedPieces.Contains(piece);
    }



}
