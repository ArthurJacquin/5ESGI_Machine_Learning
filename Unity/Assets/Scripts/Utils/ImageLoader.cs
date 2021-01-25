using System;
using System.Collections.Generic;
using System.IO;
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

        private static ImageLoader _instance;
        private List<List<float>> _images;
        private void Awake()
        {
            Debug.Log("Awake");

            if (_instance == null)
                _instance = this;
            if (_images == null)
                InitializeImages();

            updateEditor = false;
        }
    
        public static ImageLoader GetInstance()
        {
            return _instance;
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
            
            /*switch (type)
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
            */
            string[] files = System.IO.Directory.GetFiles(path, "*.jpg");
            //string[] files3D = System.IO.Directory.GetFiles(path3D, "*.jpg");

            List<List<float>> imagesReal = LoadImagesFromPath(files);
            //List<List<float>> images3D = LoadImagesFromPath(files3D);

            foreach (var image in imagesReal)
            {
                _images.Add(image);
            }

            //foreach (var image in images3D)
            //{
            //    _images.Add(image);
            //}
        
            Debug.Log("Found images : " + _images.Count);
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
                    for(int i = 0; i < nbImagesToLoad; ++i)
                    {
                        string path = filepath[i];
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
       
            foreach (var pixel in pixs)
            { 
                pixels.Add(pixel.r);    
                pixels.Add(pixel.g);    
                pixels.Add(pixel.b);    
            }
        
            return pixels;
        }

        public void InitializeImages()
        {
            ClearImages();
            _images = new List<List<float>>();
        
            ImportImages();
        }

        public void ClearImages()
        {
            if(_images != null)
                _images.Clear();
        }

        public List<List<float>> GetAllImages()
        {
            return _instance._images;
        }

        public List<float> GetImageByIndex(int index)
        {
            return _instance._images[index];
        }
    }
}
