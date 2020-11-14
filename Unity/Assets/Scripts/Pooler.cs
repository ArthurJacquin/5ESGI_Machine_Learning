using System;
using System.Collections.Generic;
using UnityEngine;

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
    public SpherePoolItem itemsToPool;

    public static List<GameObject> PooledObj;

    private void Awake()
    {
        if (PooledObj == null)
        {
            InitializePool();
        }
    }

    private void Update()
    {
        if (PooledObj == null)
        {
            InitializePool();
        }
    }

    public void InitializePool()
    {
        DestroyChildren();
        PooledObj = new List<GameObject>();
        
        ObjectPoolItemToPooledObject();
    }

    private void ObjectPoolItemToPooledObject()
    {
        var item = itemsToPool;

        for (int i = 0; i < item.amount; ++i)
        {
            GameObject obj = (GameObject) Instantiate(item.poolObj, this.transform, true);
            obj.SetActive(false);
            PooledObj.Add(obj);
        }
    }

    private void DestroyChildren()
    {
        int nbChild = this.transform.childCount;
        for (int i = 0; i < nbChild; ++i)
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
    }
}
