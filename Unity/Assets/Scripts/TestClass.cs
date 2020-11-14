using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum TypeTest
{
    LinearSimple,
    LinearMultiple,
    XOR,
    Cross,
    MultiLinear,
    MultiCross,
    LinearSimple2D,
    NonLinearSimple2D,
    LinearSimple3D,
    LinearTricky3D,
    NonLinearSimple3D
}

public struct TestInfos
{
    public int LayerCount;
    public int OutputSize;
    public int[] Dimensions;
}

public class TestClass
{
    public int SampleCount;
    public int NodeCount;

    public double[] Samples;
    public double[] Outputs;

    public TypeTest Type;

    public TestInfos Infos;
    
    public TestClass(TypeTest type)
    {
        Type = type;
        Infos = new TestInfos();
        
        switch (type)
        {
            case TypeTest.LinearSimple:
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    Dimensions = new int[] {2, 1}
                };
                break;
            
            case TypeTest.LinearMultiple:
                SampleCount = 100;
                Samples = new double[SampleCount * 2];
                for (int i = 0; i < SampleCount * 2; ++i)
                {
                    if (i < 100)
                        Samples[i] = Random.Range(0.0f, 1.0f) / double.MaxValue + 1.0f;
                    else 
                        Samples[i] = Random.Range(0.0f, 1.0f) / double.MaxValue + 2.0f;
                }

                Outputs = new double[SampleCount];
                for (int i = 0; i < SampleCount; ++i)
                {
                    if (i < 50)
                        Outputs[i] = 1.0f;
                    else
                        Outputs[i] = -1.0f;
                }

                Infos = new TestInfos() 
                {
                    LayerCount = 2,
                    OutputSize = 100,
                    Dimensions = new int[] {2, 1}
                };
                break;
            
            case TypeTest.XOR:
                SampleCount = 4;
                Samples = new double[] {1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f};
                Outputs = new double[] {1, 1, -1, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 4,
                    Dimensions = new int[] {2, 3, 1}
                };
                break;
            
            case TypeTest.Cross:
                SampleCount = 500;
                Samples = new double[SampleCount * 2];
                for (int i = 0; i < SampleCount * 2; ++i)
                {
                    Samples[i] = Random.Range(0.0f, 1.0f) / double.MaxValue * 2.0f - 1.0f;
                }
                
                Outputs = new double[SampleCount];
                for (int i = 0; i < SampleCount; ++i)
                {
                    if (Mathf.Abs((float)Samples[i * 2]) <= 0.3f || Mathf.Abs((float)Samples[i * 2 + 1]) <= 0.3f )
                        Outputs[i] = 1.0f;
                    else
                        Outputs[i] = -1.0f;
                }
                
                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 500,
                    Dimensions = new int[] {2, 4, 1}
                };
                break;
            
            case TypeTest.MultiLinear:
                //TODO : update needed
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    //TODO : update needed
                    LayerCount = 2,
                    OutputSize = 3,
                    Dimensions = new int[] {2, 1}
                };
                break;
            
            case TypeTest.MultiCross:
                //TODO : update needed
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    //TODO : update needed
                    LayerCount = 2,
                    OutputSize = 3,
                    Dimensions = new int[] {2, 1}
                };
                break;
            
            case TypeTest.LinearSimple2D:
                //TODO : update needed
                SampleCount = 2;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 2,
                    Dimensions = new int[] {1, 1}
                };
                break;
            
            case TypeTest.NonLinearSimple2D:
                //TODO : update needed
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    //TODO : update needed
                    LayerCount = 2,
                    OutputSize = 3,
                    Dimensions = new int[] {2, 1}
                };
                break;
            
            case TypeTest.LinearSimple3D:
                //TODO : update needed
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    Dimensions = new int[] {2, 1}
                };
                break;
            
            case TypeTest.LinearTricky3D:
                //TODO : update needed
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    Dimensions = new int[] {2, 1}
                };
                break;
            
            case TypeTest.NonLinearSimple3D:
                //TODO : update needed
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 4,
                    Dimensions = new int[] {2, 2, 1}
                };
                break;
            
            default:
                Debug.Log("Create a LinearSimple as default value for test");
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 3.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    Dimensions = new int[] {2, 1}
                };
                break;
        }
        for (int i = 0; i < Infos.LayerCount; ++i)
        {
            NodeCount += Infos.Dimensions[i];
        }

        NodeCount += Infos.LayerCount;
    }
    
    public void DisplayInfos()
    {
        Debug.LogFormat("Type of the test : {0}", Type.ToString());
        Debug.Log("SAMPLES :");
        for (int i = 0; i < SampleCount; ++i)
        {
            Debug.LogFormat("Sample {0} : ({1}, {2})", i, Samples[i * 2], Samples[i * 2 + 1]);
        }
        Debug.Log("OUTPUTS :");
        for (int i = 0; i < Outputs.Length; ++i)
        {
            Debug.LogFormat("Output {0} : {1}", i, Outputs[i]);
        }
    }

    public void DisplayResults()
    {
        Debug.LogFormat("Type of the test : {0}", Type.ToString());

        Debug.Log("RESULTS :");
        for (int i = 0; i < Outputs.Length; ++i)
        {
            Debug.LogFormat("Result {0} : {1}", i, Outputs[i]);
        }
    }
}
