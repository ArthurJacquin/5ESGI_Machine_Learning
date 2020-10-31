using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private System.IntPtr model;

    [Header("Parameters")]
    public int[] npl = new int[0];
    public int sampleCounts;
    public int epochs;
    public double alpha;
    public bool isClassification;

    [Header("Inputs")]
    public Transform[] transformInputs = new Transform[0];
    private double[] inputs = new double[0];

    [Header("Outputs")]
    public double[] outputs = new double[0];
    
    // Start is called before the first frame update
    void Start()
    {
        model = MlDllWrapper.CreateLinearModel(npl.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Predict()
    {

    }

    public void Train()
    {
        //MlDllWrapper.TrainLinearModelRosenblatt();
    }

    private void OnDestroy()
    {
        MlDllWrapper.DeleteLinearModel(model);
    }
}