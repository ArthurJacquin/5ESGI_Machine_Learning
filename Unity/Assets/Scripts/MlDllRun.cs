using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Save;
using TMPro;
using UnityEditor;
using UnityEngine;
using Utils;

public class MlDllRun : MonoBehaviour
{
    private List<GameObject> _pool;
    public Model myModel;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;

    [SerializeField] private Transform simulation;
    [SerializeField] private Transform training;
    [SerializeField] public TextMeshProUGUI resultFeedback;
    
    public void RunMlDll(TypeModel modelType, TypeTest type, bool isTestingImage, TypeImage image, bool isClassification, int epoch, double alpha, double gamma, bool needTrain)
    {
        //Initialisation des infos du test en fonction de son type
        TestClass test;
        if (isTestingImage)
        {
            List<List<float>> allImages = ImageLoader.GetAllImages();
            List<int> allValues = ImageLoader.GetAllValues3dOrReal();

            if (allImages.Count <= 0)
            {
                return;
            }
            
            test = new TestClass(allImages, allValues, image);
        }
        else
        {
            test = new TestClass(type);
        }

        IntPtr model = new IntPtr();
        IntPtr res = new IntPtr();
        double[] results = new double[test.SampleCount];
        double[] managedResults = new double[1];
        
        switch (modelType)
        {
            case TypeModel.Linear:
                //Création du modèle
                model = MlDllWrapper.CreateModelLinear(test.Datasize, test.NbClass);

                if (needTrain)
                {
                    MlDllWrapper.TrainModelLinear(model, test.Samples, test.SampleCount, test.InputCount * test.Datasize,
                        test.Outputs, test.NbClass, epoch, alpha, isClassification);
                }

                //Récupération des résultats
                for (int i = 0; i < test.SampleCount; i++)
                {
                    double[] sample = new double[test.InputCount * test.Datasize];
                    for (int j = 0; j < test.InputCount * test.Datasize; j++)
                    {
                        int id = i * test.InputCount * test.Datasize + j;
                        sample[j] = test.Samples[id];
                    }

                    res = MlDllWrapper.PredictModelLinear(model, sample, test.Datasize, 
                        test.NbClass, isClassification);

                    Marshal.Copy(res, managedResults, 0, 1);

                    results[i] = managedResults[0];
                }

                break;
            
            case TypeModel.MLP:
                //Création du modèle
                model = MlDllWrapper.CreateModelMLP(test.Infos.DimensionsMLP, test.Infos.LayerCount);

                if (needTrain)
                {
                    MlDllWrapper.TrainModelMLP(model, test.Samples, test.SampleCount, test.Outputs, test.Infos.DimensionsMLP,  
                        test.Infos.LayerCount, isClassification, epoch, alpha);
                }

                //Récupération des résultats
                for (int i = 0; i < test.SampleCount; i++)
                {
                    double[] sample = new double[test.InputCount * test.Datasize];
                    for (int j = 0; j < test.InputCount * test.Datasize; j++)
                    {
                        int id = i * test.InputCount * test.Datasize + j;
                        sample[j] = test.Samples[id];
                    }

                    res = MlDllWrapper.PredictModelMLP(model, sample, test.Infos.DimensionsMLP, test.Infos.LayerCount, isClassification);

                    Marshal.Copy(res, managedResults, 0, 1);

                    results[i] = managedResults[0];
                }
                break;
            
            case TypeModel.RBF:
                //Création du modèle
                model = MlDllWrapper.CreateModelRBF(test.Infos.DimensionsRBF, test.Datasize);

                if (needTrain)
                {
                    MlDllWrapper.TrainModelRBF(model, test.Infos.DimensionsRBF, test.Samples, test.SampleCount, test.InputCount,
                                                test.Datasize, test.Outputs, epoch, gamma);
                }

                //Récupération des résultats
                for (int i = 0; i < test.SampleCount; i++)
                {
                    double[] sample = new double[test.InputCount * test.Datasize];
                    for (int j = 0; j < test.InputCount * test.Datasize; j++)
                    {
                        int id = i * test.InputCount * test.Datasize + j;
                        sample[j] = test.Samples[id];
                    }

                    res = MlDllWrapper.PredictModelRBF(model, test.Infos.DimensionsRBF, sample, test.InputCount, test.Datasize, isClassification, gamma);

                    Marshal.Copy(res, managedResults, 0, 1);

                    results[i] = managedResults[0];
                }

                break;
            
            default:
                //Création du modèle
                model = MlDllWrapper.CreateModelLinear(test.SampleCount, test.Infos.OutputSize);
                
                if (needTrain)
                {
                    //Training
                }
                
                //Récupération des résultats
                res = MlDllWrapper.ExportResultMlp(test.SampleCount, test.Samples, test.Outputs, 
                    test.Infos.LayerCount, test.Infos.DimensionsMLP, test.NodeCount, isClassification, epoch, alpha);
                
                Marshal.Copy(res, managedResults, 0, test.Infos.OutputSize);
                break;
        }

        //Nettoyage !
        MlDllWrapper.DeleteModel(model);
        MlDllWrapper.DeleteModel(res);

        myModel.type = modelType;
        myModel.results = results;
        
        //Affichage des résultats dans la scène principale
        if (isTestingImage)
            CalculatePercentage(test, results);
        else
            UpdateVisualResults(test, results, isClassification, false);
    }

    public void Simulate(TypeModel model, TypeTest type, bool isClassification)
    {
        var test = new TestClass(type);
           UpdateVisualResults(test, test.Outputs, isClassification, true);
    }

    private void CalculatePercentage(TestClass test, double[] results)
    {
        var sum = 0;
        
        for (var i = 0; i < results.Length; i++)
        {
            var expected = (int)results[i];
            if (expected == -1) continue;
            if ((int)test.Outputs[i * test.NbClass + expected] == 1) sum++;
        }

        float percentage = (float)sum / results.Length * 100;
        
        resultFeedback.SetText("Taux de réussite = " + percentage + "%");
        resultFeedback.gameObject.SetActive(true);
        Visualizers.GetInstance().HideVisualizers();
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

        if (_pool.Count < (results.Length * 2) + 2)
        {
            Debug.LogWarning("There isnt enough objects in pool");
            return;
        }
        
        Visualizers.GetInstance().HideVisualizers();
        
        var testSimulation = new TestClass(test.Type);
        
        if (isClassification)
        {
            for (int i = 0; i < test.SampleCount; i++)
            {
                //résultats de la simulation
                _pool[i].transform.position = simulation.position + new Vector3((float)testSimulation.Samples[i * 2],(float)testSimulation.Samples[i * 2 + 1], 0.0f);
                _pool[i].SetActive(true);

                if (testSimulation.NbClass < 3)
                {
                    //testSimulation.Outputs = { 0, 1, 1, 0} ;
                    if((int)testSimulation.Outputs[i * testSimulation.NbClass] == 1)
                        _pool[i].GetComponent<Renderer>().material = blueMat;
                    else if((int)testSimulation.Outputs[i * testSimulation.NbClass + 1] == 1)
                        _pool[i].GetComponent<Renderer>().material = redMat;
                    else 
                        _pool[i].GetComponent<Renderer>().material.color = Color.magenta;
                }
                else
                {
                    if((int)testSimulation.Outputs[i * testSimulation.NbClass] == 1)
                        _pool[i].GetComponent<Renderer>().material = blueMat;
                    else if((int)testSimulation.Outputs[i * testSimulation.NbClass + 1] == 1)
                        _pool[i].GetComponent<Renderer>().material = redMat;
                    else if((int)testSimulation.Outputs[i * testSimulation.NbClass + 2] == 1)
                        _pool[i].GetComponent<Renderer>().material = greenMat;
                    else
                        _pool[i].GetComponent<Renderer>().material.color = Color.magenta;
                }

                if (!isSimulation)
                {
                    int j = i + results.Length + 1;
                    //résultats du training
                    _pool[j].transform.position = training.position + new Vector3((float)test.Samples[i * 2],(float)test.Samples[i * 2 + 1], 0.0f);
                    _pool[j].SetActive(true);

                    if(results[i] == 0)
                        _pool[j].GetComponent<Renderer>().material = blueMat;
                    else if(results[i] == 1)
                        _pool[j].GetComponent<Renderer>().material = redMat;
                    else if(results[i] == 2)
                        _pool[j].GetComponent<Renderer>().material = greenMat;
                    else 
                        _pool[j].GetComponent<Renderer>().material.color = Color.magenta;
                }
                
            }
        }
        else
        {
            for (int i = 0; i < results.Length; ++i)
            {
                int j = i + results.Length + 1;
                
                if (test.Infos.DimensionsMLP[0] == 1)
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