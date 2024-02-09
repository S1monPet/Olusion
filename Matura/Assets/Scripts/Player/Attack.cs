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

    private EnemyBase enemyBase; 

    public bool playerBusy = false;
    public LayerMask enemyMask; 
    //public Interactable enemy;
    public NavMeshAgent agent; 

    private RaycastHit hit;
    private float raycastDistance = 13f;

    private int tempDamage = 10;
    private bool wasTouchedAlready = false;


    private void Update()
    {
        SingleTouchCheckForEnemy(); 
    }

    public void SingleTouchCheckForEnemy()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !wasTouchedAlready)
            {
                CheckIfTouchedWasEnemy(touch);
                wasTouchedAlready = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                wasTouchedAlready = false; 

            }
        }
    }

    private void CheckIfTouchedWasEnemy(Touch touch)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, raycastDistance, enemyMask))
        {
            //Alright
            enemyBase = hit.collider.gameObject.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.EnemyTakeDamage(tempDamage);

                if (hitEffect != null)
                    hitEffect.Emit(100); //Only once
            }
        }
            
    }

}
