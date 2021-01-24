using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//TODO : vérifier les valeurs de retour des fonctions
public static class MlDllWrapper
{
    //---------------------------Linear--------------------------------------------
    
    [DllImport("MlDll", EntryPoint = "create_linear_model")]
    public static extern System.IntPtr CreateModelLinear(int inputCount, int outputCount);
    
    [DllImport("MlDll", EntryPoint = "predict_linear_model")]
    public static extern System.IntPtr PredictModelLinear(System.IntPtr model, double[] samples, int inputCount, 
        int outputCount, bool isClassification);
    
    [DllImport("MlDll", EntryPoint = "train_linear_model")]
    public static extern void TrainModelLinear(System.IntPtr model, double[] allSamples, int sampleCount,
        int inputCount, double[] allExpectedOutputs, int outputCount, int epochs, double learningRate, bool isClassification);
    

    //---------------------------MLP-----------------------------------------------

    [DllImport("MlDll", EntryPoint = "create_MLP_model")]
    public static extern System.IntPtr CreateModelMLP(int[] dims, int layerCount);

    [DllImport("MlDll", EntryPoint = "predict_MLP")]
    public static extern System.IntPtr PredictModelMLP(System.IntPtr model, double[] samples, int[] dims,
        int layerCount, bool isClassification);

    [DllImport("MlDll", EntryPoint = "train_MLP")]
    public static extern System.IntPtr TrainModelMLP(System.IntPtr model, double[] allSamples, int sampleCount,
        double[] allExpectedOutputs, int[] dims, int layerCount, bool isClassification, int epochs, double alpha);
    
    [DllImport("MlDll", EntryPoint = "export_result")]
    public static extern System.IntPtr ExportResultMlp(int sampleCount, double[] samples, double[] outputs,
        int layerCount, int[] dims, int nodeCount, bool isClassification, int epoch, double alpha);
    
    
    //---------------------------RBF----------------------------------------------

    [DllImport("MlDll", EntryPoint = "create_RBF_model")]
    public static extern System.IntPtr CreateModelRBF(int[] dims, int dataSize);

    [DllImport("MlDll", EntryPoint = "training_RBF_model")]
    public static extern System.IntPtr TrainModelRBF(System.IntPtr model, int[] dims, double[] samples, int sampleSize,
        int inputSize, int dataSize, double[] output, int epoch, double gamma);

    [DllImport("MlDll", EntryPoint = "predict_RBF_model")]
    public static extern System.IntPtr PredictModelRBF(System.IntPtr model, int[] dims, double[] samples, int inputSize,
        int dataSize, bool isClassification, double gamma);
    
    
    //----------------------------------------------------------------------------
    
    [DllImport("MlDll", EntryPoint = "delete_model")]
    public static extern void DeleteModel(System.IntPtr model);
}
