using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    { 
        //Object pooling
        pooledObjects = new List<GameObject>();
        GameObject enemy;
        for (int i = 0; i < amountToPool; i++)
        {
            enemy = Instantiate(objectToPool);
            enemy.SetActive(false);
            pooledObjects.Add(enemy);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null; 
    }
}
