using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [System.Serializable]
    public class VisualizerPoolItem
    {
        public TestImportImage poolObj;
        public int amount;

        public VisualizerPoolItem(TestImportImage obj, int amt)
        {
            poolObj = obj;
            amount = amt;
        }
    }

    [ExecuteInEditMode]
    public class Visualizers : MonoBehaviour
    {
        private static Visualizers _instance;

        public VisualizerPoolItem itemsToPool;
        private List<TestImportImage> _pooledObj;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        
            if (_pooledObj == null)
            {
                InitializePool();
            }
        }
    
        public static Visualizers GetInstance()
        {
            return _instance;
        }
    
        private void Update()
        {
            if (_instance == null)
                _instance = this;
        
            if (_pooledObj == null)
            {
                InitializePool();
            }
        }
    
        private void ObjectPoolItemToPooledObject()
        {
            var item = itemsToPool;
            var maxInARow = 10;
        
            for (var i = 0; i < item.amount; ++i)
            {
                var row = (i % maxInARow) - 3.5f;
                var col = (i / maxInARow) - 5.0f;
                var obj = (TestImportImage) Instantiate(item.poolObj, this.transform, true);
                obj.gameObject.SetActive(false);
                obj.transform.position = new Vector3(col, row, 0.0f);
                _pooledObj.Add(obj);
            }
        }
    
        private void DestroyChildren()
        {
            int nbChild = this.gameObject.transform.childCount;
            for (int i = 0; i < nbChild; ++i)
            {
                DestroyImmediate(this.gameObject.transform.GetChild(0).gameObject);
            }
        }
    
        public void InitializePool()
        {
            DestroyChildren();
            _pooledObj = new List<TestImportImage>();
        
            ObjectPoolItemToPooledObject();
        }

        public List<TestImportImage> GetPoolList()
        {
            return _pooledObj;
        }
    }
}