using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestMlDll : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(MlDllWrapper.MyAdd(42.0, 51.0));
        
    }

    private void Update()
    {
        var model = MlDllWrapper.CreateLinearModel(4096);
        var inputs = new double[] {1.0, 2.0, 3.0};
        var result = MlDllWrapper.PredictLinearModelMultiClassClassification(model,
            inputs, 3, 3);
        
        var result_managed = new double[3];
        
        Marshal.Copy(result, result_managed, 0, 3);

        foreach (var rslt in result_managed)
        {
            Debug.Log(rslt);
        }
        
        MlDllWrapper.DeleteLinearModel(result);
        
        MlDllWrapper.DeleteLinearModel(model);
    }
}