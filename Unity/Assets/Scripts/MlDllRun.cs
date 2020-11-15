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
        test.DisplayInfos();
        
        //Récupération des résultats via la dll
        var results = MlDllWrapper.TrainMlp(test.SampleCount, test.Samples, test.Outputs, 
            test.Infos.LayerCount, test.Infos.Dimensions, test.NodeCount, isClassification, epoch, alpha);

        //Gestion des résultats
        var managedResults = new double[test.Infos.OutputSize];
        Marshal.Copy(results, managedResults, 0, test.Infos.OutputSize);
        
        test.Outputs = managedResults;
        //Affichage des résultats avec les sphères
        UpdateVisualResults(test, managedResults);
        //test.DisplayResults();
    }

    private void UpdateVisualResults(TestClass test, double[] results)
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

        switch (test.Type)
        {
            case TypeTest.LinearSimple:
                for (int i = 0; i < results.Length; ++i)
                {
                    _pool[i].transform.position = new Vector3((float)test.Samples[i * 2],(float)test.Samples[i * 2 + 1], 0.0f);
                    _pool[i].SetActive(true);

                    _pool[i].GetComponent<Renderer>().material = results[i] > 0 ? blueMat : redMat;
                }
                break;
            
            case TypeTest.LinearMultiple:
                break;
            
            case TypeTest.XOR:
                for (int i = 0; i < results.Length; ++i)
                {
                    _pool[i].transform.position = new Vector3((float)test.Samples[i * 2],(float)test.Samples[i * 2 + 1], 0.0f);
                    _pool[i].SetActive(true);

                    _pool[i].GetComponent<Renderer>().material = results[i] > 0 ? blueMat : redMat;
                }
                break;
            
            case TypeTest.Cross:
                break;
            
            
            case TypeTest.MultiLinear:
                break;
            
            case TypeTest.MultiCross:
                break;
            
            case TypeTest.LinearSimple2D:
                break;
            
            case TypeTest.NonLinearSimple2D:
                break;
            
            case TypeTest.LinearSimple3D:
                break;
            
            case TypeTest.LinearTricky3D:
                break;
            
            case TypeTest.NonLinearSimple3D:
                break;
            
            default:
                for (int i = 0; i < results.Length; ++i)
                {
                    _pool[i].transform.position = new Vector3((float)test.Samples[i * 2],(float)test.Samples[i * 2 + 1], 0.0f);
                    _pool[i].SetActive(true);

                    _pool[i].GetComponent<Renderer>().material = results[i] > 0 ? blueMat : redMat;
                }
                break;
        }
    }
}
