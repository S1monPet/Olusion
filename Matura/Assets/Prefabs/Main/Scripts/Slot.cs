using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; 

public class Slot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool hovered;
    private Item heldItem; 

    private Color opaque = new Color(1, 1 , 1);
    private Color transparent = new Color(1, 1, 1, 0);

    private Image thisSlotImage;

    public TMP_Text thisSlotQuantityText; 

    public void InitialiseSlot()
    {
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

#if UNITY_STANDALONE
    /* public void OnPointerClick(PointerEventData eventData)
    {
        hovered = true; 
    } */

    public void OnPointerDown(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        hovered = false; 
    }



#endif
}
