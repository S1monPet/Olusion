using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class BaseItem : MonoBehaviour
{
    [SerializeField] private ItemOS itemStats; // Reference to the ScriptableObject
    public ItemOS ItemStats => itemStats; // Public accessor for other scripts
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
        return itemStats.currentQuantity;
    }

    public void DecreaseQuantity(int amount)
    {
        itemStats.currentQuantity -= amount;
    }

    public void IncreaseCurrentQuanitity(int currentQuantity)
    {
        itemStats.currentQuantity += currentQuantity;
    }

    public void SetCurrentQuantity(int quantity)
    {
        itemStats.currentQuantity = quantity;
    }

    public void ResetCurrentQuantity()
    {
        itemStats.currentQuantity = 0;
    }

    public int GetMaxQuantity()
    {
        return itemStats.maxQuantity;
    }

    public int GetItemDamage()
    {
        return itemStats.Damage;
    }

    public bool CanItemBeHeld()
    {
        return itemStats.CanBeHeld;
    }

    public int GetEquipableItemIndex()
    {
        return itemStats.equiappableItemIndex;
    }
}
