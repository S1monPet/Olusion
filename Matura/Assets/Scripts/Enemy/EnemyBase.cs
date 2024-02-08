using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum Enemies { MeadowHunter, 
                      ForestRunner,
                      DarkForestHunter, DarkForestSoldiers, DarkForestKing, 
                      DesertHunter, DesertSoldiers, DesertQueen, 
                      SnowHunter, SnowSoldiers, SnowGuards, SnowHeavyGuards, SnowKing }

public enum Mobs { Crab }

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemySO enemyStats;
    protected EnemySO EnemyStats => enemyStats;

    [SerializeField] public GameObject player; //For all could be changed carefull.

    //For patrolling
    private int targetPoint = 0; 

    private bool _isOnColdown;
    private bool _isOnCooldown;

    //Setting base for patrolling
    protected virtual void Patrol(Transform[] patrolPoints)
    {
        if (Vector3.Distance(transform.position, patrolPoints[targetPoint].position) < 0.1f)
        {
            targetPoint++;
            if (targetPoint >= patrolPoints.Length)
                targetPoint = 0;
        }
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position, EnemyStats.EnemyMovingSpeed * Time.deltaTime);
    }



    public abstract void EnemyAttack();

    protected virtual bool CanAttack(float percent)
    {
        return !_isOnColdown; 
    }

    //For inheratance in other classes to change cooldown duration
    protected virtual void StartAttackCoolDown(float cooldownDuration)
    {
        StartCoroutine(AttackCooldown(cooldownDuration)); 
    }
    
    protected IEnumerator AttackCooldown(float cooldownDuration)
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        _isOnCooldown = false;
    }

}