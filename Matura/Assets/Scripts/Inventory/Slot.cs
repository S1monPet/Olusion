using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Slot : MonoBehaviour//, IPointerDownHandler
{
    public bool hovered;
    public bool _canDropThisItem; 

    private Item heldItem; 

    private Color opaque = new Color(1, 1 , 1);
    private Color transparent = new Color(1, 1, 1, 0);

    private Image thisSlotImage;

    public TMP_Text thisSlotQuantityText;

    public Collider objectCollider; 


    public void InitialiseSlot()
    {
        objectCollider = GetComponent<Collider>();
        thisSlotImage = gameObject.GetComponent<Image>();
        thisSlotQuantityText = transform.GetChild(0).GetComponent<TMP_Text>();
        thisSlotImage.sprite = null;
        thisSlotImage.color = transparent;
        SetItem(null);
    }

    public void SetItem(Item item)
    {
        heldItem = item; 

        if(item != null )
        {
            thisSlotImage.sprite = heldItem.icon; 
            thisSlotImage.color = opaque;
            UpdateInventoryAmount(); 
        } 
        else
        {
            thisSlotImage.sprite = null;
            thisSlotImage.color = transparent;
            UpdateInventoryAmount();
        }
    }

    public Item GetItem()
    {
        return heldItem; 
    }


    public bool HasItem()
    {
        return heldItem ? true : false; 
    }

    //For updating amount of things in inventory
    public void UpdateInventoryAmount()
    {
        if(heldItem != null)
        {
            thisSlotQuantityText.text = heldItem.currentQuantity.ToString();
        }
        else
        {
            thisSlotQuantityText.text = "";
        }
    }
    //For inventory
    public void DropItem(Item item)
    {
        if (item.currentQuantity > 0 && item)
        {
            item.currentQuantity -= 1;
            UpdateInventoryAmount();
        } 
    }
    //For inventory
    public void CheckIfItemIsLessThanZero(Item item, List<GameObject> equaiappableItems)
    {
        if (item.currentQuantity < 1)
        {
            SetItem(null);
            equaiappableItems[item.equiappableItemIndex].SetActive(false);

            item.currentQuantity = 1;
        }

    }

    public void DropAllItems(Item item)
    {
        SetItem(null);
        item.IsHeld = false; 
    }

    public void OnTriggerEnter(Collider other)
    {
        hovered = true;

    }

    public void OnTriggerExit(Collider other)
    {
        hovered = false;
    }

    public void SetHoveredState()
    {
        if (objectCollider.isTrigger)
            hovered = true;

    }

    public bool SetItemToBeDropped
    {
        get { return _canDropThisItem; }
        set
        {
            // Additional logic when setting the value
            _canDropThisItem = value;
        }
    }

    /*
    #if UNITY_STANDALONE

        public void OnTriggerEnter(Collider other)
        {
            hovered = true;

        }

        public void OnTriggerExit(Collider other)
        {
            hovered = false; 
        }

        //Triggers it once so it can start hovering
        public void OnPointerDown(PointerEventData eventData)
        {
            if (objectCollider.isTrigger)
                hovered = true; 
        }


    #endif
    */
}
