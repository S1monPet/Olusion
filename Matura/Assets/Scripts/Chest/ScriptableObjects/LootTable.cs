using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ChestLootTable", menuName = "Inventory/ChestLootTable", order = 1)]
public class LootTable : ScriptableObject
{
    [System.Serializable]

    public class LootItem 
    {
        public GameObject ItemPrefab;
        public int MinSpawn;
        public int MaxSpawn;
        public string Name; //Setting name in hiearchy
        [Range(0f, 100f)] public float SpawnChance; 
    }

    public List<LootItem> lootItems = new List<LootItem>();
    [Range(0, 100)] public int SpawnChancePerSlot = 20; 

    public void InitialiseLootTable()
    {
        float totalSpawnChance = 0f; 
        foreach (LootItem item in lootItems)
        {
            totalSpawnChance += item.SpawnChance; //Setting spawn chance to each item
        }
        if (totalSpawnChance > 100f)
        {
            NormaliseSpawnChances();  //If spawn chance is bigger than 100f if there is two items that are the same we do 50 / 50 
        }
    }

    private void NormaliseSpawnChances()
    {
        float normalisationFactor = 100f / CalculateTotalSpawnChance(); 
        foreach (LootItem item in lootItems)
        {
            item.SpawnChance *= normalisationFactor;
        }
    }

    private float CalculateTotalSpawnChance()
    {
        float totalSpawnChance = 0f; 
        foreach (LootItem item in lootItems)
        {
            totalSpawnChance += item.SpawnChance;
        }
        return totalSpawnChance;
    }

    public void SpawnLoot(List<Slot> allChestSlots)
    {
        foreach(Slot chestSlot in allChestSlots)
        {
            if (Random.Range(0f, 100f) <= SpawnChancePerSlot)
            {
                SpawnRandomItem(chestSlot);
            }
        }
    }  

    private void SpawnRandomItem(Slot slot)
    {
        LootItem choosenItem = ChooseRandomItem();
        if (choosenItem != null)
        {
            int spawnCount = Random.Range(choosenItem.MinSpawn, choosenItem.MaxSpawn);

            GameObject spawnedItem = Instantiate(choosenItem.ItemPrefab, Vector3.zero, Quaternion.identity); //Optimise plis
            spawnedItem.name = choosenItem.Name; //Set name
            spawnedItem.SetActive(false);

            Item itemComponent = spawnedItem.GetComponent<Item>();
            if (itemComponent != null) 
                itemComponent.currentQuantity = spawnCount;

            slot.SetItem(itemComponent);
            slot.UpdateInventoryAmount();
        }
    }

    private LootItem ChooseRandomItem()
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (LootItem item in lootItems)
        {
            cumulativeChance += item.SpawnChance; 
            if (randomValue <= cumulativeChance)
            {
                return item; 
            }
        }
        return null;
    }

}
