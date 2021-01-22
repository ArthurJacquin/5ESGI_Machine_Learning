using System;

[Serializable]
public class ModelData
{
    public double[] results;

    public ModelData(Model model)
    {
        results = model.results;
    }
}
