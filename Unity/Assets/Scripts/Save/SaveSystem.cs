using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Save;
using UnityEngine;

public static class SaveSystem
{
   public static void SaveModel(Model model, string path)
   {
      BinaryFormatter formatter = new BinaryFormatter();
      path = path + "/model.fun";
      FileStream stream = new FileStream(path, FileMode.Create);
      
      ModelData data = new ModelData(model);

      formatter.Serialize(stream, data);
      stream.Close();

      Debug.Log("Model saved at : " + path);
   }

   public static ModelData LoadModel(string path)
   {
      path = path + "/model.fun";
      if (File.Exists(path))
      {
         BinaryFormatter formatter = new BinaryFormatter();
         FileStream stream = new FileStream(path, FileMode.Open);
         
         ModelData data = formatter.Deserialize(stream) as ModelData;
         stream.Close();

         Debug.Log("Model loaded from : " + path);
         return data;
      }
      else
      {
         Debug.LogError("Save file not found in " + path);
         return null;
      }
   }
}
