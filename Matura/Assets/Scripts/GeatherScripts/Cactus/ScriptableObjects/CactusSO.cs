using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gathering", menuName = "Gathering/CactusGathering", order = 1)]

public class CactusSO : ScriptableObject
{
    [field: SerializeField] public string Tag; //Name of item; 
    [field: SerializeField] public int CactusHealth;
    [field: SerializeField] public int SpawningCactusHealth;
    [field: SerializeField] public float RespawnTime;

    // public List<ItemDrop> ItemDrops = new List<ItemDrop>();
    public ItemDrop itemDrop; 
    public WaitForSeconds RespawnTimer { get; private set; }

    private void OnEnable()
    {
        RespawnTimer = new WaitForSeconds(RespawnTime);
    }
}
