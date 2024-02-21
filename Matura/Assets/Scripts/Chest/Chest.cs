using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject _chestUIPrefab;
    [SerializeField] private Transform _chestUIParent;

    [HideInInspector] public List<Slot> allChestSlots = new List<Slot>();
    [HideInInspector] public GameObject chestInstantiatedParent;

    [Header("Loot tables")]
    [SerializeField] private bool _randomLoot; 
    [SerializeField] private LootTable _lootTable;

    private void Start()
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

        //Loot table

        if (_randomLoot)
        {
            _lootTable.InitialiseLootTable();
            _lootTable.SpawnLoot(allChestSlots);
        }
    }
}
