using System.Collections;
using System.Collections.Generic;
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


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        //Checks if inventory is active
        if (!inventory.activeSelf)
        { 
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxRaycastDistance, Color.green, 3);

                if (Physics.Raycast(ray, out hit, maxRaycastDistance, layerMask))
                {
                    Debug.Log("Hit: " + hit.collider.name + ", Tag: " + hit.collider.tag); 
                    if (hit.collider.CompareTag(groundTag))
                    {
                        agent.SetDestination(hit.point);
                    }
                }
            }
        }
    }




}

