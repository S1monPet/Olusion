using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    Food, Water
};

public enum HealingType
{
    MedKit, Bandage
};

public enum ClothingType
{
    Armor, Shirt, Pants, FaceMask
};

public class Item : MonoBehaviour
{
    public new string name = "New item"; //Name of item; 
    public string description = "New Description";
    public Sprite icon;
    public int currentQuantity = 1;
    public int maxQuantity = 16;
    public int Damage;
    public bool IsHeld = false;


    [Header("Hotbar")]
    public int equiappableItemIndex = -1;

    [Header("Armor")]
    public int equiappableArmorIndex = -1;
    public int damageReduction;

    [Header("Consumable")]
    public bool Consumable = false;
    public float TimeToConsume = 1;
    public ConsumableType type;
    public int Amount;

    [Header("Healing")]
    public bool Healing = false;
    public int HealthGain = 10;
    public float TimeToGainHealth = 2f;
    public HealingType healingType; 

    [Header("Gear & Clotches")]
    public bool Wearable = false;
    public ClothingType clothingType; 
}
