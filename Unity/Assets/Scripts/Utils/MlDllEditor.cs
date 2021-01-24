using Crosstales.FB;
using Save;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    [CustomEditor(typeof(MlDllRun))]
    public class MlDllEditor : Editor
    {
        private Model _model;
        private TypeModel _modelType;
        private TypeTest _testType;
        private bool _isClassification;
        private int _epoch = 1000;
        private double _alpha = 0.1f;
        private string _saveName;
        private string _loadPath;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //Variables éditables dans l'inspecteur
            _modelType = (TypeModel) EditorGUILayout.EnumPopup("Type de modèle :", _modelType);
            _testType = (TypeTest)EditorGUILayout.EnumPopup("Type de test :", _testType);
            _isClassification = EditorGUILayout.Toggle("Classification activée :", _isClassification);
            _epoch = EditorGUILayout.IntField("Nombre d'itérations :", _epoch);
            _alpha = EditorGUILayout.DoubleField("Alpha :", _alpha);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            MlDllRun dllRun = (MlDllRun) target;
            if (GUILayout.Button("Simulate training", GUILayout.Width(135), GUILayout.Height(30)))
            {
                dllRun.Simulate(_modelType, _testType, _isClassification);
            }
        
            if (GUILayout.Button("Training", GUILayout.Width(135), GUILayout.Height(30)))
            {
                dllRun.RunMlDll(_modelType, _testType, _isClassification, _epoch, _alpha);
            }
            EditorGUILayout.EndHorizontal();
            
            _loadPath = EditorGUILayout.TextField("Chemin du projet :", _loadPath);
            //loadPath =  FileBrowser.OpenSingleFile();
            
            _saveName = EditorGUILayout.TextField("Nom de la sauvegarde : ", _saveName);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save Model", GUILayout.Width(135), GUILayout.Height(30)))
            {
                SaveSystem.SaveModel(_model, _saveName);
            }
        
            if (GUILayout.Button("Load Model", GUILayout.Width(135), GUILayout.Height(30)))
            {
                ModelData data = SaveSystem.LoadModel(_loadPath);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
