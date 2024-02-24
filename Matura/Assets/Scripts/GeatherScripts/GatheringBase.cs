using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public abstract class GatheringBase : MonoBehaviour
{
    [SerializeField] private TreeSO tree;
    public TreeSO Tree => tree;

    public Inventory inventoryScript; 
    public PlayerMovement playerMovementScript;
    public GameManager gameManager;

    private Coroutine _gatheringCoroutine; //For stopping coroutine

    private void OnDisable()
    {
        StopAllCoroutines(); //Stopping all
    }

    protected virtual void Gather(int damage, GameObject player, int objectHealth, List<ItemDrop> itemDrops, WaitForSeconds gatheringRate, WaitForSeconds respawnTimer)
    {

        foreach (ItemDrop itemToDrop in itemDrops)
        {
            if (itemToDrop.TotalAmountOfLogs == 0)
                return;
            
            _gatheringCoroutine = StartCoroutine(GatheringCourotine(gatheringRate, itemToDrop.TotalAmountOfLogs, itemToDrop, respawnTimer));
        }
    }

    protected IEnumerator GatheringCourotine(WaitForSeconds timeToGather, int amountOfItems, ItemDrop itemToDrop, WaitForSeconds respawnTimer)
    {
        for (int i = 0; i != amountOfItems; i++)
        {
            yield return timeToGather;

            //Expensive
            Item droppedItem = Instantiate(itemToDrop.ItemToDrop, transform.position, Quaternion.identity).GetComponent<Item>();
            droppedItem.currentQuantity = 1;

            inventoryScript.AddItemToInventory(droppedItem);
            Debug.Log(droppedItem.currentQuantity);
        }
        RespawnGatherableItem(respawnTimer); //Was destroyed before
    }

    protected void RespawnGatherableItem(WaitForSeconds itemRespawnTimer)
    {
        gameObject.SetActive(false);
        gameManager.RespawnGatherableItem(gameObject, itemRespawnTimer);
    }

    protected void StopGatheringCourotine()
    {
        if (_gatheringCoroutine != null)
        {
            StopCoroutine(_gatheringCoroutine);
            _gatheringCoroutine = null; //Reset the reference 
        }
    }


}
