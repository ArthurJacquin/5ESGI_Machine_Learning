using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils;

[CustomEditor(typeof(Visualizers))]
public class VisualizersEditor : Editor
{
   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();

      Visualizers visualizers = (Visualizers) target;
      if (GUILayout.Button("Generate visualizers"))
      {
         visualizers.InitializePool();
      }
   }
}
