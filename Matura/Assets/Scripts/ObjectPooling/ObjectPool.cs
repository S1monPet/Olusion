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
        GameObject currentObject;
        for (int i = 0; i < amountToPool; i++)
        {
            currentObject = Instantiate(objectToPool);
            SpawnEnemies(currentObject);
        }
    }

    private void SpawnEnemies(GameObject parentEnemy)
    {
        if (parentEnemy.transform.childCount > 0)
        {
            GameObject enemy = parentEnemy.transform.GetChild(0).gameObject;
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                Vector3 spawnPosition = enemyBase.SpawnPosition();

                parentEnemy.transform.position = spawnPosition; 
                parentEnemy.transform.rotation = Quaternion.identity;

                parentEnemy.SetActive(false); //Disabling enemy 
                pooledObjects.Add(enemy);

            }
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
