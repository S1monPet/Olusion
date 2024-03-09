using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledObjects;
    public List<GameObject> objectsToPool;
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
        for (int i = 0; i < objectsToPool.Count; i++)
        {
            for (int j = 0; j < amountToPool; j++) 
            {
                currentObject = Instantiate(objectsToPool[i]);
                SpawnEnemies(currentObject);
            }
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
    /*
    private void SpawnTrees(GameObject tree)
    {
        Tree treeScript = tree.GetComponent<Tree>();
        if (treeScript != null)
        {
            Vector3 spawnPosition = treeScript.SpawnTree();

            tree.transform.position = spawnPosition;
            tree.transform.rotation = Quaternion.identity;

            tree.SetActive(true); 
            pooledObjects.Add(tree); 
        }
    }
    */
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
