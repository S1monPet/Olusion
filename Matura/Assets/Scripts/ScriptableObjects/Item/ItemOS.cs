using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOS : ScriptableObject
{
    [field: SerializeField] public new string name; //Name of item; 
    [field: SerializeField] public string description;
    [field: SerializeField] public Sprite icon;
    [field: SerializeField] public float AttackSpeed;
    [field: SerializeField] public int currentQuantity = 1;
    [field: SerializeField] public int maxQuantity = 16;
    [field: SerializeField] public int Damage;
    [field: SerializeField] public bool CanBeHeld;

    public WaitForSeconds CoolDownWait { get; private set; }

    [Header("Hotbar")]
    [field: SerializeField] public int equiappableItemIndex = -1;

    private void OnEnable()
    {
        CoolDownWait = new WaitForSeconds(AttackSpeed);
    }
}
