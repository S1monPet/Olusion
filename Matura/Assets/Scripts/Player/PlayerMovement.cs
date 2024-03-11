using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour, IDataPersistance
{
    [Header("Player Position")]
    public Transform playerTransform;

    //public Camera camera;
    public LayerMask layerMask;
    public GameObject inventory;

    private RaycastHit hit;
    public NavMeshAgent agent;
    public Animator animator;

    //Distance of player movement
    public static float MaxRaycastDistance = 20f; //Only one max
    private string groundTag = "Ground";

    public float lookRotationSpeed = 20f;

    public bool _canMove = true; 

    public void LoadData(GameData data)
    {
        // On the first run, or if no save data exists, we skip setting the player's position to maintain the default start position.
        if (data.playerPosition == Vector3.zero) return; 

        playerTransform.position = data.playerPosition; 
    }

    public void SaveData(ref GameData data)
    {
        data.playerPosition = playerTransform.position; 
    }

    void Awake()
    {
        agent.updateRotation = false;
    }


    void Update()
    {
        Movement();
    }

    //To make sure agent is still.
    private void OnEnable()
    {
        //agent.ResetPath(); if you want to stop the agent
    }


    private void Movement()
    {
        //Checks if inventory is active
        if (!inventory.activeInHierarchy && _canMove)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * MaxRaycastDistance, Color.green, 3);

                    if (Physics.Raycast(ray, out hit, MaxRaycastDistance, layerMask))
                    {
                        if (hit.collider.CompareTag(groundTag))
                        {
                            //If grounds is hit and item was not pressed, set hit.point and go towards location
                            if (agent.isStopped)
                                agent.isStopped = false;

                            agent.destination = hit.point;

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
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && animator.GetCurrentAnimatorStateInfo(0).IsName("ReceiveHit"))
                    animator.Play("Running"); 
            } 
            else if (!agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                animator.SetBool("isRunning", false); //Setting animation off
            }
        }
    }

    public void SetAgentRotation() 
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Small threshold to avoid constant micro-adjustments && check if rotation is deafault
        if (lookRotation != Quaternion.identity && Quaternion.Angle(transform.rotation, lookRotation) > 0.1f) 
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
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

