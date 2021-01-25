using System;
using UnityEngine;
using Utils;

namespace Save
{
   public enum TypeModel
   {
      Linear,
      MLP,
      RBF
   }

   public class Model : MonoBehaviour
   {
      public IntPtr results;
      public TypeModel type;
      public TypeImage imageType;
   }
}