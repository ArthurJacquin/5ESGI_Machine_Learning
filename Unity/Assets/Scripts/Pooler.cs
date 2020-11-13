using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpherePoolItem
{
    public GameObject poolObj;
    public int amount;
    public bool shouldExpand = true;

    public SpherePoolItem(GameObject obj, int amt, bool exp = true)
    {
        poolObj = obj;
        amount = amt;
        shouldExpand = exp;
    }
}

public class Pooler : MonoBehaviour
{
    public List<SpherePoolItem> itemsToPool;

    public List<GameObject> pooledObj;
    private List<int> _positions;
    
    public void InitializePool()
    {
        pooledObj = new List<GameObject>();
        _positions = new List<int>();

        for (int i = 0; i < itemsToPool.Count; ++i)
        {
            ObjectPoolItemToPooledObject(i);
        }
    }

    public List<GameObject> GetAllPooledObjects(int index)
    {
        return pooledObj;
    }

    private void ObjectPoolItemToPooledObject(int index)
    {
        
    }
    
    public void DestroyPool()
    {
        
    }
}
