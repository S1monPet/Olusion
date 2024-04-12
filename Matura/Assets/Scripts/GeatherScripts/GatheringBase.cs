using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public abstract class GatheringBase : MonoBehaviour, IGatherable
{
    [Header("Gatherable Variables")]
    protected int gatherableHealth;
    protected int currentAmountOfItems;
    protected GameObject itemDrop; 
    protected WaitForSeconds respawnTimer; 

    public Inventory inventoryScript; 
    public PlayerMovement playerMovementScript;
    public Transform playerTransform;
    public Animator playerAnimator; 
    public SurvivalSceneManager survivalSceneManager;
    public ItemLifecycleManager itemLifecycleManager; // For dropping items

    public float TimeToWaitToCancelAnimation = 2f;

    [Header("Buttons")]
    public GameObject EnableGather;
    public GameObject DisableGather;

    private void OnEnable()
    {
        InitialiseGatherable(); 
    }

    protected abstract void InitialiseGatherable();

    public bool Gather(int damage)
    {
        // Returning true if gatherable was gathered
        return BaseGather(damage, itemDrop, respawnTimer);
    }

    protected bool BaseGather(int damage, GameObject itemDrop, WaitForSeconds respawnTimer)
    {
        playerMovementScript.StopMovingAndPlayAnimation(TimeToWaitToCancelAnimation);
        playerMovementScript.SetAgentRotationToTarget(gameObject);

        playerAnimator.Play("Attack");
        playerAnimator.SetBool("isAttacking", false);

        gatherableHealth -= damage; //Set tree health

        Item droppedItem = Instantiate(itemDrop, transform.position, Quaternion.identity).GetComponent<Item>();

        if (gatherableHealth <= 0)
        {
            droppedItem.currentQuantity = currentAmountOfItems; // Setting value of current amount if we instantly finish cutting
            inventoryScript.AddItemToInventory(droppedItem, collect: false); // We are not collecting it with animation


            InitialiseGatherable(); // Reseting
            RespawnGatherableItem(respawnTimer);

            return true;
        }

        --currentAmountOfItems;
        inventoryScript.AddItemToInventory(droppedItem, collect: false);

        return false; 
    }

    protected void RespawnGatherableItem(WaitForSeconds itemRespawnTimer)
    {
        gameObject.SetActive(false);

        if (itemLifecycleManager != null)
        {
            itemLifecycleManager.StopAllCoroutines(); 
            survivalSceneManager.RespawnGatherableItem(gameObject, itemLifecycleManager, itemRespawnTimer);
        }
        // We don't produce items
        else 
        {
            survivalSceneManager.RespawnGatherableItem(gameObject, null, itemRespawnTimer);
        }
    }


}
