using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public int Health;
    public int Food;
    public int Water; 
    public Vector3 playerPosition; 

    public InventoryData inventoryData;
    public AllChestData allChestData;


    public GameData()
    {
        this.Health = 100;
        this.Food = 60;
        this.Water = 40; 
        this.playerPosition = Vector3.zero; 

        this.inventoryData = new InventoryData();
        this.allChestData = new AllChestData(); 
    }
}

//Inventory 
[System.Serializable]
public class ItemData
{
    public string itemName;
    public int quantity;
    public int slotIndex;

    public ItemData(string itemName, int quantity, int slotIndex)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.slotIndex = slotIndex;
    }
}

[System.Serializable]

public class InventoryData
{
    public List<ItemData> slotData = new List<ItemData>(); 
}


//Chest
[System.Serializable]
public class ChestItemData
{
    public string itemName;
    public int quantity;    
    public int slotIndex;

    public ChestItemData(string itemName, int quantity, int slotIndex)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.slotIndex = slotIndex;
    }
}

[System.Serializable]
public class ChestData
{
    public List<ChestItemData> slotData = new List<ChestItemData>();
}

[System.Serializable]
public class AllChestData
{
    public List<ChestData> allChestDataList = new List<ChestData>();
}