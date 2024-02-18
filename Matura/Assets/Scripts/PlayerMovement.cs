using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{

    //public Camera camera;
    public LayerMask layerMask;
    public GameObject inventory;

    private RaycastHit hit;
    public NavMeshAgent agent;
    public Animator animator;

    //Distance of player movement
    private float maxRaycastDistance = 20f;
    private string groundTag = "Ground";

    private float lookRotationSpeed = 20f;
    private bool _rotationOnly = false;
    private Vector3 _agentDestination; 



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
        if (!inventory.activeInHierarchy)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxRaycastDistance, Color.green, 3);

                    if (Physics.Raycast(ray, out hit, maxRaycastDistance, layerMask))
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
            } 
            else if (!agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                animator.SetBool("isRunning", false); //Setting animation off
            }
        }
    }

    private void SetAgentRotation()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Small threshold to avoid constant micro-adjustments && check if rotation is deafault
        if (lookRotation != Quaternion.identity && Quaternion.Angle(transform.rotation, lookRotation) > 0.1f) 
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    //Agent logic to stop moving and look towards item
    public void StopPlayerNotRotation()
    {
        _rotationOnly = true;
        if (agent.hasPath)
        {
            _agentDestination = agent.destination;
            agent.ResetPath();
        }

        if (!agent.isStopped)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }
        animator.SetBool("isRunning", false);
        StartCoroutine(PlayerRotationCoroutine(_agentDestination));
    }

    private IEnumerator PlayerRotationCoroutine(Vector3 agentDestinaton)
    {
        Vector3 direction = (agentDestinaton - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Continue rotation 
        while (targetRotation != Quaternion.identity && Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookRotationSpeed);
            yield return null; //Wait for the next frame
        }
        _rotationOnly = false;
    }




}

