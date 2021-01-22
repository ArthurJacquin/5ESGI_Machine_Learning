using UnityEngine;

public enum TypeModel
{
   Linear,
   MLP,
   RBF
}

public class Model : MonoBehaviour
{
   public double[] results;
   public TypeModel type;

}
