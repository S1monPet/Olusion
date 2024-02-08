using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Attack : MonoBehaviour
{
    [SerializeField] float attackSpeed = 1.5f;
    [SerializeField] float attackDelay = 0.3f;
    [SerializeField] float attackDistance = 1.5f;
    [SerializeField] int attackDamage = 100;
    [SerializeField] ParticleSystem hitEffect;

    public bool playerBusy = false;
    //public Interactable enemy;
    public NavMeshAgent agent; 

    private RaycastHit hit;
    private float raycastDistance = 13f; 

    private void Update()
    {
        CheckIfTouchedWasEnemy(); 
    }

    private void CheckIfTouchedWasEnemy()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                //enemy = hit.transform.GetComponent<Interactable>(); 
                if (hitEffect != null)
                    Instantiate(hitEffect, hit.point += new Vector3(0, 0.1f, 0), hitEffect.transform.rotation);
            } 
            else
            {
                //enemy = null; 
            }
        }
    }

}
