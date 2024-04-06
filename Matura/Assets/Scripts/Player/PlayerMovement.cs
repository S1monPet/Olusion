using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour, IDataPersistance
{
    [Header("Player Position")]
    public Transform playerTransform;

    //public Camera camera;
    private LayerMask _combinedLayerMask;
    public LayerMask GroundLayerMask;
    public LayerMask EnemyLayerMask; 
    public LayerMask GatherableLayerMask;

    public GameObject inventory;
    public Inventory inventoryScript;

    private RaycastHit hit;
    public NavMeshAgent agent;
    public Animator animator;

    //Distance of player movement
    public static float MaxRaycastDistance = 20f; //Only one max
    public string GroundTag = "Ground";
    public string TreeTag = "Tree";
    public string CactusTag = "Cactus";
    public string EnemyTag = "Enemy";

    public float lookRotationSpeed = 20f;

    private Coroutine _rotationCoroutine = null;
    private Coroutine _autoMoveCoroutine = null; 

    // Temporary 
    public static bool AutoMoveEnabled = true; 
    public GameObject AutoMoveButton;
    private bool _isAutoMoving; 

    public bool _canMove = true;
    // Is player in range and has appropriate item to hit Gatherable or Enemy
    public static bool CanInteractWithTarget = false;
    public void LoadData(GameData data)
    {
        // On the first run, or if no save data exists, we skip setting the player's position to maintain the default start position.
        if (data.playerPosition == Vector3.zero) return;

        playerTransform.position = data.playerPosition;

        if (data.playerRotation != Quaternion.identity)
            playerTransform.rotation = data.playerRotation;

        agent.Warp(data.playerPosition); // Warping Agent to our position
    }

    public void SaveData(ref GameData data)
    {
        data.playerPosition = playerTransform.position;  //Not saving, because this is in Start
        data.playerRotation = playerTransform.rotation;
    }

    void Awake()
    {
        agent.updateRotation = false;
        // Setting all masks to check
        _combinedLayerMask = GroundLayerMask | EnemyLayerMask | GatherableLayerMask; 
    }


    void Update()
    {
        Movement();
    }

    #region Set player interaction state
    public static void SetTargetInteractionState(bool canInteract)
    {
        CanInteractWithTarget = canInteract; 
    }

    public static bool GetTargetInteractionState()
    {
        return CanInteractWithTarget; 
    }

    #endregion

    private void Movement()
    {
        //Checks if inventory is active
        if (!inventory.activeInHierarchy && _canMove)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * MaxRaycastDistance, Color.green, 3);

                    if (Physics.Raycast(ray, out hit, MaxRaycastDistance, _combinedLayerMask)) 
                    {
                        // Checking Interactions
                        if (!hit.collider.CompareTag(GroundTag))
                        {
                            if (CanInteractWithTarget)
                            {
                                agent.destination = hit.point;
                                SetAgentRotation();

                                StopMoving();
                            }
                            else
                            {
                                MovePlayer(hit.point);
                            }
                            return; 
                        }
                        else 
                        {
                            MovePlayer(hit.point); 
                        }

                    }
                }
            } 

            //Has path because we are resseting path in the other file if object is not clicked
            if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                SetAgentRotation();
                animator.SetBool("isRunning", true); //Setting animation

                //If animation is not idle
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("ReceiveHit"))
                    animator.Play("Running");

                ActivateAutoMoveButton();
            } 
            else if (!agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                DeactivateAutoMoveButton();

                animator.SetBool("isRunning", false); //Setting animation off
            }
        }
    }

    private void ActivateAutoMoveButton()
    {
        if (AutoMoveEnabled && !AutoMoveButton.activeInHierarchy)
        {
            AutoMoveButton.SetActive(true);
        }
    }

    private void DeactivateAutoMoveButton()
    {
        if (AutoMoveEnabled && AutoMoveButton.activeInHierarchy)
        {
            AutoMoveButton.SetActive(false);
        }
    }

    // Button 
    public void AutoMove()
    {
        _isAutoMoving = true; 
        /*
        if (_autoMoveCoroutine != null)
        {
            StopCoroutine(_autoMoveCoroutine);
        }
        _autoMoveCoroutine = StartCoroutine(AutoRunCoroutine());
        */
    }

    private IEnumerator AutoRunCoroutine()
    {
        while (true)
        {
            Vector3 nextDestination = transform.position + transform.forward * 2;
            MovePlayer(nextDestination);
            yield return null;
        }
    }

    private void MovePlayer(Vector3 destination)
    {
        // Disable any running coroutine
        StopOnGoingProcesses();


        // If grounds is hit and item was not pressed, set hit.point and go towards location
        if (agent.isStopped)
            agent.isStopped = false;

        agent.SetDestination(destination); // Returns bool
    }

    public void StopOnGoingProcesses()
    {
        inventoryScript.StopRestoration();
    }

    public void SetAgentRotation()
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
        }
        _rotationCoroutine = StartCoroutine(SetAgentRotationCoroutine());
    }

    private IEnumerator SetAgentRotationCoroutine() 
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Small threshold to avoid constant micro-adjustments && check if rotation is deafault
        while (lookRotation != Quaternion.identity && Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
            yield return null; 
        }
    }

    //Agent logic to stop moving and look towards item
    public void StopMoving() //For MAP UI
    {
        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        agent.ResetPath();
        animator.SetBool("isRunning", false);
    }

    public void StopMovingAndPlayAnimation(float timeToWaitToCancelAnimation)
    {
        _canMove = false; 
        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        agent.ResetPath();
        animator.SetBool("isRunning", false);

        WaitForAnimation(timeToWaitToCancelAnimation);
    }

    public void SetAgentRotationToTarget(GameObject target) //For fast rotation
    {
        Vector3 direction = (target.transform.position - playerTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        while (lookRotation != Quaternion.identity && Quaternion.Angle(playerTransform.rotation, lookRotation) > 0.1f)
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed); 
    }


    public void WaitForAnimation(float timeToWaitToCancelAnimation)
    {
        StartCoroutine(WaitForAnimationCoroutine(timeToWaitToCancelAnimation));
    }

    private IEnumerator WaitForAnimationCoroutine(float timeToWaitToCancelAnimation)
    {
        yield return new WaitForSeconds(timeToWaitToCancelAnimation);
        _canMove = true; 
    }
}

