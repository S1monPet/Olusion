using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    Food, Water
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

    [Header("Consumable")]
    public bool Consumable = false;
    public float TimeToConsume = 1;
    public ConsumableType type;
    public int Amount;
}
