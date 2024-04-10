using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class TreeGathering : GatheringBase
{
    [SerializeField] private TreeSO tree;
    public TreeSO Tree => tree;

    private void OnEnable()
    {
        // PlayerGather.CanGather = true;
        InitialiseTree(); 
    }


    private void InitialiseTree()
    {
        gatherableHealth = Tree.SpawningTreeHealth;

        // Using foreach loop, because lambda's are used for querying or returing results from collections
        foreach (ItemDrop itemDrop in Tree.ItemDrops)
        {
            // Setting each item to it's max dropping amount
            currentAmountOfItems = itemDrop.MaxAmountOfItems; 
        }
    }

    public bool GatherTree(int damage)
    {
        // Reseting Gather button in ResetGather
        // PlayerGather.CanGather = false;
        return base.GatherBase(damage, gatherableHealth, Tree.ItemDrops, Tree.RespawnTimer);

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


            InitialiseTree(); // Reseting
            base.RespawnGatherableItem(respawnTimer);

            return true; 
        }

        --currentAmountOfItems; 
        inventoryScript.AddItemToInventory(droppedItem, collect: false);

        return false; 
    }
}
