using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemLifecycleManager : MonoBehaviour
{
    public Transform spawningZone; 
    public List<Transform> spawningZones = new List<Transform>();
    public GameObject item;
    public GameObject appleTree; 

    public int MaximumAmountOfItems, MinimumAmountOfItems;
    private int _randomAmountOfItemsToSpawn; 

    private Vector3 _randomSpawnPosition = Vector3.zero;
    private Vector3 _spawnerSize = Vector3.zero;

    private Coroutine _currentRespawnCoroutine; 
    private Dictionary<Item, Coroutine>  _currentRespawnCoroutines = new Dictionary<Item, Coroutine>();

    [Header("Math")]
    [SerializeField] private readonly float sin60 = Mathf.Sin(Mathf.PI / 3f);

    private void Start()
    {
        _randomAmountOfItemsToSpawn = Random.Range(MinimumAmountOfItems, MaximumAmountOfItems);
        SpawnItemsRandomly(); 
    }

    private void OnEnable()
    {
        SpawnItemsRandomly(); 
    }

    public void SpawnItemsRandomly()
    {
        Transform currentSpawningZone = null;
        int numberOfSpawningZones = spawningZones.Count;
        for (int i = 0; i < _randomAmountOfItemsToSpawn; i++)
        {
            int currentSpawningZoneIndex = Random.Range(0, numberOfSpawningZones);

            currentSpawningZone = spawningZones[currentSpawningZoneIndex];
            _spawnerSize = currentSpawningZone.localScale; // The scale of the parentTransform is used as the size of the box

            float randomX = Random.Range(-_spawnerSize.x / 2, _spawnerSize.x / 2);
            float randomY = Random.Range(-_spawnerSize.y / 2, _spawnerSize.y / 2);
            float randomZ = Random.Range(-_spawnerSize.z / 2, _spawnerSize.z / 2);

            Vector3 _randomSpawnPosition = currentSpawningZone.position + new Vector3(randomX, randomY, randomZ);

            GameObject spawnedItem = Instantiate(item, _randomSpawnPosition, Quaternion.identity, spawningZone.parent.parent); // Items will be spawned in Apple Tree

            //Setting respawnable mode 
            Item itemScript = spawnedItem.GetComponent<Item>();
            itemScript.Respawnable = true; 
        }
    }
    // If Item was taken we respawn only that item
    private void SpawnItemRandomly()
    {
        Transform currentSpawningZone = null;
        int numberOfSpawningZones = spawningZones.Count;

        int currentSpawningZoneIndex = Random.Range(0, numberOfSpawningZones);

        currentSpawningZone = spawningZones[currentSpawningZoneIndex];
        _spawnerSize = currentSpawningZone.localScale; // The scale of the parentTransform is used as the size of the box

        float randomX = Random.Range(-_spawnerSize.x / 2, _spawnerSize.x / 2);
        float randomY = Random.Range(-_spawnerSize.y / 2, _spawnerSize.y / 2);
        float randomZ = Random.Range(-_spawnerSize.z / 2, _spawnerSize.z / 2);

        Vector3 _randomSpawnPosition = currentSpawningZone.position + new Vector3(randomX, randomY, randomZ);

        GameObject spawnedItem = Instantiate(item, _randomSpawnPosition, Quaternion.identity, spawningZone.parent.parent); // Items will be spawned in Apple Tree

        //Setting respawnable mode 
        Item itemScript = spawnedItem.GetComponent<Item>();
        itemScript.Respawnable = true;
        
    }

    private void RespawnItem(GameObject item)
    {
        int currentSpawningZoneIndex = Random.Range(0, spawningZones.Count);
        Transform currentSpawningZone = spawningZones[currentSpawningZoneIndex];

        _spawnerSize = currentSpawningZone.localScale; // The scale of the parentTransform is used as the size of the box

        float randomX = Random.Range(-_spawnerSize.x / 2, _spawnerSize.x / 2);
        float randomY = Random.Range(-_spawnerSize.y / 2, _spawnerSize.y / 2);
        float randomZ = Random.Range(-_spawnerSize.z / 2, _spawnerSize.z / 2);

        Vector3 _randomSpawnPosition = currentSpawningZone.position + new Vector3(randomX, randomY, randomZ);

        item.transform.position = _randomSpawnPosition; 
    }

    public void HandleItemRespawn(Item currentItem)
    {
        if (!_currentRespawnCoroutines.ContainsKey(currentItem) && appleTree.activeSelf)
            RespawnItem(currentItem);

    }
    private void RespawnItem(Item itemScript)
    {
        _currentRespawnCoroutine = StartCoroutine(RespawnItemCoroutine(itemScript));
        _currentRespawnCoroutines[itemScript] = _currentRespawnCoroutine; 
    }

    private IEnumerator RespawnItemCoroutine(Item itemScript)
    {
        yield return new WaitForSeconds(itemScript.RespawnTimer);

        if (itemScript != null)
        {
            RespawnItem(itemScript.gameObject); // For spawning in random position
            itemScript.gameObject.SetActive(true);
        }
        else
        {
            // Instantiate new Item at random position
            SpawnItemRandomly();
        }

        if (_currentRespawnCoroutines.ContainsKey(itemScript))
            _currentRespawnCoroutines.Remove(itemScript);
    }
}
