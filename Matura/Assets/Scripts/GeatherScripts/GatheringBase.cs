using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public abstract class GatheringBase : MonoBehaviour
{
    public Inventory inventoryScript; 
    public PlayerMovement playerMovementScript;
    public Transform playerTransform;
    public Animator playerAnimator; 
    public SurvivalSceneManager survivalSceneManager;
    public ItemLifecycleManager itemLifecycleManager; // For dropping items

    private Coroutine _gatheringCoroutine; //For stopping coroutine
    protected ItemDrop _currentItemDrop; 

    public float TimeToWaitToCancelAnimation = 0.3f;

    [Header("Buttons")]
    public GameObject EnableGather;
    public GameObject DisableGather;


    protected void Gather(int damage, GameObject player, int objectHealth, List<ItemDrop> itemDrops, WaitForSeconds gatheringRate, WaitForSeconds respawnTimer)
    {
        foreach (ItemDrop itemToDrop in itemDrops)
        {
            _currentItemDrop = itemToDrop; 
            if (_currentItemDrop.CurrentAmountOfItems == 0)
                return;

             _gatheringCoroutine = StartCoroutine(GatheringCourotine(damage, gatheringRate, _currentItemDrop.CurrentAmountOfItems, _currentItemDrop.StartingAmountOfItems, itemToDrop, respawnTimer));
        }
    }

    protected void SetCurrentAmountOfItems(int currentAmountOfItems) // Keeping track of how many we dropped, if we stop coroutine
    {
        _currentItemDrop.CurrentAmountOfItems = currentAmountOfItems;
    }


    public virtual void StopGathering()
    {
        StopGatheringCoroutine();
        //Animation get's handled in PlayerMovement Script
    }

    protected abstract IEnumerator GatheringCourotine(int damage, WaitForSeconds timeToGather, int amountOfItems, int maxAmountOfItems, ItemDrop itemToDrop, WaitForSeconds respawnTimer);

    protected void RespawnGatherableItem(WaitForSeconds itemRespawnTimer)
    {
        gameObject.SetActive(false);

        itemLifecycleManager.StopAllCoroutines(); 
        survivalSceneManager.RespawnGatherableItem(gameObject, itemLifecycleManager, itemRespawnTimer);
    }

    protected void StopGatheringCoroutine()
    {
        // StopAllCoroutines() - Could work as well

        if (_gatheringCoroutine != null)
        {
            StopCoroutine(_gatheringCoroutine);
            _gatheringCoroutine = null; //Reset the reference 
        }
    }


}
