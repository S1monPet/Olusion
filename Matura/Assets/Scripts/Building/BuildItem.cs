using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public sealed class BuildItem : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject _enableBuilding;

    [Header("Scripts")]
    [SerializeField] private Inventory inventoryScript;

    [SerializeField] private Transform buildingLocation; // Where we will be building

    private Item _currentHeldItem; 

    private void Update()
    {
        CanBuildItem();
    }

    private void CanBuildItem()
    {
        _currentHeldItem = inventoryScript.hotbarSlots.Select(slot => slot.GetItem()).FirstOrDefault(item => item != null && item.IsHeld);

        // Building item 
        if (_currentHeldItem != null) 
        {
            _enableBuilding.SetActive(_currentHeldItem.buildingItem);
        }
        else
        {
            _enableBuilding.SetActive(false);   
        }
    }

    public void OnBuildButtonPress()
    {
        _currentHeldItem.transform.position = buildingLocation.position; 
        _currentHeldItem.transform.rotation = buildingLocation.rotation;

        _currentHeldItem.gameObject.SetActive(true);
    }
}
