using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGathering : GatheringBase
{
    public Axe axeScript;

    private void OnEnable()
    {
        axeScript.CanGather = true; 
    }

    public void Gather(int damage, GameObject player, string gatheringTool, WaitForSeconds gatheringRate)
    {
        if (IsToolEquiped(gatheringTool))
        {
            axeScript.CanGather = false; 
            base.Gather(damage, player, Tree.TreeHealth, Tree.ItemDrops, gatheringRate, Tree.RespawnTimer);
        }

    }

    public void StopGathering()
    {
        base.StopGatheringCourotine(); 
    }

    private bool IsToolEquiped(string requiredTool)
    {
        //Because it has to be in the hotbar for you to hold it
        foreach (Slot hotbarSlot in inventoryScript.hotbarSlots)
        {
            //Let's check if harvestable item is equal to, item that we need
            if (hotbarSlot.HasItem() && hotbarSlot.GetItem() != null)
            {
                Item currentItem = hotbarSlot.GetItem();
                if (currentItem.IsHeld && currentItem.name == requiredTool)
                    return true; 
            }
        }
        return false; 
    }
}
