using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{

    public Camera camera;
    public LayerMask layerMask;
    public GameObject inventory;

    private RaycastHit hit;
    private NavMeshAgent agent;

    //Distance of player movement
    private float maxRaycastDistance = 30f;
    private string groundTag = "Ground";

    private float lookRotationSpeed = 20f;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }


    void Update()
    {
        //Checks if inventory is active
        if (!inventory.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxRaycastDistance, Color.green, 3);

                if (Physics.Raycast(ray, out hit, maxRaycastDistance, layerMask))
                {
                    if (hit.collider.CompareTag(groundTag))
                    {
                        agent.destination = hit.point;
                    } 
                }
            }
            //Has path because we are resseting path in the other file if object is not clicked
            if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance && agent.hasPath)
                SetAgentRotation();
        }
    }

    private void SetAgentRotation()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        if (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f) //Small threshold to avoid constant micro-adjustments
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        
    }
}

