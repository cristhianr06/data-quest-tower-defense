using UnityEditor;
using UnityEngine;

public class RenameSelectedObjects : EditorWindow
{
    //Iep! Soy el ChardiTronic, sientete libre de cambiar lo que necesites.
    //Este script permite cambiar el nombre de los objetos selccionados
    // y les pone una terminación numérica _00, _01 ...etc
    // Si no te mola, lo puedes cambiar en la linea 47

    private string baseName = "Objeto"; // Nombre por defecto

    // Hacer ventana para escribir nuevo nombre del GameObject
    [MenuItem("Tools/Renombrar Objetos")]

    static void ShowWindow()
    {
        GetWindow<RenameSelectedObjects>("Renombrar Objetos");
    }

    void OnGUI()
    {
        GUILayout.Label("Renombrar Objetos Seleccionados", EditorStyles.boldLabel);

        baseName = EditorGUILayout.TextField("Nuevo Nombre", baseName); // Campo de texto para poder escribir el nuevo nombre del gameObject

        if (GUILayout.Button("Renombrar"))  //Boton que llama la funcion de renombrar
        {
            RenameObjects();
        }
    }

    // Para renombrar los objetos
    void RenameObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No hay ningún GameObject seleccionado.");
            return;
        }

        // Renombrar cada objeto con el nuevo nombre + un numero
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            selectedObjects[i].name = baseName + "_" + i.ToString("00");
        }

        Debug.Log("GameObjects seleccionados renombrados con el prefijo: " + baseName);
    }
}