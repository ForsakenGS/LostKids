using UnityEngine;
//using UnityEditor;

public class MaterialModificator : MonoBehaviour {
    /// <summary>
    /// Indica el nombre del objeto por el que se relaciona con su textura
    /// </summary>
    public string objectName;

    // Use this for content generation
    void Awake() {
        // Obtiene el mundo al que pertenece el nivel
        string worldName = "world2"; //LevelManager.GetWorldName();
        // Asigna el material correspondiente al mundo
        //Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/pruebas/" + worldName + "_" + objectName + ".mat");
        //GetComponent<Renderer>().material = mat;
    }
}
