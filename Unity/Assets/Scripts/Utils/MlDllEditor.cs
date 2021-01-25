using Crosstales.FB;
using Save;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    [CustomEditor(typeof(MlDllRun))]
    public class MlDllEditor : Editor
    {
        private TypeModel _modelType;
        private TypeTest _testType;
        private bool _isTestingImage;
        private TypeImage _imageType;
        private bool _isClassification;
        private int _epoch = 1000;
        private double _alpha = 0.1f;
        private double _gamma = 0.1f;
        private bool _needTraining;
        private string _saveName;
        private int _nbCenters;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Variables à configurer :");
            EditorGUILayout.EndHorizontal();
            
            //Variables éditables dans l'inspecteur
            _modelType = (TypeModel) EditorGUILayout.EnumPopup("Type de modèle :", _modelType);
            _isTestingImage = EditorGUILayout.Toggle("Tester sur des images ? ", _isTestingImage);

            if(_modelType == TypeModel.RBF &&_isTestingImage == true)
                _nbCenters = EditorGUILayout.IntField("Nombre de centres :", _nbCenters);

            _testType = (TypeTest)EditorGUILayout.EnumPopup("Type de test :", _testType);
            _imageType = (TypeImage) EditorGUILayout.EnumPopup("Type d'images :", _imageType);
            _isClassification = EditorGUILayout.Toggle("Classification activée :", _isClassification);
            _epoch = EditorGUILayout.IntField("Nombre d'itérations :", _epoch);
            _alpha = EditorGUILayout.DoubleField("Alpha :", _alpha);
            _gamma = EditorGUILayout.DoubleField("Gamma :", _gamma);
            _needTraining = EditorGUILayout.Toggle("Entrainer le modèle ?", _needTraining);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            MlDllRun dllRun = (MlDllRun) target;
            if (GUILayout.Button("Simulate training", GUILayout.Width(100), GUILayout.Height(30)))
            {
                dllRun.Simulate(_modelType, _testType, _isClassification);
            }
        
            if (GUILayout.Button("Training", GUILayout.Width(100), GUILayout.Height(30)))
            {
                //if (_isTestingImage && _imageType != ImageLoader.GetInstance().type)
                //{
                //    Debug.LogWarning("Seems like image type selected is not the one ImageLoader script had imported");
                //}
                //else
                //{
                //    Debug.Log("Ah bon ? ");
                //}
                dllRun.RunMlDll(_modelType, _testType, _isTestingImage, _imageType, _isClassification, _epoch, _alpha, _gamma, _needTraining, _nbCenters);
            }

            if (GUILayout.Button("Predict", GUILayout.Width(100), GUILayout.Height(30)))
            {
                dllRun.PredictOneImage(_modelType, _imageType, _gamma, _nbCenters);
            }
            EditorGUILayout.EndHorizontal();
            
            _saveName = EditorGUILayout.TextField("Nom de la sauvegarde : ", _saveName);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save Model", GUILayout.Width(135), GUILayout.Height(30)))
            {
                SaveSystem.SaveModel(dllRun.myModel, _saveName);
            }
        
            if (GUILayout.Button("Load Model", GUILayout.Width(135), GUILayout.Height(30)))
            {
                ModelData data = SaveSystem.LoadModel();
                dllRun.myModel.results = data.results;
                dllRun.myModel.type = data.type;
                dllRun.myModel.imageType = data.imageType;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}
