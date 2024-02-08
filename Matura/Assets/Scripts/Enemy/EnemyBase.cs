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

    [SerializeField] private GameObject player; 
    public GameObject Player => player;

    //For patrolling
    private int targetPoint = 0; 
    private bool _isOnCooldown;

    public abstract Enemies TypeOfEnemy();
    protected virtual void EnemyAttack(NavMeshAgent agent)
    {
        if (Vector3.Distance(player.transform.position, agent.transform.position) <= EnemyStats.EnemyRange)
        {
            agent.SetDestination(player.transform.position);
            agent.speed = EnemyStats.EnemyAttackSpeed; 
        }

    }

    //Setting base for patrolling
    protected virtual void Patrol(Transform[] patrolPoints, NavMeshAgent agent)
    {
        EnemyAttack(agent);

        if(!agent.pathPending && agent.remainingDistance < 0.1f)
    {
            targetPoint = (targetPoint + 1) % patrolPoints.Length; //For effective looping through array 4 % 4 = 0; 
            agent.SetDestination(patrolPoints[targetPoint].position);
            agent.speed = EnemyStats.EnemyMovingSpeed; 
        }
    }

    //public abstract void EnemyAttack();

    protected virtual bool CanAttack(float percent)
    {
        return !_isOnCooldown; 
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