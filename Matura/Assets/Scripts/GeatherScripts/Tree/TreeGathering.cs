using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeGathering : GatheringBase
{
    [SerializeField] private TreeSO tree;
    public TreeSO Tree => tree;

    private void OnEnable()
    {
        PlayerGather.CanGather = true;
        InitialiseTree(); 
    }


    private void InitialiseTree()
    {
        Tree.TreeHealth = Tree.SpawningTreeHealth;
        
        // Using foreach loop, because lambda's are used for querying or returing results from collections
        foreach (ItemDrop itemDrop in Tree.ItemDrops)
        {
            // Setting each item to it's max dropping amount
            itemDrop.CurrentAmountOfItems = itemDrop.StartingAmountOfItems; 
        }
    }

    public void Gather(int damage, GameObject player, WaitForSeconds gatheringRate)
    {
        PlayerGather.CanGather = false;
        base.Gather(damage, player, Tree.TreeHealth, Tree.ItemDrops, gatheringRate, Tree.RespawnTimer);

    }


    protected override IEnumerator GatheringCourotine(int damage, WaitForSeconds timeToGather, int amountOfItems, int maxAmountOfItems, ItemDrop itemToDrop, WaitForSeconds respawnTimer)
    {
        Item droppedItem = null;
        while (Tree.TreeHealth > 0)
        {
            playerMovementScript.StopMovingAndPlayAnimation(TimeToWaitToCancelAnimation);
            playerMovementScript.SetAgentRotationToTarget(gameObject);

            playerAnimator.Play("Attack");
            playerAnimator.SetBool("isAttacking", false);


            Tree.TreeHealth -= damage; //Set tree health

            if (droppedItem == null)
            {
                droppedItem = Instantiate(itemToDrop.ItemToDrop, transform.position, Quaternion.identity).GetComponent<Item>();
            }

            if (Tree.TreeHealth <= 0)
            {
                droppedItem.currentQuantity = amountOfItems;
                inventoryScript.AddItemToInventory(droppedItem, collect: false); // We are not collecting it with animation

                InitialiseTree(); // Reseting
                base.RespawnGatherableItem(respawnTimer);

                break;// yield return new WaitForSeconds(timeToGather / 2.0f); // Faster last hit
            }

            --amountOfItems;
            inventoryScript.AddItemToInventory(droppedItem, collect: false);

            SetCurrentAmountOfItems(amountOfItems);
            yield return (timeToGather); // Gathering

        }


        // Finish last hit
        playerAnimator.Play("Attack");
        playerAnimator.SetBool("isAttacking", false);

        base.RespawnGatherableItem(respawnTimer); //Was destroyed before
        InitialiseTree();

    }


    public override void StopGathering()
    {
        base.StopGathering();

        PlayerGather.CanGather = true; //Enable gathering another object again
    }

    /*private bool IsToolEquiped(string requiredTool)
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
    }*/
}
