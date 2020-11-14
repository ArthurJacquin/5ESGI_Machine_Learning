using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MlDllRun))]
public class MlDllEditor : Editor
{
    private TypeTest type;
    private bool isClassification;
    private int epoch;
    private double alpha;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        //Variables éditables dans l'inspecteur
        type = (TypeTest)EditorGUILayout.EnumPopup("Type de test :", type);
        isClassification = EditorGUILayout.Toggle("Classification activée :", isClassification);
        epoch = EditorGUILayout.IntField("Nombre d'itérations :", epoch);
        alpha = EditorGUILayout.DoubleField("Alpha :", alpha);
        
        MlDllRun dllRun = (MlDllRun) target;
        if (GUILayout.Button("TRAINING"))
        {
            dllRun.RunMlDll(type, isClassification, epoch, alpha);
        }
              
    }
}
