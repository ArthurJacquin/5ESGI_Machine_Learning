using System.Runtime.InteropServices;

public static class MlDllWrapper
{
    [DllImport("MlDll", EntryPoint = "my_add")]
    public static extern double MyAdd(double a, double b);

    //--------------------------------------Modèle linéaire-----------------------------------
    [DllImport("MlDll", EntryPoint = "create_linear_model")]
    public static extern System.IntPtr CreateLinearModel(int inputSize);
    
    [DllImport("MlDll", EntryPoint = "predict_linear_model_classification")]
    public static extern double PredictLinearModelClassification(System.IntPtr model,
        double[] inputs, int inputSize);

    [DllImport("MLDLL", EntryPoint = "train_linear_model_Rosenblatt")]
    public static extern void TrainLinearModelRosenblatt(System.IntPtr model, double[] all_samples, int sample_count, int input_count,
        double[] all_expected_outputs, int epochs, double learning_rate);

    [DllImport("MlDll", EntryPoint = "delete_linear_model")]
    public static extern void DeleteLinearModel(System.IntPtr model);

    //-------------------------------------------------PMC--------------------------------------------
    [DllImport("MlDll", EntryPoint = "create_pmc_model")]
    public static extern System.IntPtr CreatePmcModel(int[] npl, int size);

    [DllImport("MlDll", EntryPoint = "predict_linear_model_multiclass_classification")]
    public static extern System.IntPtr PredictLinearModelMultiClassClassification(System.IntPtr model,
        double[] inputs, int inputSize, int classCount);

    [DllImport("MLDLL", EntryPoint = "predict")]
    public static extern System.IntPtr Predict(System.IntPtr model, double[] inputs, bool isClassification);

    [DllImport("MLDLL", EntryPoint = "train")]
    public static extern void Train(System.IntPtr model, double[] allInputs, double[] allExpectedOutputs,
    bool isClassification, int sampleCount, int epochs, double alpha);

    [DllImport("MlDll", EntryPoint = "delete_pmc_model")]
    public static extern void DeletePmcModel(System.IntPtr model);
}