using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MlDllRun))]
public class MlDllEditor : Editor
{
    private TypeModel modelType;
    private TypeTest testType;
    private bool isClassification;
    private int epoch = 1000;
    private double alpha = 0.1f;
    private string savePath;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        //Variables éditables dans l'inspecteur
        modelType = (TypeModel) EditorGUILayout.EnumPopup("Type de modèle :", modelType);
        testType = (TypeTest)EditorGUILayout.EnumPopup("Type de test :", testType);
        isClassification = EditorGUILayout.Toggle("Classification activée :", isClassification);
        epoch = EditorGUILayout.IntField("Nombre d'itérations :", epoch);
        alpha = EditorGUILayout.DoubleField("Alpha :", alpha);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        MlDllRun dllRun = (MlDllRun) target;
        if (GUILayout.Button("Simulate training", GUILayout.Width(135), GUILayout.Height(30)))
        {
            dllRun.Simulate(testType, isClassification);
        }
        
        if (GUILayout.Button("Training", GUILayout.Width(135), GUILayout.Height(30)))
        {
            dllRun.RunMlDll(testType, isClassification, epoch, alpha);
        }
        EditorGUILayout.EndHorizontal();

        savePath = EditorGUILayout.TextField("Chemin de sauvegarde :", savePath);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save Model", GUILayout.Width(135), GUILayout.Height(30)))
        {
            Model model = new Model();
            SaveSystem.SaveModel(model, savePath);
        }
        
        if (GUILayout.Button("Load Model", GUILayout.Width(135), GUILayout.Height(30)))
        {
            ModelData data = SaveSystem.LoadModel(savePath);
        }
        EditorGUILayout.EndHorizontal();
    }
}
