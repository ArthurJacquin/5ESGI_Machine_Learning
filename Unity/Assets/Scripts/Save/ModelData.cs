using System;

namespace Save
{
    [Serializable]
    public class ModelData
    {
        public double[] results;
        public TypeModel type;
        public ModelData(Model model)
        {
            results = model.results;
            type = model.type;
        }
    }
}
