using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Attack : MonoBehaviour
{
    private EnemyBase enemyBase;
    private List<Slot> HotbarSlots = new List<Slot>();

    public bool playerBusy = false;
    public LayerMask enemyMask;
    public Transform playerTransform; 
    public NavMeshAgent agent;
    public PlayerMovement playerMovement;
    public Animator playerAnimator;
    public float TimeToWaitToCancelAnimation = .3f;

    private RaycastHit hit;
    private float raycastDistance = 10f;

    private int handDamage = 10;
    private float _handRange = 2f;
    public float punchCooldown;
    private WaitForSeconds _punchCooldown;
    private bool _isOnCooldown = false; 
    private bool wasTouchedAlready = false;

    [Header("Buttons")]
    public GameObject EnableAttack; 
    public GameObject DisableAttack;


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
        CheckIfCurrentEnemyIsCloseEnough(); 
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

    public void OnAttackButtonClick()
    {
        //We've already captured enemy if it was clicked
        if (enemyBase != null && CheckIfCurrentEnemyIsCloseEnough()) 
        {
            playerMovement.StopMovingAndPlayAnimation(TimeToWaitToCancelAnimation);
            playerMovement.SetAgentRotationToTarget(enemyBase.gameObject);
            playerAnimator.Play("Attack");

            enemyBase.EnemyTakeDamage(HoldingItemDamage());
            StartCoroutine(AttackCooldown());

            enemyBase = null; //Reseting reference
            EnableAttack.SetActive(false);
            DisableAttack.SetActive(true);
        }

    }

    private bool CheckIfCurrentEnemyIsCloseEnough()
    {
        SingleTouchCheckForEnemy(); //Check for enemy

        if (enemyBase != null)
        {
            Item currentItem = HotbarSlots.Select(slot => slot.GetItem()).FirstOrDefault(item => item != null && item.IsHeld);
            float attackingRange = _handRange;

            Transform enemyTransform = enemyBase.gameObject.transform;
            float distanceToEnemy = Vector3.Distance(playerTransform.position, enemyTransform.position);

            if (currentItem != null)
                //Check if item's range is suitable for attack
                if (currentItem.HitRange != 0f)
                    attackingRange = currentItem.HitRange;

            if (distanceToEnemy < attackingRange && !_isOnCooldown)
            {
                DisableAttack.SetActive(false);
                EnableAttack.SetActive(true);
                return true;
            }
            else if (distanceToEnemy > attackingRange || !enemyTransform.GetChild(0).gameObject.activeInHierarchy)
            {
                EnableAttack.SetActive(false);
                DisableAttack.SetActive(true);

                enemyBase = null; //Reseting Reference
                return false; 
            }
        }
        return false; 
    }

    private void CheckIfTouchedWasEnemy(Touch touch)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, raycastDistance, enemyMask))
        {
            enemyBase = hit.collider.gameObject.GetComponent<EnemyBase>();
            Debug.Log("Set" + enemyBase);
        }
    }

    private IEnumerator AttackCooldown()
    {
        _isOnCooldown = true; 
        yield return AttackCooldownTimer();
        _isOnCooldown = false; 
    }
}
