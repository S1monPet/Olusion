using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ChestType
{
    TreasureChest,
    DroppedChest,
    LootChest,
}


public class Chest : MonoBehaviour, IDataPersistance
{
    [SerializeField] private GameObject _chestUIPrefab;
    [SerializeField] private Transform _chestUIParent;

    [HideInInspector] public List<Slot> allChestSlots = new List<Slot>();
    [HideInInspector] public GameObject chestInstantiatedParent;

    [Header("Loot tables")]
    [SerializeField] private bool _randomLoot; 
    [SerializeField] private LootTable _lootTable;

    [Header("Save/Load")]
    public ChestType chestType; 
    public Transform dropLocation; 
    public List<GameObject> _allChestItemPrefabs = new List<GameObject>();

    public void LoadData(GameData data)
    {
        if (data.allChestData.allChestDataList.Count != 0)
        {
            CreateChestSlots();
            LoadSavedChestData(data.allChestData);

            //If object was disabled it wouldn't load data
            if (chestType == ChestType.DroppedChest)
                gameObject.SetActive(false);

            return;
        }

        CreateChestSlots();
        SpawnRandomChestItems();

        //If object was disabled it wouldn't make data
        if (chestType == ChestType.DroppedChest)
            gameObject.SetActive(false);

    }

    public void SaveData(ref GameData data)
    {
        SaveChestData(data); 
    }

    private void SaveChestData(GameData data)
    {
        ChestData chestData = new ChestData(); 
        foreach (Slot slot in allChestSlots)
        {
            Item item = slot.GetItem();
            if (item != null)
            {
                ChestItemData chestItemData = new ChestItemData(item.name, item.currentQuantity, allChestSlots.IndexOf(slot));
                chestData.slotData.Add(chestItemData);
            }
        }
        data.allChestData.allChestDataList.Add(chestData); //Adding chest

    }

    private void LoadSavedChestData(AllChestData allChestData)
    {
        ClearChest();

        ChestData chestData = allChestData.allChestDataList[0];
        allChestData.allChestDataList.Remove(chestData);

        foreach (ChestItemData chestItemData in chestData.slotData)
        {
            //Getting item by name 
            GameObject itemPrefab = _allChestItemPrefabs.Find(prefab => prefab.GetComponent<Item>().name == chestItemData.itemName);

            if (itemPrefab != null)
            {
                GameObject createdItem = Instantiate(itemPrefab, dropLocation.position, Quaternion.identity);
                Item item = createdItem.GetComponent<Item>();

                item.currentQuantity = chestItemData.quantity;

                //Debug.Log(item + chestItemData.slotIndex.ToString());
                AddItemToChest(item, chestItemData.slotIndex);
            }
        }
        

        foreach (Slot slot in allChestSlots)
        {
            slot.UpdateInventoryAmount();
        }
    }

    private void ClearChest()
    {
        foreach (Slot slot in allChestSlots)
        {
            slot.SetItem(null); 
        }
    }

    public void AddItemToChest(Item itemToAdd, int overrideIndex = -1)
    {
        if (overrideIndex != -1)
        {
            allChestSlots[overrideIndex].SetItem(itemToAdd);
            itemToAdd.gameObject.SetActive(false);

            allChestSlots[overrideIndex].UpdateInventoryAmount();
            return;
        }
    }

    public void CreateChestSlots()
    {
        GameObject chestSlots = Instantiate(_chestUIPrefab, _chestUIParent.position, _chestUIParent.rotation, _chestUIParent);

        foreach (Transform childSlot in chestSlots.transform.GetChild(1))
        {
            Slot childSlotScript = childSlot.GetComponent<Slot>();
            allChestSlots.Add(childSlotScript);

            childSlotScript.InitialiseSlot();
        }

        chestInstantiatedParent = chestSlots;
        chestInstantiatedParent.SetActive(false);
    }

    public void SpawnRandomChestItems()
    {
        //Loot table

        if (_randomLoot)
        {
            _lootTable.InitialiseLootTable();
            _lootTable.SpawnLoot(allChestSlots);
        }
    }
}
