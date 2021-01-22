using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SpherePoolItem
{
    public GameObject poolObj;
    public int amount;

    public SpherePoolItem(GameObject obj, int amt)
    {
        poolObj = obj;
        amount = amt;
    }
}

[ExecuteInEditMode]
public class Pooler : MonoBehaviour
{
    private static Pooler _instance;
    
    public SpherePoolItem itemsToPool;
    private List<GameObject> _pooledObj;
    
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        if (_pooledObj == null)
        {
            InitializePool();
        }
    }
    
    public static Pooler GetInstance()
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

        for (int i = 0; i < item.amount; ++i)
        {
            var obj = (GameObject) Instantiate(item.poolObj, this.transform, true);
            obj.SetActive(false);
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
        _pooledObj = new List<GameObject>();
        
        ObjectPoolItemToPooledObject();
    }

    public List<GameObject> GetPoolList()
    {
        return _pooledObj;
    }

}
