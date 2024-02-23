using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    [SerializeField] private ItemOS itemStats; // Reference to the ScriptableObject
    public ItemOS ItemStats => itemStats; // Public accessor for other scripts

    private int currentQuantity { get; set; }


    private void Awake()
    {
        currentQuantity = 1; 
    }

    public string GetItemName()
    {
        return itemStats.name;
    }

    public string GetItemDescription()
    {
        return itemStats.description;
    }

    public Sprite GetItemIcon()
    {
        return itemStats.icon;
    }

    public int GetCurrentQuantity()
    {
        return currentQuantity;
    }

    public void DecreaseQuantity(int amount)
    {
        currentQuantity -= amount;
    }

    public void IncreaseCurrentQuanitity(int currentQuantity)
    {
        currentQuantity += currentQuantity;
    }

    public void SetCurrentQuantity(int quantity)
    {
        currentQuantity = quantity;
    }

    public void ResetCurrentQuantity()
    {
        currentQuantity = 0;
    }

    public int GetMaxQuantity()
    {
        return itemStats.maxQuantity;
    }

    public int GetItemDamage()
    {
        return itemStats.Damage;
    }

    public bool IsItemHeld()
    {
        return itemStats.IsHeld;
    }

    public void UpdateItemHeldBool(bool IsHeld)
    {
        itemStats.IsHeld = IsHeld;
    }

    public int GetEquipableItemIndex()
    {
        return itemStats.equiappableItemIndex;
    }
}
