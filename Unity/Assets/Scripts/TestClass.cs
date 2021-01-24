using System.Collections.Generic;
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
    public int[] DimensionsMLP;
    public int[] DimensionsRBF;
}

public class TestClass
{
    public int SampleCount; //Nombre de sample
    public int InputCount; //Nombre de data dans 1 samples (nb de pixels)
    public int Datasize; //Nombre de composantes brutes (r, g, b pour un pixel)
    public int NbClass; //Nombre de classes
    public int NodeCount; //Nombre de neuronnes dans le réseau

    public double[] Samples;
    public double[] Outputs;

    public TypeTest Type;

    public TestInfos Infos;

    public TestClass(List<float> image, TypeTest type)
    {
        Type = type;
        Infos = new TestInfos();
        
        SampleCount = image.Count;
        Samples = new double[SampleCount];

        for (int i = 0; i < image.Count; ++i)
        {
            Samples[i] = image[i];
        }
        
        switch (type)
        {
            case TypeTest.LinearSimple:
                Outputs = new double[] {1, -1, -1};

                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
            
            case TypeTest.LinearMultiple:
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
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
            
            case TypeTest.XOR:
                Outputs = new double[] {1, 1, -1, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 4,
                    DimensionsMLP = new int[] {2, 3, 1}
                };
                break;
            
            case TypeTest.Cross:
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
                    DimensionsMLP = new int[] {2, 4, 1}
                };
                break;
            
            case TypeTest.MultiLinear:
                //TODO : update needed
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    //TODO : update needed
                    LayerCount = 2,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
            
            case TypeTest.MultiCross:
                //TODO : update needed
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    //TODO : update needed
                    LayerCount = 2,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
            
            case TypeTest.LinearSimple2D:
                Outputs = new double[] {2, 3};
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 2,
                    DimensionsMLP = new int[] {1, 1}
                };
                break;
            
            case TypeTest.NonLinearSimple2D:
                Outputs = new double[] {2, 3, 2.5f};
                
                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {1, 3, 1}
                };
                break;
            
            case TypeTest.LinearSimple3D:
                Outputs = new double[] {2, 3, 2.5f};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
            
            case TypeTest.LinearTricky3D:
                Outputs = new double[] {1, 2, 3};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
            
            case TypeTest.NonLinearSimple3D:
                Outputs = new double[] {2, 1, -2, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 4,
                    DimensionsMLP = new int[] {2, 2, 1}
                };
                break;
            
            default:
                Debug.Log("Create a LinearSimple as default value for test");
                Outputs = new double[] {1, -1, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
        }
        for (int i = 0; i < Infos.LayerCount; ++i)
        {
            NodeCount += Infos.DimensionsMLP[i];
        }

        NodeCount += Infos.LayerCount;
    }
    
    public TestClass(TypeTest type)
    {
        Type = type;
        Infos = new TestInfos();
        NbClass = 2;

        switch (type)
        {
            case TypeTest.LinearSimple:
                SampleCount = 3;
                InputCount = 1;
                Datasize = 2;

                Samples = new double[] {1.0, 1.0, 2.0, 3.0, 3.0, 3.0};
                Outputs = new double[] {1, 0, 0, 1, 0, 1};

                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 3, 2 },
                    DimensionsRBF = new int[] { 3, 2 }
                };
                break;
            
            case TypeTest.LinearMultiple:
                InputCount = 1;
                Datasize = 2;
                SampleCount = 100;
                Samples = new double[SampleCount * 2];
                for (int i = 0; i < SampleCount * 2; ++i)
                {
                    if (i < 100)
                        Samples[i] = (double)(Random.Range(0.0f, 1.0f) * 0.9f + 1.0f);
                    else 
                        Samples[i] = (double)(Random.Range(0.0f, 1.0f) * 0.9f + 2.0f);
                }

                Outputs = new double[SampleCount * NbClass];
                for (int i = 0; i < SampleCount; ++i)
                {
                    if (i < 50)
                    {
                        Outputs[i * NbClass] = 1.0;
                        Outputs[i * NbClass + 1] = 0.0;
                    }
                    else
                    {
                        Outputs[i * NbClass] = 0.0;
                        Outputs[i * NbClass + 1] = 1.0;
                    }
                }

                Infos = new TestInfos() 
                {
                    OutputSize = 100,
                    LayerCount = 3,
                    DimensionsMLP = new int[] {2, 3, 2},
                    DimensionsRBF = new int[] { 100, 2 }
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
                    DimensionsMLP = new int[] {2, 3, 1}
                };
                break;
            
            case TypeTest.Cross:
                SampleCount = 500;
                Samples = new double[SampleCount * 2];
                for (int i = 0; i < SampleCount * 2; ++i)
                {
                    Samples[i] = Random.Range(0.0f, 1.0f) * 2.0f - 1.0f;
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
                    DimensionsMLP = new int[] {2, 4, 1}
                };
                break;
            
            case TypeTest.MultiLinear:
                SampleCount = 500;
                Samples = new double[SampleCount * 2];

                for (int i = 0; i < (SampleCount * 2); ++i)
                {
                    Samples[i] = Random.Range(0.0f, 1.0f) * 2.0f - 1.0f;
                }

                Outputs = new double[SampleCount * 3];
                for (int i = 0; i < SampleCount; ++i)
                {
                    if (-(Samples[i * 2]) - Samples[i * 2 + 1] - 0.5 > 0 && Samples[i * 2 + 1] < 0 && Samples[i * 2] - Samples[i * 2 + 1] - 0.5 < 0)
                    {
                        Outputs[i * 3] = 1.0;
                        Outputs[i * 3 + 1] = 0.0;
                        Outputs[i * 3 + 2] = 0.0;
                    }
                    else if (-(Samples[i * 2]) - Samples[i * 2 + 1] - 0.5 < 0 && Samples[i * 2 + 1] > 0 && Samples[i * 2] - Samples[i * 2 + 1] - 0.5 < 0)
                    {
                        Outputs[i * 3] = 0.0;
                        Outputs[i * 3 + 1] = 1.0;
                        Outputs[i * 3 + 2] = 0.0;
                    }
                    else if (-(Samples[i * 2]) - Samples[i * 2 + 1] - 0.5 < 0 && Samples[i * 2 + 1] < 0 && Samples[i * 2] - Samples[i * 2 + 1] - 0.5 > 0)
                    {
                        Outputs[i * 3] = 0.0;
                        Outputs[i * 3 + 1] = 0.0;
                        Outputs[i * 3 + 2] = 1.0;
                    }
                    else
                    {
                        Outputs[i * 3] = 0.0;
                        Outputs[i * 3 + 1] = 0.0;
                        Outputs[i * 3 + 2] = 0.0;
                    }
                }

                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 500,
                    DimensionsMLP = new int[] {2, 3}
                };
                break;
            
            case TypeTest.MultiCross:
                SampleCount = 1000;
                Samples = new double[SampleCount * 2];

                for (int i = 0; i < (SampleCount * 2); ++i)
                {
                    Samples[i] = Random.Range(0.0f, 1.0f) * 2.0f - 1.0f;
                }

                Outputs = new double[SampleCount * 3];
                for (int i = 0; i < SampleCount; ++i)
                {
                    if (Mathf.Abs((float)Samples[i * 2] % 0.5f) <= 0.25 && Mathf.Abs((float)Samples[i * 2 + 1] % 0.5f) > 0.25)
                    {
                        Outputs[i * 3] = 1.0;
                        Outputs[i * 3 + 1] = 0.0;
                        Outputs[i * 3 + 2] = 0.0;
                    }
                    else if (Mathf.Abs((float)Samples[i * 2] % 0.5f) > 0.25 && Mathf.Abs((float)Samples[i * 2 + 1] % 0.5f) <= 0.25)
                    {
                        Outputs[i * 3] = 0.0;
                        Outputs[i * 3 + 1] = 1.0;
                        Outputs[i * 3 + 2] = 0.0;
                    }
                    else
                    {
                        Outputs[i * 3] = 0.0;
                        Outputs[i * 3 + 1] = 0.0;
                        Outputs[i * 3 + 2] = 1.0;
                    }
                }

                Infos = new TestInfos()
                {
                    LayerCount = 4,
                    OutputSize = 1000,
                    DimensionsMLP = new int[] {2, 3, 3, 3}
                };
                break;
            
            case TypeTest.LinearSimple2D:
                SampleCount = 2;
                Samples = new double[] {1.0f, 2.0f};
                Outputs = new double[] {2, 3};
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 2,
                    DimensionsMLP = new int[] {1, 1}
                };
                break;
            
            case TypeTest.NonLinearSimple2D:
                SampleCount = 3;
                Samples = new double[] {1.0f, 2.0f, 3.0f};
                Outputs = new double[] {2, 3, 2.5f};
                
                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {1, 3, 1}
                };
                break;
            
            case TypeTest.LinearSimple3D:
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 2.0f, 3.0f, 1.0f};
                Outputs = new double[] {2, 3, 2.5f};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
            
            case TypeTest.LinearTricky3D:
                SampleCount = 3;
                Samples = new double[] {1.0f, 1.0f, 2.0f, 2.0f, 3.0f, 3.0f};
                Outputs = new double[] {1, 2, 3};
                
                Infos = new TestInfos()
                {
                    LayerCount = 2,
                    OutputSize = 3,
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
            
            case TypeTest.NonLinearSimple3D:
                SampleCount = 4;
                Samples = new double[] {1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f};
                Outputs = new double[] {2, 1, -2, -1};
                
                Infos = new TestInfos()
                {
                    LayerCount = 3,
                    OutputSize = 4,
                    DimensionsMLP = new int[] {2, 2, 1}
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
                    DimensionsMLP = new int[] {2, 1}
                };
                break;
        }
        for (int i = 0; i < Infos.LayerCount; ++i)
        {
            NodeCount += Infos.DimensionsMLP[i];
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
