using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

public abstract class TreeGather : MonoBehaviour
{
    public LayerMask layerMask;
    public Animator animator;

    public GameObject inventory;
    public PlayerMovement playerMovementScript;
    public Item currentItem;
    public float _timeOfGather;

    private RaycastHit _hit;
    private float _maxRaycastDistance = 20f;
    private string _treeTag = "Tree";
    private bool _gathering = false; 

    

    private void Update()
    {
        DetectIfObjectIsGatherable();
        FastExitIfPlayerGathering(); 
    }

    private void DetectIfObjectIsGatherable()
    {
        if (!inventory.activeInHierarchy)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {

                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * _maxRaycastDistance, Color.blue, 15);

                    if (Physics.Raycast(ray, out _hit, _maxRaycastDistance, layerMask))
                    {
                        Debug.Log(_hit.collider.gameObject.name);
                        if (_hit.collider.CompareTag(_treeTag))
                        {
                            StopPlayer(); 
                            Gather();
                            Debug.Log("Gathering");
                        }
                    }
                }
            }
        }
    }
    private void FastExitIfPlayerGathering()
    {
        if (_gathering)
        {
            if (!inventory.activeInHierarchy)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        playerMovementScript.enabled = true;
                        StopCoroutine(GatheringCourotine());
                    }
                }
            }
        }
    }

    private void StopPlayer()
    { 
        playerMovementScript.agent.isStopped = false;
        playerMovementScript.agent.velocity = Vector3.zero;
        playerMovementScript.agent.ResetPath();

        playerMovementScript.enabled = false;
    }

    private void Gather()
    {
        //Start animation
        _gathering = true;
        StartCoroutine(GatheringCourotine());
        //Give logs in inventory empty slot
    }

    private IEnumerator GatheringCourotine()
    {
        yield return new WaitForSeconds(_timeOfGather);
        //Stop animation
        //Slowly put 1 by on object in slot
        playerMovementScript.enabled = true; 
    }

}
