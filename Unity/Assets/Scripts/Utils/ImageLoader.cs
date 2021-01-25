using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public enum TypeImage
    {
        Color4X4,
        Color16X16,
        Color,
        RnB4X4,
        RnB16X16,
    }
    
    [ExecuteInEditMode]
    public class ImageLoader : MonoBehaviour
    {
        public List<TestImportImage> visualizers;
        public bool visualize;

        public bool updateEditor;
        public string projectFolderPath;
        public TypeImage type;
        public bool loadAllImages;
        public int nbImagesToLoad;
        public bool loadRandomImages;

        private static ImageLoader _instance;
        public static List<List<float>> images;
        public static List<int> images3DOrReal; // Si la valeur est à 0 => 3D, si la valeur est à 1 => Real
        private void Awake()
        {
            Debug.Log("Awake");

            if (_instance == null)
                _instance = this;
            if (images == null)
                InitializeImages();

            updateEditor = false;
        }
    
        public static ImageLoader GetInstance()
        {
            if(_instance)
                return _instance;
            
            Debug.LogWarning("No instances of ImageLoader found !!! ");
            return null;
        }
    
        private void Update()
        {
            if (_instance == null && updateEditor)
                _instance = this;
            if(!visualize)
                Visualizers.GetInstance().HideVisualizers();
        }

        private void ImportImages()
        {
            if (visualize)
                visualizers = Visualizers.GetInstance().GetPoolList();

            string path = projectFolderPath;
            
            switch (type)
            {
                case TypeImage.Color4X4:
                    path += "/DataSet_Color/DataSet_4x4";
                    break;
                case TypeImage.Color16X16:
                    path += "/DataSet_Color/DataSet_16x16";
                    break;
                case TypeImage.Color:
                    path += "/DataSet_Color/DataSet_FullReso";
                    break;
                case TypeImage.RnB4X4:
                    path += "/DataSet_RnB/DataSet_4x4";
                    break;
                case TypeImage.RnB16X16:
                    path += "/DataSet_RnB/DataSet_16x16";
                    break;
                default:
                    path += "/DataSet_Color/DataSet_4x4";
                    break;
            }
            
            string pathReal = projectFolderPath != "" ? path + "/DataSet_real" : "C:/ESGI/5A_Projets/5ESGI_Machine_Learning/DataSet_Color/DataSet_4x4/DataSet_Real";
            Debug.Log("Path for real images dataset : " + pathReal);
            string path3D = projectFolderPath != "" ? path + "/DataSet_3D" : "C:/ESGI/5A_Projets/5ESGI_Machine_Learning/DataSet_Color/DataSet_4x4/DataSet_3D";
            Debug.Log("Path for 3D images dataset : " + path3D);
            
            string[] files = System.IO.Directory.GetFiles(pathReal, "*.jpg");
            string[] files3D = System.IO.Directory.GetFiles(path3D, "*.jpg");

            List<List<float>> imagesReal = LoadImagesFromPath(files);
            List<List<float>> images3D = LoadImagesFromPath(files3D);

            List<int> valuesReal = Enumerable.Repeat(1, imagesReal.Count).ToList();
            List<int> values3D = Enumerable.Repeat(0, images3D.Count).ToList();
            
            for (var i = 0; i < imagesReal.Count; i++)
            {
                images.Add(imagesReal[i]);
                images3DOrReal.Add(valuesReal[i]);
            }

            for (var i = 0; i < images3D.Count; i++)
            {
                images.Add(images3D[i]);
                images3DOrReal.Add(values3D[i]);
            }

            Debug.Log("Found images : " + images.Count);
        }

        private List<List<float>> LoadImagesFromPath(string[] filepath)
        {
            List<List<float>> images = new List<List<float>>();
            byte[] fileData;

            if (filepath.Length > 0)
            {
                int visualizerIndex = 0;

                if (loadAllImages)
                {
                    foreach (string path in filepath)
                    {
                        fileData = File.ReadAllBytes(path);
                        Texture2D tmp = new Texture2D(500, 500, TextureFormat.DXT1, false);
                        tmp.LoadImage(fileData);

                        if (visualize)
                        {
                            visualizers[visualizerIndex % visualizers.Count].ApplyTexture(tmp);
                    
                            visualizerIndex++;
                        }
                
                        images.Add(ConvertTexture2DIntoFloatList(tmp));
                    }
                }
                else
                {
                    List<int> usedIndexes = new List<int>();
                    for(int i = 0; i < nbImagesToLoad; ++i)
                    {
                        string path;
                        if(loadRandomImages)
                        {
                            int id = 0;
                            do
                            {
                                id = UnityEngine.Random.Range(0, filepath.Length);
                                path = filepath[id];
                            } 
                            while (usedIndexes.Contains(id));

                            usedIndexes.Add(id);
                        }
                        else
                        {
                            path = filepath[i];
                        }
                        Debug.Log(path);
                        fileData = File.ReadAllBytes(path);
                        Texture2D tmp = new Texture2D(500, 500, TextureFormat.DXT1, false);
                        tmp.LoadImage(fileData);

                        if (visualize)
                        {
                            visualizers[i % visualizers.Count].ApplyTexture(tmp);
                        }
                
                        images.Add(ConvertTexture2DIntoFloatList(tmp));
                    }
                }
            }
        
            return images;
        }

        private List<float> ConvertTexture2DIntoFloatList(Texture2D tex)
        { 
            List<float> pixels = new List<float>();

            Color[] pixs = tex.GetPixels();

            if (type == TypeImage.Color || type == TypeImage.Color4X4 || type == TypeImage.Color16X16)
            {
                foreach (var pixel in pixs)
                { 
                    pixels.Add(pixel.r);    
                    pixels.Add(pixel.g);    
                    pixels.Add(pixel.b);    
                }
            }
            else
            {
                foreach (var pixel in pixs)
                { 
                    pixels.Add(pixel.r);  
                }
            }
            
            return pixels;
        }

        public void InitializeImages()
        {
            ClearImages();
            images = new List<List<float>>();
            images3DOrReal = new List<int>();
        
            ImportImages();
        }

        public void ClearImages()
        {
            if(images != null)
                images.Clear();
            if (images3DOrReal != null)
                images3DOrReal.Clear();
        }

        public static List<List<float>> GetAllImages()
        {
            return images;
        }

        public static List<int> GetAllValues3dOrReal()
        {
            return images3DOrReal;
        }

        public static List<float> GetImageByIndex(int index)
        {
            return images[index];
        }
    }
}
