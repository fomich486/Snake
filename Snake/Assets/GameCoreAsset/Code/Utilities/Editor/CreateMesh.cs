using UnityEngine;
using UnityEditor;
 
class CreateMesh
{   
    [MenuItem("Assets/Create Procedural Mesh")]
    static void Create()
    {
        string filePath =
            EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", "Procedural Mesh", "asset", "");

        if (filePath == "")
            return; 
         
        Mesh _myMesh = Mesh.Instantiate(Selection.gameObjects[0].GetComponent<MeshFilter>().sharedMesh) as Mesh;  //make a deep copy 

        if (_myMesh == null)
            Debug.LogWarning("There is no mesh...");

        AssetDatabase.CreateAsset(_myMesh, filePath);
    } 
}