using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Inventory : MonoBehaviour
{
    [Header("IU")]
    public GameObject inventory; 
    public List<Slot> inventorySlots = new List<Slot>();
    public Image crosshair;
    public TMP_Text itemHoverText;

    [Header("Raycast")]
    //Match the 
    private float raycastDistance = 15f; 
    public LayerMask itemLayer;

    public void Start()
    {
        ToggleInventory(false);

        foreach(Slot uiSlots in  inventorySlots)
        {
            uiSlots.InitialiseSlot();
        }
    }

    public void Update()
    {
        //Instantly picks it up right now
        //ItemRaycast(Input.GetMouseButtonDown(0));

        if (Input.GetMouseButtonDown(0))
        {
            ItemRaycast(true);
        }

        //Make a sprite so he will be able to open inventory 
        //if(Input.GetKeyDown(KeyCode.E))
        //ToggleInventory(!inventory.activeInHierarchy);

    }

    private void ItemRaycast(bool hasClicked = false)
    {
        itemHoverText.text = "";
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastDistance, Color.red, 10);
        if (Physics.Raycast(ray, out hit, raycastDistance, itemLayer) && !inventory.activeSelf)
        {
            if (hit.collider != null)
            {
                if (hasClicked) //Pick up
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem)
                    {
                        AddItemToInventory(newItem);
                    }
                }
                else //Get the name
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem)
                    {
                        //Setting text for item
                        itemHoverText.text = newItem.name;
                    }
                }
            }
        }
    }

    private void AddItemToInventory(Item itemToAdd)
    {
        int leftoverQuantity = itemToAdd.currentQuantity;
        Slot openSlot = null; 

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Item heldItem = inventorySlots[i].GetItem();

            if (heldItem != null && itemToAdd.name == heldItem.name)
            {
                int freeSpaceInSlots = heldItem.maxQuantity - heldItem.currentQuantity;
                if (freeSpaceInSlots >= leftoverQuantity)
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    
                    Destroy(itemToAdd.gameObject);
                    inventorySlots[i].UpdateInventoryAmount(); 
                    return; 
                } 
                else
                {
                    heldItem.currentQuantity = heldItem.maxQuantity; 
                    leftoverQuantity -= freeSpaceInSlots;
                }
            }
            else if (heldItem == null)
            {
                if (!openSlot) 
                    openSlot = inventorySlots[i];
            }

            inventorySlots[i].UpdateInventoryAmount();
        }

        if (leftoverQuantity > 0 && openSlot)
        {
            openSlot.SetItem(itemToAdd);
            itemToAdd.currentQuantity = leftoverQuantity;
            itemToAdd.gameObject.SetActive(false);
        } else
        {
            itemToAdd.currentQuantity = leftoverQuantity; 
        }
    }

    private void ToggleInventory(bool enable)
    {
        inventory.SetActive(enable);
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;

        //Disable the rotation of the camera; 
        //Camera.main.GetComponent<FirstPersonLook>().sensitivity = enable = 0 : 2; 
    }
}
