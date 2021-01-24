using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pooler))]
public class PoolerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Pooler pooler = (Pooler) target;
        if (GUILayout.Button("Generate pool"))
        {
            pooler.InitializePool();
        }
    }
}
