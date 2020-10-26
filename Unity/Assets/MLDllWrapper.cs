
using System.Runtime.InteropServices;

public static class MlDllWrapper
{
    [DllImport("MlDll", EntryPoint = "my_add")]
    public static extern double MyAdd(double a, double b);
    
    [DllImport("MlDll", EntryPoint = "create_linear_model")]
    public static extern System.IntPtr CreateLinearModel(int inputSize);
    
    [DllImport("MlDll", EntryPoint = "predict_linear_model_classification")]
    public static extern double PredictLinearModelClassification(System.IntPtr model,
        double[] inputs, int inputSize);
    
    [DllImport("MlDll", EntryPoint = "predict_linear_model_multiclass_classification")]
    public static extern System.IntPtr PredictLinearModelMultiClassClassification(System.IntPtr model,
        double[] inputs, int inputSize, int classCount);
    
    [DllImport("MlDll", EntryPoint = "delete_linear_model")]
    public static extern void DeleteLinearModel(System.IntPtr model);
}
