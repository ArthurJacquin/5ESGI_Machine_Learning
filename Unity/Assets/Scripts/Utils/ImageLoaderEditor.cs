using UnityEditor;
using UnityEngine;

namespace Utils
{
    [CustomEditor(typeof(ImageLoader))]
    public class ImageLoaderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            ImageLoader loader = (ImageLoader) target;
            if (GUILayout.Button("Import Images",GUILayout.Width(135), GUILayout.Height(30)))
            {
                loader.InitializeImages();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}
