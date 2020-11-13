using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct TestInfos
{
    public int layerCount;
    public int outputSize;
    public int[] dimensions;
}

public class MlDllRun : MonoBehaviour
{
    private List<GameObject> pool;
    
    public void RunMlDll(TypeTest type, bool isClassification, int epoch, double alpha)
    {
        //Initialisation des infos du test en focntion de son type
        var test = CreateTest(type);
        
        //Récupération des résultats via la dll
        var results = MlDllWrapper.TrainMlp((int)type, test.layerCount, test.dimensions, isClassification, epoch, alpha);

        //Gestion des résultats
        var managedResults = new double[test.outputSize];
        Marshal.Copy(results, managedResults, 0, test.outputSize);
        
        //Affichage des résultats avec les sphères
        UpdateVisualResults(managedResults);
    }

    private TestInfos CreateTest(TypeTest type)
    {
        TestInfos infos;
        
        switch (type)
        {
            case TypeTest.LinearSimple:
                Debug.Log("test : LinearSimple");
                infos = new TestInfos()
                {
                    layerCount = 2,
                    outputSize = 3,
                    dimensions = new int[] {2, 1}
                };
                break;
            case TypeTest.LinearMultiple:
                Debug.Log("test : LinearMultiple");
                infos = new TestInfos() 
                {
                    layerCount = 2,
                    outputSize = 100,
                    dimensions = new int[] {2, 1}
                };
                break;
            case TypeTest.XOR:
                Debug.Log("test : XOR");
                infos = new TestInfos()
                {
                    layerCount = 3,
                    outputSize = 4,
                    dimensions = new int[] {2, 3, 1}
                };
                break;
            case TypeTest.Cross:
                Debug.Log("test : Cross");
                infos = new TestInfos()
                {
                    layerCount = 3,
                    outputSize = 1000,
                    dimensions = new int[] {2, 4, 1}
                };
                break;
            case TypeTest.MultiLinear:
                Debug.Log("test : MultiLinear");
                infos = new TestInfos()
                {
                    //TODO : update needed
                    layerCount = 2,
                    outputSize = 3,
                    dimensions = new int[] {2, 1}
                };
                break;
            case TypeTest.MultiCross:
                Debug.Log("test : MultiCross");
                infos = new TestInfos()
                {
                    //TODO : update needed
                    layerCount = 2,
                    outputSize = 3,
                    dimensions = new int[] {2, 1}
                };
                break;
            case TypeTest.LinearSimple2D:
                Debug.Log("test : LinearSimple2D");
                infos = new TestInfos()
                {
                    layerCount = 2,
                    outputSize = 2,
                    dimensions = new int[] {1, 1}
                };
                break;
            case TypeTest.NonLinearSimple2D:
                Debug.Log("test : NonLinearSimple2D");
                infos = new TestInfos()
                {
                    //TODO : update needed
                    layerCount = 2,
                    outputSize = 3,
                    dimensions = new int[] {2, 1}
                };
                break;
            case TypeTest.LinearSimple3D:
                Debug.Log("test : LinearSimple3D");
                infos = new TestInfos()
                {
                    layerCount = 2,
                    outputSize = 3,
                    dimensions = new int[] {2, 1}
                };
                break;
            case TypeTest.LinearTricky3D:
                Debug.Log("test : LinearTricky3D");
                infos = new TestInfos()
                {
                    layerCount = 2,
                    outputSize = 3,
                    dimensions = new int[] {2, 1}
                };
                break;
            case TypeTest.NonLinearSimple3D:
                Debug.Log("test : NonLinearSimple3D");
                infos = new TestInfos()
                {
                    layerCount = 3,
                    outputSize = 4,
                    dimensions = new int[] {2, 2, 1}
                };
                break;
            default:
                Debug.Log("default test : LinearSimple");
                infos = new TestInfos()
                {
                    layerCount = 2,
                    outputSize = 3,
                    dimensions = new int[] {2, 1}
                };
                break;
        }

        return infos;
    }
    
    private void UpdateVisualResults(double[] results)
    {
        Debug.Log("Ran dll ok !");
        
        
    }
}
