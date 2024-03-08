using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{

    [Header("Timer")]
    public float itemRespawnTimer;
    private WaitForSeconds _itemRespawnTimer;

    private void OnEnable()
    {
        _itemRespawnTimer = new WaitForSeconds(itemRespawnTimer);
    }
    private void SpawnItemsRandomly()
    {

    }

    public void RespawnItem()
    {
        StartCoroutine(RespawnItemCoroutine());
    }

    private IEnumerator RespawnItemCoroutine()
    {
        yield return _itemRespawnTimer;
    }

}
