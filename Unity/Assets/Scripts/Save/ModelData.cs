﻿using System;
using Utils;

namespace Save
{
    [Serializable]
    public class ModelData
    {
        public IntPtr results;
        public TypeModel type;
        public TypeImage imageType;
        public ModelData(Model model)
        {
            results = model.results;
            type = model.type;
            imageType = model.imageType;
        }
    }
}
