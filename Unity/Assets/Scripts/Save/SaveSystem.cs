using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Save
{
   public static class SaveSystem
   {
      public static void SaveModel(Model model, string name)
      {
         string path = Application.dataPath + "/Saves/model_" + name;
         //TODO : créer une nouvelle sauvegarde pour pas écraser l'ancienne si existante
      
         BinaryFormatter formatter = new BinaryFormatter();
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
}
