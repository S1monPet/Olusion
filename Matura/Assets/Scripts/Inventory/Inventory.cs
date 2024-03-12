using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Threading;
using JetBrains.Annotations;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class Inventory : MonoBehaviour, IDataPersistance
{
    private CancellationTokenSource _tokenSource;

    [Header("IU")]
    public GameObject inventory;
    public PlayerMovement playerMovementScript; 
    private List<Slot> allInventorySlots = new List<Slot>();
    public List<Slot> inventorySlots = new List<Slot>();
    public List<Slot> hotbarSlots = new List<Slot>();
    public ToggleChest toggleChestScript; 
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
    private float normalTapSpeed = 0.2f;

    private Item previousHeldItem; 

    [Header("Equaippable Items")]
    public List<GameObject> equaiappableItems = new List<GameObject>();
    private int _currentHeldItemIndex = -1;
    private int _fixedHeldItemIndexForCoroutine = -1; 
    private int _previousHeldItemIndex = -1;

    [Header("Animator")]
    public Animator playerAnimator;
    private Coroutine _currentCoroutine; 

    [Header("Edibles")] 
    public PlayerFood FoodScript;
    public PlayerWater WaterScript;
    private bool _disableAnotherEventCall = false;

    [Header("Healing")]
    public PlayerHealth HealthScript; 

    [Header("Clothing")]
    public List<GameObject> equaiappableArmor = new List<GameObject>();
    private Item _currentEquiappedArmor; 

    [Header("Chest")] 
    private List<Slot> _chestSlots = new List<Slot>();
    private GameObject _chestSlotParent;

    [Header("Crafting")]
    public List<Recipe> itemRecipes = new List<Recipe>();

    [Header("Save/Load")]
    public List<GameObject> allItemPrefabs = new List<GameObject>();

    public void LoadData(GameData data)
    {
        LoadInventoryData(data.inventoryData);
    }

    public void SaveData(ref GameData data)
    {
        data.inventoryData = SaveInventoryData(); 
    }

    private InventoryData SaveInventoryData()
    {
        InventoryData data = new InventoryData(); 

        //Because allInventorySlots is also shared with chest
        List<Slot> allSlots = new List<Slot>();
        allSlots.AddRange(hotbarSlots);
        allSlots.AddRange(inventorySlots);


        foreach (Slot slot in allSlots)
        {
            Item item = slot.GetItem(); 
            if (item != null)
            {
                ItemData itemData = new ItemData(item.name, item.currentQuantity, allInventorySlots.IndexOf(slot)); 
                data.slotData.Add(itemData);    
            }
        }
        return data; 
    }

    private void LoadInventoryData(InventoryData inventoryData)
    {
        ClearInventory(); //Make sure nothing is taking our space

        foreach (ItemData itemData in inventoryData.slotData)
        {
            //Getting item by name 
            GameObject itemPrefab = allItemPrefabs.Find(prefab => prefab.GetComponent<Item>().name == itemData.itemName); 

            if (itemPrefab != null)
            {
                GameObject createdItem = Instantiate(itemPrefab, dropLocation.position, Quaternion.identity);
                Item item = createdItem.GetComponent<Item>();

                item.currentQuantity = itemData.quantity; 

                AddItemToInventory(item, itemData.slotIndex);
            }
        }

        foreach (Slot slot in allInventorySlots)
        {
            slot.UpdateInventoryAmount(); 
        }
    }

    public void ClearInventory()
    {
        foreach (Slot slot in allInventorySlots)
        {
            slot.SetItem(null); 
        }
    }

    private void OnDisable()
    {
        _tokenSource.Cancel();
    }

    public void Start()
    {
        _tokenSource = new CancellationTokenSource(); 

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

    public void StopCurrentCoroutine()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);

            //Reset Animator
            playerAnimator.Play("PlayerIdle");
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
                        //Agent logic to stop moving and look towards item, could be added in future
                        //playerMovementScript.StopPlayerNotRotation(); 

                        if (ItemIsRespawnable(newItem)) //Getting spawning zone with parent)
                            RespawnItem(newItem);

                        AddItemToInventory(newItem);
                    }
                } 
            }
        }
    }

    private bool ItemIsRespawnable(Item item)
    {
        return item.Respawnable;
    }

    private void RespawnItem(Item item)
    {
        ItemLifecycleManager itemLifecycleManager = item.gameObject.transform.parent.GetComponentInParent<ItemLifecycleManager>(); //Optimise
        if (itemLifecycleManager != null)
        {
            itemLifecycleManager.HandleItemRespawn(item);
        }
    }

    public void AddItemToInventory(Item itemToAdd, int overrideIndex = -1)
    {
        if (overrideIndex != - 1) //If chest is oppened
        {
            allInventorySlots[overrideIndex].SetItem(itemToAdd);
            itemToAdd.gameObject.SetActive(false);
            allInventorySlots[overrideIndex].UpdateInventoryAmount();
            return; 
        }

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

                    if (!heldItem.Respawnable) 
                        Destroy(itemToAdd.gameObject);
                    else
                        itemToAdd.gameObject.SetActive(false);

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
        } 
        /*
        else
        {
            itemToAdd.currentQuantity = leftoverQuantity; 
        }
        */
    }

    public void ToggleInventory(bool enable) //Public because I am accessing it in chest to close it
    {
        if (!enable)
        {
            foreach (Slot currentSlot in  allInventorySlots)
            {
                currentSlot.hovered = false; 
            }
        }

        if (!enable && _chestSlotParent != null)
        {
            foreach (Slot chestSlot in _chestSlots)
            {
                allInventorySlots.Remove(chestSlot);
            }

            _chestSlotParent.SetActive(false);

            _chestSlotParent = null;
            _chestSlots = null; 
        }

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

                    //Check if current item is the same as dragged item
                    if (itemToSwap.name == currentDraggedItem.name && itemToSwap.currentQuantity < itemToSwap.maxQuantity)
                    {
                        int combinedQuantity = currentDraggedItem.currentQuantity + itemToSwap.currentQuantity;
                        if (combinedQuantity < itemToSwap.maxQuantity)
                        {
                            SetItemQuantity(itemToSwap, combinedQuantity);
                            currSlot.UpdateInventoryAmount();

                            ResetDragVariables();
                        } 
                        //Otherwise we check for how much more it is, and leave it in a slot. 
                        else
                        {
                            int maxQuantityToAdd = combinedQuantity - itemToSwap.maxQuantity;

                            //Setting quantity for item
                            SetItemQuantity(itemToSwap, itemToSwap.maxQuantity);
                            currSlot.UpdateInventoryAmount();

                            //Setting quantity for dragged item
                            allInventorySlots[currentDragSlotIndex].SetItem(currentDraggedItem);

                            SetItemQuantity(currentDraggedItem, maxQuantityToAdd);
                            allInventorySlots[currentDragSlotIndex].UpdateInventoryAmount();

                            ResetDragVariables();
                        }

                        return; 
                    } 

                    currSlot.SetItem(currentDraggedItem);

                    allInventorySlots[currentDragSlotIndex].SetItem(itemToSwap);
                    ResetDragVariables();

                    ToggleItemStateOnSwap(itemToSwap, i);
                    
                    return; 
                } 
                else //Place with no swap
                {
                    currSlot.SetItem(currentDraggedItem);
                    ResetDragVariables();

                    ToggleItemStateOnSwap(currSlot.GetItem(), i);
                    
                    return; 
                }
            }
        }
        // ITEM WAS DROPPED
        allInventorySlots[currentDragSlotIndex].SetItem(currentDraggedItem);
        ResetDragVariables(); 
    }

    private void AddItemQuantity(Item currentItem, int quantityToAdd)
    {
        currentItem.currentQuantity += quantityToAdd;
    }

    private void SetItemQuantity(Item currentItem, int newQuantity)
    {
        currentItem.currentQuantity = newQuantity;
    }

    private void ResetDragVariables()
    {
        currentDraggedItem = null;
        currentDragSlotIndex = -1;
    }

    //Check if item is not in hotbar than swap it out of hand and put IsHeld to false so our attack script will not see it, and put damage on it
    private void ToggleItemStateOnSwap(Item currentItem, int inventoryIndex)
    {
        //There is 6 HotBar slots
        if (inventoryIndex > 6 && currentItem.equiappableItemIndex != -1)
        { 
            currentItem.IsHeld = false;
            equaiappableItems[currentItem.equiappableItemIndex].SetActive(false);
        }
        else if (inventoryIndex > 6 && currentItem.equiappableArmorIndex != -1)
        {
            currentItem.IsHeld = false;
            equaiappableArmor[currentItem.equiappableArmorIndex].SetActive(false);
        }

        /* Subject to change get item instantly in hand, probably not the best
        else if (inventoryIndex <= 6)
        {
            //Is in Hotbar enable it
            equaiappableItems[currentItem.equiappableItemIndex].SetActive(true);
            currentItem.IsHeld = true;
        }
        */
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
                    //DropItem(); 
                    DropAllItems();
                } else
                {
                    RemoveAbilityToDropItem(); 
                }
                lastTapTime = Time.time;
            } 
        }
    }

    private bool DoubleTapEquipArmor()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if ((Time.time - lastTapTime) <= normalTapSpeed)
                {
                    return true;
                }
                lastTapTime = Time.time;
            }
        }
        return false; 
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

    private void DropAllItems()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot currSlot = allInventorySlots[i];
            if (currSlot._canDropThisItem && currentDraggedItem != null && currentDraggedItem.equiappableItemIndex != -1)  
            {
                //Get's item into currSlot && resets currentDragedItem
                currSlot.SetItem(currentDraggedItem);

                Item currentItem = currSlot.GetItem();
                ResetDragVariables();

                currSlot.DropAllItems(currentItem);
                currentItem.gameObject.SetActive(true);
                currentItem.transform.position = dropLocation.position;

                //Sets item off the hand
                equaiappableItems[currentItem.equiappableItemIndex].SetActive(false);
                Debug.Log("Debil");

                break;
            }
            else if (currSlot._canDropThisItem && currentDraggedItem != null && currentDraggedItem.equiappableArmorIndex != -1)
            {
                //Get's item into currSlot && resets currentDragedItem
                currSlot.SetItem(currentDraggedItem);

                Item currentItem = currSlot.GetItem();
                ResetDragVariables();

                currSlot.DropAllItems(currentItem);
                currentItem.gameObject.SetActive(true);
                currentItem.transform.position = dropLocation.position;

                //Sets item off the hand
                equaiappableArmor[currentItem.equiappableArmorIndex].SetActive(false);

                break;
            }

        }
    }

    //Set new or the same item's HeldItem variable
    private void SetCurrentHeldItem(Item item)
    {
        // If the new item is the same as the previously held item, just ensure it's marked as held.
        if (previousHeldItem == item)
        {
            if (item != null && !item.IsHeld)
            {
                item.IsHeld = true;
            }
            return; //For quick execution
        }

        if (previousHeldItem != null) 
        {
            previousHeldItem.IsHeld = false; //Throws exception if no if statement, because it is null
        }

        previousHeldItem = item;

        if (item != null)
            item.IsHeld = true;

    }


    //Hotbar
    public void EnableHotBarItem(int hotbarIndex) //Previously called in the script now in inspector
    {

        foreach (GameObject item in equaiappableItems)
        {
            item.SetActive(false);
        }
        
        Slot hotbarSlot = hotbarSlots[hotbarIndex];

        if (hotbarSlot.HasItem())
        {
            if (hotbarSlot.GetItem().equiappableItemIndex != -1)
            {
                Item currentItem = hotbarSlot.GetItem();
                equaiappableItems[currentItem.equiappableItemIndex].SetActive(true);
                SetCurrentHeldItem(currentItem); //Set new or the same item's HeldItem variable
                _currentHeldItemIndex = hotbarIndex;
                if (_previousHeldItemIndex == _currentHeldItemIndex) //If item is the same consume/heal
                {
                    ConsumeIfHeldAndConsumable();
                    RegenerateHealthIfHealing();
                }
                _previousHeldItemIndex = hotbarIndex; 
            } 
            //We are also checking inventory here, because we equip item in hotbar not with inventory open, with inventory open we drop it
            else if (hotbarSlot.GetItem().equiappableArmorIndex != -1 && !inventory.activeInHierarchy) 
            {
                bool doubleTap = DoubleTapEquipArmor();
                if (doubleTap)
                {
                    Item currentArmor = hotbarSlot.GetItem();
                    equaiappableArmor[currentArmor.equiappableArmorIndex].SetActive(true);

                    if (currentArmor != _currentEquiappedArmor && _currentEquiappedArmor != null)
                    {
                        equaiappableArmor[_currentEquiappedArmor.equiappableArmorIndex].SetActive(false);
                        hotbarSlot.SetItem(_currentEquiappedArmor);
                        equaiappableArmor[currentArmor.equiappableArmorIndex].SetActive(true);
                        _currentEquiappedArmor = currentArmor;

                        ResetDragVariables(); 
                    }
                    else
                    {
                        _currentEquiappedArmor = currentArmor;
                        hotbarSlot.SetItem(null);

                        ResetDragVariables(); 
                    }
                }
            }
        } 
        else
        {
            SetCurrentHeldItem(null); //For attacking set it to null so, previous item is not held anymore
        }

        //Re-enable the script
        playerMovementScript.enabled = true; 

    }

    public void ConsumeIfHeldAndConsumable() //Event
    {
        if (!inventory.activeInHierarchy)
        {
            if (_currentHeldItemIndex != -1 && 
                hotbarSlots[_currentHeldItemIndex].GetItem() != null &&
                hotbarSlots[_currentHeldItemIndex].GetItem().IsHeld &&
                hotbarSlots[_currentHeldItemIndex].GetItem().Consumable &&
                !_disableAnotherEventCall)
            {

                Item currentItem = hotbarSlots[_currentHeldItemIndex].GetItem();
                _disableAnotherEventCall = true;
                _fixedHeldItemIndexForCoroutine = _currentHeldItemIndex;


                _currentCoroutine = StartCoroutine(ConsumingCoroutine(currentItem.TimeToConsume, currentItem));
            }
        }
    }

    /*public void StopConsuming() //Event
    {
        StopCoroutine(ConsumingCoroutine(0, null));
    }
    */
    private IEnumerator ConsumingCoroutine(float timeToWait, Item currentItem)
    { 
        yield return new WaitForSeconds(timeToWait);


        currentItem.currentQuantity--;
        hotbarSlots[_fixedHeldItemIndexForCoroutine].UpdateInventoryAmount(); 
        hotbarSlots[_fixedHeldItemIndexForCoroutine].CheckIfItemIsLessThanZero(currentItem, equaiappableItems);

        ConsumableType itemType = currentItem.type; 

        if (itemType == ConsumableType.Water)
        {
            WaterScript.AddWater(currentItem.Amount);
        } 
        else if (itemType == ConsumableType.Food) 
        { 
            FoodScript.AddFood(currentItem.Amount);
        }

        _disableAnotherEventCall = false;
    }

    public void RegenerateHealthIfHealing()
    {
        if (!inventory.activeInHierarchy)
        {
            if (_currentHeldItemIndex != -1 &&
                    hotbarSlots[_currentHeldItemIndex].GetItem() != null &&
                    hotbarSlots[_currentHeldItemIndex].GetItem().IsHeld &&
                    hotbarSlots[_currentHeldItemIndex].GetItem().Healing &&
                    !_disableAnotherEventCall)
            {
                Item currentItem = hotbarSlots[_currentHeldItemIndex].GetItem();
                _disableAnotherEventCall = true;
                _fixedHeldItemIndexForCoroutine = _currentHeldItemIndex;

                playerAnimator.Play("Heal");
                _currentCoroutine = StartCoroutine(HealingCoroutine(currentItem.TimeToGainHealth, currentItem));
            } 
        }
    }

    private IEnumerator HealingCoroutine(float timeToWait, Item currentItem)
    {

        yield return new WaitForSeconds(timeToWait);

        currentItem.currentQuantity--;
        hotbarSlots[_fixedHeldItemIndexForCoroutine].UpdateInventoryAmount();
        hotbarSlots[_fixedHeldItemIndexForCoroutine].CheckIfItemIsLessThanZero(currentItem, equaiappableItems);

        HealingType itemType = currentItem.healingType;

        if (itemType == HealingType.MedKit)
        {
            HealthScript.AddHealth(currentItem.HealthGain);
        }
        else if (itemType == HealingType.Bandage)
        {
            Debug.Log(currentItem.HealthGain);
            HealthScript.AddHealth(currentItem.HealthGain);
        }

        _disableAnotherEventCall = false;
    }

    /*
    public void EquipIfWearable() //Event
    {
        if (!inventory.activeInHierarchy)
        {
            if (_currentHeldItemIndex != -1 &&
                hotbarSlots[_currentHeldItemIndex].GetItem() != null &&
                hotbarSlots[_currentHeldItemIndex].GetItem().Wearable)
            {
                Item currentItem = hotbarSlots[_currentHeldItemIndex].GetItem();


                ClothingType clothingType = currentItem.clothingType;

                if (clothingType == ClothingType.Armor)
                {
                    //equaiappableClothes[currentItem.equiappableItemIndex].SetActive(true);
                }

                currentItem.currentQuantity--;
                hotbarSlots[_currentHeldItemIndex].CheckIfItemIsLessThanZero(currentItem, equaiappableItems);
            }
        }
    }
    */
    private void StopAgent() //Fix for better
    {
        playerAnimator.SetBool("isRunning", false);
        agent.isStopped = false;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        playerMovementScript.enabled = false;
    }

    public void OpenChest(Chest chest) //Getting it in ToggleChest
    {   
        StopAgent(); 

        ToggleInventory(true);

        chest.chestInstantiatedParent.SetActive(true);
        _chestSlotParent = chest.chestInstantiatedParent;

        allInventorySlots.AddRange(chest.allChestSlots);
        _chestSlots = chest.allChestSlots;
    }

    public void CraftItem(string itemName)
    {
        foreach (Recipe recipe in itemRecipes)
        {

            if (recipe.createdItemPrefab.GetComponent<Item>().name == itemName) //Could be optimised
            {
                bool haveAllIngredients = true;
                for (int i = 0; i != recipe.requiredIngredients.Count; i++)
                {
                    if (haveAllIngredients)
                    {
                        haveAllIngredients = HaveAllIngredients(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                    }
                }

                if (haveAllIngredients)
                {
                    for (int i = 0; i != recipe.requiredIngredients.Count; i++)
                    {
                        RemoveIngredients(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                    }
                    //Using dropLocation because if inventory is full or not full it's gonna be the same and it's better
                    GameObject craftedItem = Instantiate(recipe.createdItemPrefab, dropLocation.position, Quaternion.identity);
                    craftedItem.GetComponent<Item>().currentQuantity = recipe.quantityProduced;

                    //We don't want this to be put in the inventory
                    AddItemToInventory(craftedItem.GetComponent<Item>());
                }
                break;
            }
        }

    }

    private bool HaveAllIngredients(string itemName, int requiredQuantity)
    {
        int foundQuantity = 0;
        foreach (Slot currSlot in allInventorySlots)
        {
            if (currSlot.HasItem() && currSlot.GetItem().name == itemName && currSlot.GetItem() != null)
            {
                foundQuantity += currSlot.GetItem().currentQuantity;

                if (foundQuantity >= requiredQuantity)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void RemoveIngredients(string itemName, int quantity)
    {
        if (!HaveAllIngredients(itemName, quantity))
            return;

        int remainingQuantity = quantity; 

        foreach (Slot currSlot in allInventorySlots)
        {
            Item item = currSlot.GetItem();

            if (item != null && item.name == itemName)
            {
                if (item.currentQuantity >= remainingQuantity)
                {
                    item.currentQuantity -= remainingQuantity; 

                    if (item.currentQuantity == 0)
                    {
                        currSlot.SetItem(null);
                        currSlot.UpdateInventoryAmount();
                    }
                    return; 
                }
            }
        }
    }
}


