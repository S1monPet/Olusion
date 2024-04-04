using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settings 
{
    // Settings
    public bool SoundEnabled; 

    public Settings() 
    {
        this.SoundEnabled = true; 
    }
}

[System.Serializable]
public class GameData
{
    public int Health;
    public int Food;
    public int Water; 
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    public AllItemData allItemData; 

    public InventoryData inventoryData;
    public AllChestData allChestData;

    // For clothes
    public ActiveWear activeWear; 


    public GameData()
    {
        this.Health = 100;
        this.Food = 60;
        this.Water = 40; 
        this.playerPosition = Vector3.zero;
        this.playerRotation = Quaternion.identity;

        this.allItemData = new AllItemData();
        this.inventoryData = new InventoryData();
        this.allChestData = new AllChestData();
    }
}

// Item
[System.Serializable] 
public class ItemData
{
    public Vector3 currentItemPosition;
    public Quaternion currentItemRotation;
    public ItemData(Vector3 currentItemPosition, Quaternion currentItemRotation)
    {
        this.currentItemPosition = currentItemPosition;
        this.currentItemRotation = currentItemRotation;
    }
}

[System.Serializable]
public class AllItemData
{
    public List<ItemData> items = new List<ItemData>();    
}

//Inventory 
[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int quantity;
    public int slotIndex;

    public InventoryItemData(string itemName, int quantity, int slotIndex)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.slotIndex = slotIndex;
    }
}

[System.Serializable]

public class InventoryData
{
    public List<InventoryItemData> slotData = new List<InventoryItemData>(); 
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
    public List<ChestItemData> chestData = new List<ChestItemData>();
    public Vector3 chestPosition;
    public ChestData(Vector3 chestPosition)
    {
        this.chestPosition = chestPosition;
    }
}

[System.Serializable]
public class AllChestData
{
    public List<ChestData> allChestDataList = new List<ChestData>();
}

[System.Serializable]
public class ClothingItem
{

}


[System.Serializable]
public class ActiveWear
{
    public List<ClothingItem> clothingItems = new List<ClothingItem>(); 
}