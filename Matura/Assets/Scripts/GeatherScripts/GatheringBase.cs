using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public abstract class GatheringBase : MonoBehaviour, IGathering
{
    [Header("Gatherable Variables")]
    protected int gatherableHealth;
    protected int currentAmountOfItems; 

    public Inventory inventoryScript; 
    public PlayerMovement playerMovementScript;
    public Transform playerTransform;
    public Animator playerAnimator; 
    public SurvivalSceneManager survivalSceneManager;
    public ItemLifecycleManager itemLifecycleManager; // For dropping items


    public float TimeToWaitToCancelAnimation = 0.3f;

    [Header("Buttons")]
    public GameObject EnableGather;
    public GameObject DisableGather;

    public bool GatherGatherable(int damage) 
    {
        UnityEngine.Debug.Log("Gathering"); 
        return true; 
    }

    protected bool GatherBase(int damage, int objectHealth, List<ItemDrop> itemDrops, WaitForSeconds respawnTimer)
    {
        foreach (ItemDrop itemToDrop in itemDrops)
        {
            if (itemToDrop.MaxAmountOfItems == 0)
                continue; // Continue to the next drop

            return Gather(damage, itemToDrop, respawnTimer);
        }
        return false; 
    }

    protected abstract bool Gather(int damage, ItemDrop itemToDrop, WaitForSeconds respawnTimer);

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
