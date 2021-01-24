using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Save;
using UnityEngine;

public class MlDllRun : MonoBehaviour
{
    private List<GameObject> _pool;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;

    [SerializeField] private Transform simulation;
    [SerializeField] private Transform training;
    //[SerializeField] private List<int> dimensions;
    
    public void RunMlDll(TypeModel model, TypeTest type, bool isClassification, int epoch, double alpha)
    {
        //Initialisation des infos du test en fonction de son type
        var test = new TestClass(type);
        //test.DisplayInfos();

        IntPtr results;
        
        //TODO : Selon le modèle, appeler la bonne fonction de la dll
        switch (model)
        {
            case TypeModel.Linear:
                //Récupération des résultats via la dll
                results = MlDllWrapper.TrainMlp(test.SampleCount, test.Samples, test.Outputs, 
                    test.Infos.LayerCount, test.Infos.Dimensions, test.NodeCount, isClassification, epoch, alpha);
                break;
            
            case TypeModel.MLP:
                results = MlDllWrapper.TrainMlp(test.SampleCount, test.Samples, test.Outputs, 
                    test.Infos.LayerCount, test.Infos.Dimensions, test.NodeCount, isClassification, epoch, alpha);
                break;
            
            case TypeModel.RBF:
                results = MlDllWrapper.TrainMlp(test.SampleCount, test.Samples, test.Outputs, 
                    test.Infos.LayerCount, test.Infos.Dimensions, test.NodeCount, isClassification, epoch, alpha);
                break;
            
            default:
                results = MlDllWrapper.TrainMlp(test.SampleCount, test.Samples, test.Outputs, 
                    test.Infos.LayerCount, test.Infos.Dimensions, test.NodeCount, isClassification, epoch, alpha);
                break;
        }
        
        //Gestion des résultats
        var managedResults = new double[test.Infos.OutputSize];
        Marshal.Copy(results, managedResults, 0, test.Infos.OutputSize);
        
        test.Outputs = managedResults;
        //Affichage des résultats dans la scène principale
        UpdateVisualResults(test, managedResults, isClassification, false);
        //test.DisplayResults();
    }

    public void Simulate(TypeModel model, TypeTest type, bool isClassification)
    {
        var test = new TestClass(type);
        UpdateVisualResults(test, test.Outputs, isClassification, true);
    }

    private void UpdateVisualResults(TestClass test, double[] results, bool isClassification, bool isSimulation)
    {
        Pooler.GetInstance().InitializePool();
        _pool = new List<GameObject>(Pooler.GetInstance().GetPoolList());

        if (_pool == null)
        {
            Debug.LogWarning("There is no objects in pool");
            return;
        }

        if (_pool.Count < results.Length)
        {
            Debug.LogWarning("There isnt enough objects in pool");
            return;
        }
        
        var testSimulation = new TestClass(test.Type);
        
        if (isClassification)
        {
            if (test.Infos.Dimensions[test.Infos.LayerCount - 1] == 1)
            {
                for (int i = 0; i < results.Length; ++i)
                {
                    //résultats de la simulation
                    _pool[i].transform.position = simulation.position + new Vector3((float)testSimulation.Samples[i * 2],(float)testSimulation.Samples[i * 2 + 1], 0.0f); 
                    _pool[i].SetActive(true);

                    _pool[i].GetComponent<Renderer>().material = testSimulation.Outputs[i] > 0 ? blueMat : redMat;
                    
                    if (!isSimulation)
                    {
                        int j = i + results.Length + 1;
                        //résultats du training
                        _pool[j].transform.position = training.position + new Vector3((float)test.Samples[i * 2],(float)test.Samples[i * 2 + 1], 0.0f);
                        _pool[j].SetActive(true);

                        _pool[j].GetComponent<Renderer>().material = results[i] > 0 ? blueMat : redMat;
                    }
                }
            }
            else //TODO : multi class
            {
           
            }
        }
        else
        {
            for (int i = 0; i < results.Length; ++i)
            {
                int j = i + results.Length + 1;
                
                if (test.Infos.Dimensions[0] == 1)
                {
                    //Résultats de la simulation
                    _pool[i].transform.position = simulation.position + new Vector3((float)testSimulation.Samples[i],(float)testSimulation.Outputs[i], 0.0f);

                    if (!isSimulation)
                    {
                        //résultats du training
                        _pool[j].transform.position = training.position + new Vector3((float)test.Samples[i],(float)results[i], 0.0f);
                    }
                }
                else
                {
                    //résultats de la simulation
                    _pool[i].transform.position = simulation.position + new Vector3((float)testSimulation.Samples[i * 2],(float)testSimulation.Samples[i * 2 + 1], (float)testSimulation.Outputs[i]);

                    if (!isSimulation)
                    {
                        //résultats du training
                        _pool[j].transform.position = training.position + new Vector3((float)test.Samples[i * 2],(float)test.Samples[i * 2 + 1], (float)results[i]);
                    }
                }
                
                _pool[i].SetActive(true);
                _pool[i].GetComponent<Renderer>().material = blueMat;

                if (!isSimulation)
                {
                    _pool[j].SetActive(true);
                    _pool[j].GetComponent<Renderer>().material = blueMat;
                }
            }
            
        }
        
    }
}
