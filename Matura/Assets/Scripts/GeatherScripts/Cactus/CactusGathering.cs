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
    }

    public void Gather(int damage, GameObject player, float gatheringRate)
    {
        PlayerGather.CanGather = false;
        base.Gather(damage, player, cactus.CactusHealth, cactus.ItemDrops, gatheringRate, cactus.RespawnTimer);
    }

    protected override IEnumerator GatheringCourotine(int damage, float timeToGather, int amountOfItems, ItemDrop itemToDrop, WaitForSeconds respawnTimer)
    {
        while (cactus.CactusHealth > 0)
        {
            playerMovementScript.StopMovingAndPlayAnimation(TimeToWaitToCancelAnimation);
            playerMovementScript.SetAgentRotationToTarget(gameObject);
            playerAnimator.Play("Attack");
            playerAnimator.SetBool("isAttacking", false);

            cactus.CactusHealth -= damage; //Set tree health

            if (cactus.CactusHealth <= 0)
            {
                base.RespawnGatherableItem(respawnTimer);

                //Expensive
                Item droppedItem = Instantiate(itemToDrop.ItemToDrop, transform.position, Quaternion.identity).GetComponent<Item>();
                droppedItem.currentQuantity = amountOfItems;

                inventoryScript.AddItemToInventory(droppedItem);

                break; // yield return new WaitForSeconds(timeToGather / 2.0f); // Faster last hit
            }
            yield return new WaitForSeconds(timeToGather);
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
