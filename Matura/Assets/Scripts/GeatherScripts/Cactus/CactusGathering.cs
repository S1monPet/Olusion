using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusGathering : GatheringBase
{
    [SerializeField] private CactusSO cactus;
    public CactusSO Cactus => cactus;

    private void OnEnable()
    {
        // PlayerGather.CanGather = true;
        InitialiseCactus(); 
    }

    private void InitialiseCactus()
    {
        gatherableHealth = cactus.SpawningCactusHealth;

        // Using foreach loop, because lambda's are used for querying or returing results from collections
        foreach (ItemDrop itemDrop in Cactus.ItemDrops)
        {
            // Setting each item to it's max dropping amount
            currentAmountOfItems = itemDrop.MaxAmountOfItems;
        }
    }

    public bool GatherCactus(int damage, GameObject player)
    {
        // Reseting CanGather in ResetGather
        // PlayerGather.CanGather = false;
        return base.GatherBase(damage, player, gatherableHealth, cactus.ItemDrops, cactus.RespawnTimer);
    }

    protected override bool Gather(int damage, ItemDrop itemToDrop, WaitForSeconds respawnTimer)
    {
        playerMovementScript.StopMovingAndPlayAnimation(TimeToWaitToCancelAnimation);
        playerMovementScript.SetAgentRotationToTarget(gameObject);

        playerAnimator.Play("Attack");
        playerAnimator.SetBool("isAttacking", false);

        gatherableHealth -= damage; //Set tree health

        Item droppedItem = Instantiate(itemToDrop.ItemToDrop, transform.position, Quaternion.identity).GetComponent<Item>();

        if (gatherableHealth <= 0)
        {
            droppedItem.currentQuantity = currentAmountOfItems; // Setting value of current amount if we instantly finish cutting
            inventoryScript.AddItemToInventory(droppedItem, collect: false); // We are not collecting it with animation


            InitialiseCactus(); // Reseting
            base.RespawnGatherableItem(respawnTimer);

            return true; 
        }

        --currentAmountOfItems;
        inventoryScript.AddItemToInventory(droppedItem, collect: false);

        return false; 

    }
}
