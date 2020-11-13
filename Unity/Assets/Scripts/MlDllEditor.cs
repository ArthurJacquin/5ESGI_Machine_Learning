using UnityEditor;
using UnityEngine;

public enum TypeTest
{
    LinearSimple,
    LinearMultiple,
    XOR,
    Cross,
    MultiLinear,
    MultiCross,
    LinearSimple2D,
    NonLinearSimple2D,
    LinearSimple3D,
    LinearTricky3D,
    NonLinearSimple3D
}

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
            Debug.Log("Try to run dll !");
            dllRun.RunMlDll(type, isClassification, epoch, alpha);
        }
              
    }
}
