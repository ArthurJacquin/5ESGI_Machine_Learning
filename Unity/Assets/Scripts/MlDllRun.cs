using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MlDllRun : MonoBehaviour
{
    private List<GameObject> _pool;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;
    
    public void RunMlDll(TypeTest type, bool isClassification, int epoch, double alpha)
    {
        //Initialisation des infos du test en focntion de son type
        var test = new TestClass(type);
        //test.DisplayInfos();
        
        //Récupération des résultats via la dll
        var results = MlDllWrapper.TrainMlp(test.SampleCount, test.Samples, test.Outputs, 
            test.Infos.LayerCount, test.Infos.Dimensions, test.NodeCount, isClassification, epoch, alpha);

        //Gestion des résultats
        var managedResults = new double[test.Infos.OutputSize];
        Marshal.Copy(results, managedResults, 0, test.Infos.OutputSize);
        
        test.Outputs = managedResults;
        //Affichage des résultats avec les sphères
        UpdateVisualResults(test, managedResults, isClassification);
        //test.DisplayResults();
    }

    public void Simulate(TypeTest type, bool isClassification)
    {
        var test = new TestClass(type);
        UpdateVisualResults(test, test.Outputs, isClassification);
    }

    private void UpdateVisualResults(TestClass test, double[] results, bool isClassification)
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

        if (isClassification)
        {
            if (test.Infos.Dimensions[test.Infos.LayerCount - 1] == 1)
            {
                for (int i = 0; i < results.Length; ++i)
                {
                    _pool[i].transform.position = new Vector3((float)test.Samples[i * 2],(float)test.Samples[i * 2 + 1], 0.0f);
                    _pool[i].SetActive(true);

                    _pool[i].GetComponent<Renderer>().material = results[i] > 0 ? blueMat : redMat;
                }
            }
            else //multi class
            {
           
            }
        }
        else
        {
            for (int i = 0; i < results.Length; ++i)
            {
                if (test.Infos.Dimensions[0] == 1)
                {
                    _pool[i].transform.position = new Vector3((float)test.Samples[i],(float)results[i], 0.0f);
                }
                else
                {
                    _pool[i].transform.position = new Vector3((float)test.Samples[i * 2],(float)test.Samples[i * 2 + 1], (float)results[i]);
                
                }
                _pool[i].SetActive(true);
                _pool[i].GetComponent<Renderer>().material = blueMat;
            }
            
        }
        
    }
}
