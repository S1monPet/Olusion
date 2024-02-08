using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    [Header("IU")]
    public GameObject inventory;
    public PlayerMovement playerMovementScript; 
    private List<Slot> allInventorySlots = new List<Slot>();
    public List<Slot> inventorySlots = new List<Slot>();
    public List<Slot> hotbarSlots = new List<Slot>();
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

    private float lastTapTime = 0f;
    private float normalTapSpeed = 0.3f; 

    [Header("Equaippable Items")]
    public List<GameObject> equaiappableItems = new List<GameObject>();




    public void Start()
    {
        ToggleInventory(false);

        allInventorySlots.AddRange(hotbarSlots); //Put it in the first 
        allInventorySlots.AddRange(inventorySlots);

        foreach(Slot uiSlots in allInventorySlots)
        {
            uiSlots.InitialiseSlot();
        }

        CreateEventTriggersForHotbar(); //Creates event triggers for carrying objects
    }

    private void CreateEventTriggersForHotbar()
    {
        for (int i = 0; i < hotbarSlots.Count; i++) 
        {
            EventTrigger trigger = hotbarSlots[i].GetComponent<EventTrigger>(); //?? hotbarSlots[i].gameObject.AddComponent<EventTrigger>(); Already set them up

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown // The type of event to listen for
            };

            int currentSlotIndex = i; 

            entry.callback.AddListener((data) => {

                //To stop moving and disable script
                playerMovementScript.enabled = false;
                EnableHotBarItem(currentSlotIndex); });

            trigger.triggers.Add(entry);

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

        InventoryOpened(); 
    }


    private void ActivateHotbarColliderTrigger()
    {
        foreach (Slot slot in hotbarSlots)
        {
            slot.gameObject.GetComponent<Collider>().isTrigger = true;
        }
    }

    private void DeactivateHotbarColliderTrigger() 
    {
        foreach (Slot slot in hotbarSlots)
        {
            slot.gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }

    private void InventoryOpened()
    {
        if (inventory.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            //To drag icon
            DragInventoryIcon();

        }
        else if (currentDragSlotIndex != -1 && Input.GetMouseButtonUp(0) || currentDragSlotIndex != -1 && !inventory.activeInHierarchy)
        {
            DropInventoryIcon();
        }

        if (inventory.activeInHierarchy)
        {
            ActivateHotbarColliderTrigger();

            DoubleTapDrop(); 

            if (!dragIconImage.gameObject.activeSelf)
                dragIconImage.gameObject.SetActive(true);

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    dragIconImage.transform.position = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {

                    dragIconImage.transform.position = new Vector3(-500, 0, 0);
                }
            }
        }

        if (!inventory.activeInHierarchy && dragIconImage.gameObject.activeSelf)
        {
            dragIconImage.gameObject.SetActive(false);
            dragIconImage.transform.position = Vector3.zero;

            DeactivateHotbarColliderTrigger(); 
        }
    }


    private void ItemRaycast(bool hasClicked = false)
    {
        itemHoverText.text = "";
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastDistance, Color.red, 10);
        if (Physics.Raycast(ray, out hit, raycastDistance, itemLayer) && !inventory.activeSelf) //Check if INVENTORY UI && HOTBAR are NOT HIT. 
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.ToString());
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
            }
        }
    }


    private void AddItemToInventory(Item itemToAdd)
    {
        int leftoverQuantity = itemToAdd.currentQuantity;
        Slot openSlot = null; 

        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Item heldItem = allInventorySlots[i].GetItem();
            if (heldItem != null && itemToAdd.name == heldItem.name)
            {
                int freeSpaceInSlots = heldItem.maxQuantity - heldItem.currentQuantity;
                if (freeSpaceInSlots >= leftoverQuantity)
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    Debug.Log("hER");
                    Destroy(itemToAdd.gameObject);
                    allInventorySlots[i].UpdateInventoryAmount(); 
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
                Debug.Log("no way");
                if (!openSlot) 
                    openSlot = allInventorySlots[i];
            }

            allInventorySlots[i].UpdateInventoryAmount();
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
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot currSlot = allInventorySlots[i];

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

        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot currSlot = allInventorySlots[i];
            if (currSlot.hovered)
            {
                if (currSlot.HasItem()) //Swap the items
                {
                    Item itemToSwap = currSlot.GetItem();

                    currSlot.SetItem(currentDraggedItem);

                    allInventorySlots[currentDragSlotIndex].SetItem(itemToSwap);

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
        allInventorySlots[currentDragSlotIndex].SetItem(currentDraggedItem);
        ResetDragVariables(); 
    }

    private void ResetDragVariables()
    {
        currentDraggedItem = null;
        currentDragSlotIndex = -1;
    }

    private void DoubleTapDrop()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); 

            if (touch.phase == TouchPhase.Began)
            {
                if ((Time.time - lastTapTime) <= normalTapSpeed)
                {
                    DropItem();
                } else
                {
                    RemoveAbilityToDropItem(); 
                }
                lastTapTime = Time.time;
            }
        }
    }


    private void RemoveAbilityToDropItem()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot currSlot = allInventorySlots[i];

            if (currSlot._canDropThisItem && currSlot.HasItem())
            {
                //Setting it back to false since Click event doesn't do that.
                currSlot._canDropThisItem = false;
                break;
            }
        }
    }

    private void DropItem()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot currSlot = allInventorySlots[i];

            if(currSlot._canDropThisItem && currentDraggedItem != null)
            {
                //Get's item into currSlot && resets currentDragedItem
                currSlot.SetItem(currentDraggedItem);
                Item currentItem = currSlot.GetItem(); 
                ResetDragVariables();


                //For setting item's counter
                currSlot.DropItem(currentItem); 


                if (!currentItem.isActiveAndEnabled) 
                {
                    currentItem.gameObject.SetActive(true);
                    currentItem.transform.position = dropLocation.position;
                } else
                {
                    GameObject currentGameObject = currentItem.gameObject;
                    GameObject newItem = Instantiate(currentGameObject, dropLocation.position, dropLocation.rotation);
                }
            


                //For checking if item is less than zero
                currSlot.CheckIfItemIsLessThanZero(currentItem, equaiappableItems);


                //Setting it back to false since Click event doesn't do that.
                currSlot._canDropThisItem = false;
                break;
                
            }
        }
    }

    //Hotbar
    private void EnableHotBarItem(int hotbarIndex)
    {

        foreach (GameObject item in equaiappableItems)
        {
            item.SetActive(false);
        }
        
        Slot hotbarSlot = hotbarSlots[hotbarIndex];

        if (hotbarSlot.HasItem())
        {
            if (hotbarSlot.GetItem().equiappableItemIndex != - 1)
            {
                equaiappableItems[hotbarSlot.GetItem().equiappableItemIndex].SetActive(true);
            }
        }
        //Re-enable the script
        playerMovementScript.enabled = true; 

    }
}
