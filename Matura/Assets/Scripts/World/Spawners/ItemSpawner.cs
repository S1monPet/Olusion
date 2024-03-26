using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnerTransform; 
    [SerializeField] private int proximity;
    [SerializeField] private int amountOfSpawningItems;

    [Header("Offset's for spawning")]
    [SerializeField] private float zOffset;
    [SerializeField] private float yOffset;

    public List<SpawningItems> spawningItems = new List<SpawningItems>();


    // Multiple item spawning, usually at the start of the program
    protected virtual void SpawnItemsRandomly()
    {
        foreach (SpawningItems item in spawningItems)
        {
            // Setting maximum amount of item's that can be spawned
            amountOfSpawningItems = Random.Range(1, item.MaxSpawningItems + 1); // Range goes until that number, so we add + 1 to include that number.

            while (amountOfSpawningItems > 0)
            {
                Vector3 randomSpawnPosition = SetRandomSpawnPosition(); 

                Instantiate(item.SpawningItem, randomSpawnPosition, Quaternion.identity, spawnerTransform);

                amountOfSpawningItems--; 
            }
        }
    }

    public Vector3 SetRandomSpawnPosition()
    {
        Vector3 randomSpawnPosition = Random.insideUnitSphere * proximity;
        randomSpawnPosition += spawnerTransform.position; // Spawning around spawner
        // Little offset so it doesn't get spawned behind the well, or in the ground
        randomSpawnPosition.z -= zOffset;
        randomSpawnPosition.y += yOffset;

        return randomSpawnPosition; 
    }
    
    // For single item spawning
    public void SpawnItemRandomly(GameObject itemToSpawn)
    {
        Vector3 randomSpawnPosition = SetRandomSpawnPosition(); 

        Instantiate(itemToSpawn, randomSpawnPosition, Quaternion.identity, spawnerTransform);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnerTransform.position, proximity);   
    }
}

[System.Serializable]
public class SpawningItems
{
    public GameObject SpawningItem;
    public int MaxSpawningItems; 
}
