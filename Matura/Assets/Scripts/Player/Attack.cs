using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Attack : MonoBehaviour
{
    private EnemyBase enemyBase;
    private List<Slot> HotbarSlots = new List<Slot>();

    public bool playerBusy = false;
    public LayerMask enemyMask; 
    //public Interactable enemy;
    public NavMeshAgent agent;
    public PlayerMovement playerMovement;
    public Animator playerAnimator; 

    private RaycastHit hit;
    private float raycastDistance = 10f;

    private int handDamage = 10;
    public float punchCooldown;
    private WaitForSeconds _punchCooldown;
    private bool _isOnCooldown = false; 
    private bool wasTouchedAlready = false;


    private void OnEnable()
    {
        _punchCooldown = new WaitForSeconds(punchCooldown);
    }

    private void Awake()
    {
        HotbarSlots = GetComponent<Inventory>().hotbarSlots;
    }


    private void Update()
    {
        SingleTouchCheckForEnemy(); 
    }

    public void SingleTouchCheckForEnemy()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !wasTouchedAlready && !_isOnCooldown)
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

    private int HoldingItemDamage()
    {
        for (int i = 0; i < HotbarSlots.Count; i++)
        {
            //Get current Item and check if IsHeld
            if (HotbarSlots[i].GetItem() != null && HotbarSlots[i].GetItem().IsHeld) //HasItem() Is only TRUE or FALSE null == TRUE
            {
                //Return current item damage
                return HotbarSlots[i].GetItem().Damage;
            }
        }
        return handDamage; 
    }

    private WaitForSeconds AttackCooldownTimer()
    {
        for (int i = 0; i < HotbarSlots.Count; i++)
        {
            //Get current Item and check if IsHeld
            if (HotbarSlots[i].GetItem() != null && HotbarSlots[i].GetItem().IsHeld) //HasItem() Is only TRUE or FALSE null == TRUE
            {
                //Return current item cooldown
                return HotbarSlots[i].GetItem()._attackCooldown;
            }
        }
        return _punchCooldown; 
    }

    private void CheckIfTouchedWasEnemy(Touch touch)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, raycastDistance, enemyMask))
        {
            //Alright
            enemyBase = hit.collider.gameObject.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                playerAnimator.Play("Attack");
                playerMovement.StopMoving(); 

                playerMovement.SetAgentRotation(); 
                enemyBase.EnemyTakeDamage(HoldingItemDamage());
                StartCoroutine(AttackCooldown());

                playerAnimator.SetBool("isAttacking", false); //Going back to idle
            } 

        }
            
    }

    private IEnumerator AttackCooldown()
    {
        _isOnCooldown = true; 
        yield return AttackCooldownTimer();
        _isOnCooldown = false; 
    }
}
