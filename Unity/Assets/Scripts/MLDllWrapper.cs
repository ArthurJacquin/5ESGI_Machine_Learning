using System.Runtime.InteropServices;

public static class MlDllWrapper
{
    [DllImport("MlDll", EntryPoint = "export_result")]
    public static extern System.IntPtr TrainMlp(int sampleCount, double[] samples, double[] outputs,
        int layerCount, int[] dims, int nodeCount, bool isClassification, int epoch, double alpha);
}
