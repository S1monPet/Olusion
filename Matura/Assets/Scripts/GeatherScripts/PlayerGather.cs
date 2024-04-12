using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PlayerGather : MonoBehaviour
{
    public Transform playerTransform;
    public PlayerMovement playerMovementScript; 

    public Inventory inventoryScript;
    public GameObject inventory; 

    public static bool CanGather = true;
    public static bool AutoFarm = false; 

    private Item _currentItem; 

    private RaycastHit _hit;
    private float _maxRaycastDistance = 11f;

    public string TreeTag;
    public string CactusTag;
    public LayerMask layerMask; //For gatherable items Gatherable

    private GameObject _currentTarget;
    private Transform _objectTransform;

    [Header("Buttons")]
    public GameObject EnableGather;
    public GameObject DisableGather;

    private void Update()
    {
        DetectTouchAndSetTarget();
        CheckAndUpdateGatherButton();
    }


    private void DetectTouchAndSetTarget()
    {
        if (Input.touchCount > 0)
        {
            if (!inventory.activeInHierarchy)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit, _maxRaycastDistance, layerMask))
                    {
                        // Rotating towards current target
                        playerMovementScript.SetAgentRotationToTarget(hit.collider.gameObject);

                        // Setting current hitting target
                        _currentTarget = hit.collider.gameObject;
                        return;
                    }
                }
            }
        }
    }
    private void CheckAndUpdateGatherButton()
    {
        // If there's a target, check distance and update buttons
        if (_currentTarget != null && _currentTarget.activeInHierarchy && CanGather)
        {
            if (GetHoldingItemAndCheckDistance(_currentTarget))
            {
                // DisableGather.SetActive(false);
                EnableGather.SetActive(true);
            }
            else
            {
                // Too far away, reseting
                _currentTarget = null; 

                // DisableGather.SetActive(true);
                EnableGather.SetActive(false); 
            }
        }
        else
        {
            //Default state
            // DisableGather.SetActive(true);
            EnableGather.SetActive(false);
        }
    }

    private void StartGatherCooldown(WaitForSeconds timeToGather)
    {
        CanGather = false; 
        StartCoroutine(StartGatherCooldownCoroutine(timeToGather));
    }

    private IEnumerator StartGatherCooldownCoroutine(WaitForSeconds timeToGather)
    {
        yield return timeToGather;
        CanGather = true; 
    }

    public bool GetHoldingItemAndCheckDistance(GameObject currentGameObject)
    {
        _currentItem = inventoryScript.hotbarSlots.Select(slot => slot.GetItem()).FirstOrDefault(item => item != null && item.IsHeld);

        _objectTransform = currentGameObject.transform;

        if (IsWithinGatheringDistance())
        {
            return true; 
        }

        return false;
    }

    public bool IsWithinGatheringDistance()
    {
        float distanceToEnemy = Vector3.Distance(playerTransform.position, _objectTransform.position);

        if (_currentItem != null)
        {
            //Check if item's range is suitable for attack
            if (_currentItem.GatherRange != 0f)
            {

                if (distanceToEnemy < _currentItem.GatherRange)
                {
                    return true;
                }
                else if (!_objectTransform.gameObject.activeInHierarchy)
                {
                    return false;
                }
            }
        }
        return false; 
    }

    public void OnGatherButtonPress()
    {
        if (_currentTarget == null)
            return;

        // Gathering
        IGatherable gatherableComponent = _currentTarget.GetComponent<IGatherable>();
        if (gatherableComponent != null && CanGather)
        {
            if (AutoFarm)
            {
                AutoFarming(gatherableComponent); 
            }
            else
            {
                if (gatherableComponent.Gather(_currentItem.GatherDamage))
                {
                    ResetGather();
                    StartGatherCooldown(_currentItem.TimeToGather);
                }
                else
                {
                    // StartGatherCooldown
                    StartGatherCooldown(_currentItem.TimeToGather);
                }
            }
        }
    }

    private void AutoFarming(IGatherable gatherableComponent)
    {
        CanGather = false; 
        StartCoroutine(AutoFarmingCoroutine(gatherableComponent));
    }

    private IEnumerator AutoFarmingCoroutine(IGatherable gatherableComponent)
    {
        while (IsWithinGatheringDistance())
        {
            if (gatherableComponent != null)
            {
                if (gatherableComponent.Gather(_currentItem.GatherDamage))
                {
                    ResetGather();
                    yield break;
                }
            }

            yield return _currentItem.TimeToGather;
        }

        ResetGather();
        yield return null; 
    }

    private void ResetGather()
    {
        CanGather = true;
        _currentTarget = null;
    }
}
