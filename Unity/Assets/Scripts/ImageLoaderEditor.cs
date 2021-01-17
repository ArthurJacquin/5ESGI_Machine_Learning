using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ImageLoader))]
public class ImageLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ImageLoader loader = (ImageLoader) target;
        if (GUILayout.Button("Import Images"))
        {
            loader.InitializeImages();
        }
    }
}
