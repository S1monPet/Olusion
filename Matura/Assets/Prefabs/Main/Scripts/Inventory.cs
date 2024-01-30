using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using Unity.VisualScripting;
using UnityEngine.AI;

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
    public Transform dropLocation; //Where we are dropping our element
    public NavMeshAgent agent;

    [Header("Drag and drop")]
    public Image dragIconImage;
    private Item currentDraggedItem;
    private int currentDragSlotIndex = -1; 



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
        if (inventory.activeInHierarchy && Input.GetMouseButtonDown(0)) 
        {
            DragInventoryIcon(); 
        } 
        else if(currentDragSlotIndex != -1 && Input.GetMouseButtonUp(0) || currentDragSlotIndex != -1 && !inventory.activeInHierarchy) 
        {
            DropInventoryIcon();
        }

        if (inventory.activeInHierarchy)
        {
            if (!dragIconImage.gameObject.activeSelf)
                dragIconImage.gameObject.SetActive(true);

            dragIconImage.transform.position = Input.mousePosition; 
        }

        if (!inventory.activeInHierarchy && dragIconImage.gameObject.activeSelf)
        {
            dragIconImage.gameObject.SetActive(false);
            dragIconImage.transform.position = Vector3.zero;
        }

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
                        //Agent logic to stop moving and look towards item
                        if (!agent.isStopped)
                        {
                            agent.velocity = Vector3.zero;
                            agent.isStopped = true;
                        }

                        AddItemToInventory(newItem);
                    }
                }
                /*
                else //Get the name
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem)
                    {
                        //Setting text for item
                        itemHoverText.text = newItem.name;
                    }
                }
                */
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

    private void DragInventoryIcon()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Slot currSlot = inventorySlots[i];

            if (currSlot.hovered && currSlot.HasItem())
            {
                currentDragSlotIndex = i; //Update the current drag slot index variable

                currentDraggedItem = currSlot.GetItem();
                dragIconImage.sprite = currentDraggedItem.icon;
                dragIconImage.color = new Color(1, 1, 1, 1); //Opaque (invinsible)

                currSlot.SetItem(null); //Remove the item from slot
            }
        }
    }

    private void DropInventoryIcon()
    {
        dragIconImage.sprite = null;
        dragIconImage.color = new Color(1, 1, 1, 0);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Slot currSlot = inventorySlots[i];
            if (currSlot.hovered)
            {
                if (currSlot.HasItem()) //Swap the items
                {
                    Item itemToSwap = currSlot.GetItem();

                    currSlot.SetItem(currentDraggedItem);

                    inventorySlots[currentDragSlotIndex].SetItem(itemToSwap);

                    ResetDragVariables();
                    return; 
                } 
                else //Place with no swap
                {
                    currSlot.SetItem(currentDraggedItem);
                    ResetDragVariables();
                    return; 
                }
            }
        }
        // ITEM WAS DROPPED
        inventorySlots[currentDragSlotIndex].SetItem(currentDraggedItem);
        ResetDragVariables(); 
    }

    private void ResetDragVariables()
    {
        currentDraggedItem = null;
        currentDragSlotIndex = -1; 
    }

    private void DropItem()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Slot currSlot = inventorySlots[i];

            if(currSlot.hovered && currSlot.HasItem())
            {
                currSlot.GetItem().gameObject.SetActive(true);
                currSlot.GetItem().transform.position = dropLocation.position; 
                currSlot.SetItem(null);
                break; 
            }
        }
    }
}
