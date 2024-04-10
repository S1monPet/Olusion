using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PlayerGather : MonoBehaviour
{
    public Transform playerTransform;
    public Inventory inventoryScript; 

    public static bool CanGather = true;
    public static bool AutoFarm = false; 

    private Item _currentItem; 

    private RaycastHit _hit;
    private float _maxRaycastDistance = 11f;

    public string TreeTag;
    public string CactusTag;
    public LayerMask layerMask; //For gatherable items Gatherable

    private IGathering _currentGatheringScript; 
    private TreeGathering _currentTreeGatheringScript;
    private CactusGathering _currentCactusGatheringScript;
    private GameObject _currentTarget;
    private Transform _objectTransform;

    [Header("Buttons")]
    public GameObject EnableGather;
    public GameObject DisableGather;

    private void Update()
    {
        DetectIfObjectIsGatherable();
        CheckAndUpdateGatherButton();
    }


    private void DetectTouchAndSetTarget()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit, _maxRaycastDistance, layerMask))
                {
                    if (hit.collider.CompareTag(TreeTag) || hit.collider.CompareTag(CactusTag))
                    {
                        ResetGatherableScripts();
                        _currentTarget = hit.collider.gameObject;

                        return; 
                    }
                }
            }
        }
    }

    private void ResetGatherableScripts()
    {
        // Stopping current gathering 
        if (_currentTreeGatheringScript != null)
        {
            _currentTreeGatheringScript = null; //Reset the reference
        }
        else if (_currentCactusGatheringScript != null)
        {
            _currentCactusGatheringScript = null;
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
                // Too far away, reseting scripts
                _currentTarget = null; 
                ResetGatherableScripts();

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

    private void ResetGather(WaitForSeconds timeToGather)
    {
        CanGather = false; 
        StartCoroutine(ResetGatherCoroutine(timeToGather));
    }

    private IEnumerator ResetGatherCoroutine(WaitForSeconds timeToGather)
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
            // We will interact with target stop moving
            if (!PlayerMovement.GetTargetInteractionState())
            {
                PlayerMovement.SetTargetInteractionState(true);
            }
            return true; 
        }

        // Move past target
        if (PlayerMovement.GetTargetInteractionState())
        {
            PlayerMovement.SetTargetInteractionState(false);
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
                    ResetGatherableScripts();
                    return false;
                }
            }
        }
        return false; 
    }

    private void DetectIfObjectIsGatherable()
    {

        DetectTouchAndSetTarget();

        if (_currentTarget != null)
        {
            _currentGatheringScript = GetComponent<IGathering>(); 
            if (_currentGatheringScript != null) 
            {
                _currentGatheringScript.GatherGatherable(5); 
            }

            if (_currentTarget.CompareTag(TreeTag))
            {
                _currentTreeGatheringScript = _currentTarget.GetComponent<TreeGathering>();
            }
            else if (_currentTarget.CompareTag(CactusTag))
            {
                _currentCactusGatheringScript = _currentTarget.GetComponent<CactusGathering>();
            }
        }
    }

    public void OnGatherButtonPress()
    {
        #region Tree
        if (_currentTreeGatheringScript != null && CanGather)
        {
            if (AutoFarm)
            {
                AutoFarming(); 
            }
            // Touch farming
            else
            {
                // Returns true if cut down
                if (_currentTreeGatheringScript.GatherTree(_currentItem.GatherDamage))
                {
                    CanGather = true;

                    _currentTarget = null;
                    _currentTreeGatheringScript = null;
                }
                else
                {
                    ResetGather(_currentItem.TimeToGather);
                }
            }
        }
        #endregion

        #region Cactus
        else if (_currentCactusGatheringScript != null && CanGather)
        {
            if (AutoFarm)
            {
                AutoFarming();
            }
            // Touch farming
            else
            {
                if (_currentCactusGatheringScript.GatherCactus(_currentItem.GatherDamage))
                {
                    CanGather = true;

                    _currentTarget = null;
                    _currentCactusGatheringScript = null;
                }
                else
                {
                    ResetGather(_currentItem.TimeToGather);
                }
            }
        }
        #endregion
    }

    private void AutoFarming()
    {
        CanGather = false; 
        StartCoroutine(AutoFarmingCoroutine());
    }

    private IEnumerator AutoFarmingCoroutine()
    {
        while (IsWithinGatheringDistance())
        {
            if (_currentTreeGatheringScript != null)
            {
                if (_currentTreeGatheringScript.GatherTree(_currentItem.GatherDamage))
                {
                    CanGather = true;

                    _currentTarget = null;
                    _currentTreeGatheringScript = null;

                    yield break; 
                }

            }
            else if (_currentCactusGatheringScript != null)
            {
                if (_currentCactusGatheringScript.GatherCactus(_currentItem.GatherDamage)) 
                {
                    CanGather = true;

                    _currentTarget = null;
                    _currentTreeGatheringScript = null;

                    yield break;
                }
            }

             yield return _currentItem.TimeToGather;
        }

        // Reset variables
        ResetAutoFarming();

        yield return null; 
    }

    private void ResetAutoFarming()
    {
        CanGather = true;

        _currentTarget = null;
        ResetGatherableScripts();
    }
}
