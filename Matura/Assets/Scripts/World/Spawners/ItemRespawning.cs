using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawning : MonoBehaviour
{
    // public ItemLifecycleManager lifecycleManager;
    private Coroutine _currentRespawnCoroutine;
    private Dictionary<Item, Coroutine> _currentRespawnCoroutines = new Dictionary<Item, Coroutine>();

    private ItemSpawner _currentSpawnerScript;

    private void Awake()
    {
        _currentSpawnerScript = GetComponent<ItemSpawner>();    
    }

    [ContextMenu("Handling Respawn")]
    public void HandleItemRespawn(Item currentItemToRespawn)
    {
        currentItemToRespawn.gameObject.SetActive(false);
        if (!_currentRespawnCoroutines.ContainsKey(currentItemToRespawn))
            RespawnItem(currentItemToRespawn);

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
            itemScript.gameObject.transform.position = _currentSpawnerScript.SetRandomSpawnPosition(); 
            itemScript.gameObject.SetActive(true);
        }
        else
        {
            // Instantiate new Item at random position
            _currentSpawnerScript.SpawnItemRandomly(itemScript.gameObject);
        }

        if (_currentRespawnCoroutines.ContainsKey(itemScript))
            _currentRespawnCoroutines.Remove(itemScript);
    }
}
