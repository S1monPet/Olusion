using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGathering : GatheringBase
{
    [SerializeField] private TreeSO tree;
    public TreeSO Tree => tree;
    public Axe axeScript;

    private void OnEnable()
    {
        axeScript.CanGather = true;
        InitialiseTree(); 
    }


    private void InitialiseTree()
    {
        Tree.TreeHealth = Tree.SpawningTreeHealth;
    }

    public void Gather(int damage, GameObject player, string gatheringTool, WaitForSeconds gatheringRate)
    {
        if (IsToolEquiped(gatheringTool))
        {
            axeScript.CanGather = false;
            base.Gather(damage, player, Tree.TreeHealth, Tree.ItemDrops, gatheringRate, Tree.RespawnTimer);
        }

    }

    protected override IEnumerator GatheringCourotine(int damage, WaitForSeconds timeToGather, int amountOfItems, ItemDrop itemToDrop, WaitForSeconds respawnTimer)
    {
        for (int i = 0; i != amountOfItems; i++)
        {
            playerAnimator.Play("Attack");

            yield return timeToGather;

            Tree.TreeHealth -= damage; //Set tree health

            if (Tree.TreeHealth <= 0) 
                base.RespawnGatherableItem(respawnTimer);

            //Expensive
            Item droppedItem = Instantiate(itemToDrop.ItemToDrop, transform.position, Quaternion.identity).GetComponent<Item>();
            droppedItem.currentQuantity = 1;

            inventoryScript.AddItemToInventory(droppedItem);
        }
        base.RespawnGatherableItem(respawnTimer); //Was destroyed before
        InitialiseTree();

        playerAnimator.Play("Attack");
    }


    public override void StopGathering()
    {
        base.StopGathering();

        axeScript.CanGather = true; //Enable gathering another object again
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
