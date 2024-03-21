using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusGathering : GatheringBase
{
    [SerializeField] private CactusSO cactus;
    public CactusSO Cactus => cactus;

    private void OnEnable()
    {
        PlayerGather.CanGather = true;
    }

    private void InitialiseCactus()
    {
        Cactus.CactusHealth = cactus.SpawningCactusHealth;

        // Using foreach loop, because lambda's are used for querying or returing results from collections
        foreach (ItemDrop itemDrop in Cactus.ItemDrops)
        {
            // Setting each item to it's max dropping amount
            itemDrop.CurrentAmountOfItems = itemDrop.StartingAmountOfItems;
        }
    }

    public void Gather(int damage, GameObject player, WaitForSeconds gatheringRate)
    {
        PlayerGather.CanGather = false;
        base.Gather(damage, player, cactus.CactusHealth, cactus.ItemDrops, gatheringRate, cactus.RespawnTimer);
    }

    protected override IEnumerator GatheringCourotine(int damage, WaitForSeconds timeToGather, int amountOfItems, ItemDrop itemToDrop, WaitForSeconds respawnTimer)
    {
        Item droppedItem = null; 
        while (cactus.CactusHealth > 0)
        {
            playerMovementScript.StopMovingAndPlayAnimation(TimeToWaitToCancelAnimation);
            playerMovementScript.SetAgentRotationToTarget(gameObject);
            playerAnimator.Play("Attack");
            playerAnimator.SetBool("isAttacking", false);

            cactus.CactusHealth -= damage; //Set tree health

            if (droppedItem == null)
            {
                droppedItem = Instantiate(itemToDrop.ItemToDrop, transform.position, Quaternion.identity).GetComponent<Item>();
            }

            if (cactus.CactusHealth <= 0)
            {
                droppedItem.currentQuantity = amountOfItems;
                inventoryScript.AddItemToInventory(droppedItem, collect: false); // We are not collecting it with animation

                InitialiseCactus(); // Reseting
                base.RespawnGatherableItem(respawnTimer);

                break;// yield return new WaitForSeconds(timeToGather / 2.0f); // Faster last hit
            }
            --amountOfItems;
            inventoryScript.AddItemToInventory(droppedItem, collect: false);

            SetCurrentAmountOfItems(amountOfItems);
            yield return timeToGather;
        }

        // Finish last hit
        playerAnimator.Play("Attack");
        playerAnimator.SetBool("isAttacking", false);

        base.RespawnGatherableItem(respawnTimer); //Was destroyed before
        InitialiseCactus();
    }


    public override void StopGathering()
    {
        base.StopGathering();

        PlayerGather.CanGather = true; //Enable gathering another object again
    }

    /*
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
    */
}
