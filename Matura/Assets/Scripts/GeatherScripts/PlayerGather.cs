using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PlayerGather : MonoBehaviour
{
    public Transform playerTransform;
    public Inventory inventoryScript; 
    public static bool CanGather;

    private Item _currentItem; 

    private RaycastHit _hit;
    private float _maxRaycastDistance = 11f;

    public string TreeTag;
    public string CactusTag;
    public LayerMask layerMask; //For gatherable items Gatherable

    private TreeGathering _currentTreeGatheringScript;
    private CactusGathering _currentCactusGatheringScript;
    private GameObject _currentTarget; 

    [Header("Buttons")]
    public GameObject EnableGather;
    public GameObject DisableGather;


    private void Awake()
    {

    }

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
                        _currentTarget = hit.collider.gameObject;

                        return; 
                    }
                    // We hit something else
                }
                // We reset _currentTarget
                
                else
                {
                    _currentTarget = null;
                    CanGather = true; // Add cooldown
                }
                
                // Stopping current gathering 
                if (_currentTreeGatheringScript != null)
                {
                    _currentTreeGatheringScript.StopGathering();
                    _currentTreeGatheringScript = null; //Reset the reference
                }
                else if (_currentCactusGatheringScript != null)
                {
                    _currentCactusGatheringScript.StopGathering();
                    _currentCactusGatheringScript = null;
                }
            }
        }
    }

    private void CheckAndUpdateGatherButton()
    {
        // If there's a target, check distance and update buttons
        if (_currentTarget != null && _currentTarget.activeInHierarchy && CanGather)
        {
            if (CheckIfCurrentObjectIsCloseEnough(_currentTarget))
            {
                DisableGather.SetActive(false);
                EnableGather.SetActive(true);
            }
            else
            {
                DisableGather.SetActive(true);
                EnableGather.SetActive(false);
            }
        }
        else
        {
            //Default state
            DisableGather.SetActive(true);
            EnableGather.SetActive(false);
        }
    }

    private bool CheckIfCurrentObjectIsCloseEnough(GameObject currentGameObject)
    {
        _currentItem = inventoryScript.hotbarSlots.Select(slot => slot.GetItem()).FirstOrDefault(item => item != null && item.IsHeld);

        Transform objectTransform = currentGameObject.transform;
        float distanceToEnemy = Vector3.Distance(playerTransform.position, objectTransform.position);

        if (_currentItem != null)
            //Check if item's range is suitable for attack
            if (_currentItem.GatherRange != 0f)
            {
                if (distanceToEnemy < _currentItem.GatherRange)
                    return true; 
                else if (!objectTransform.gameObject.activeInHierarchy)
                    return false;
            }
        return false;
    }

    private void DetectIfObjectIsGatherable()
    {

        DetectTouchAndSetTarget();

        if (_currentTarget != null)
        {
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
        // Tree
        if (_currentTreeGatheringScript != null && CanGather)
        {
            // To prevent faster gathering
            _currentTreeGatheringScript.StopGathering();

            _currentTreeGatheringScript.Gather(_currentItem.GatherDamage, gameObject, _currentItem.TimeToGather);

            _currentTreeGatheringScript = null;
            /*
            EnableGather.SetActive(false);
            DisableGather.SetActive(true);
            */
        }
        // Cactus
        else if (_currentCactusGatheringScript != null && CanGather)
        {
            // To prevent faster gathering
            _currentCactusGatheringScript.StopGathering();

            _currentCactusGatheringScript.Gather(_currentItem.GatherDamage, gameObject, _currentItem.TimeToGather);

            _currentCactusGatheringScript = null;
            /*
            EnableGather.SetActive(false);
            DisableGather.SetActive(true);
            */
        }
    }

    private void StartGatherCooldownTimer(WaitForSeconds timeToWait)
    {
        StartCoroutine(GatherCooldownTimer(timeToWait));
    }

    private IEnumerator GatherCooldownTimer(WaitForSeconds timeToWait)
    {
        CanGather = false; 
        yield return timeToWait;
        CanGather = true; 
    }
}
