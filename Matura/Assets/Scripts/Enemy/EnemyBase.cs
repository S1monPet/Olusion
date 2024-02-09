using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public EnemySO EnemyStats => enemyStats;

    [SerializeField] private GameObject player;
    public GameObject Player => player;
    public PlayerHealth playerHealthScript; //Maybe some class will need it so it's protected

    //For patrolling
    private int targetPoint = 0; 
    private bool _isOnCooldown;

    public abstract Enemies TypeOfEnemy();
    protected virtual void EnemyAttack(NavMeshAgent agent)
    {
        if (Vector3.Distance(player.transform.position, agent.transform.position) <= EnemyStats.EnemyPatrolingRange)
        {
            agent.SetDestination(player.transform.position);
            agent.speed = EnemyStats.EnemyAttackSpeed; 
        }
        //Checking if enemy is close enough to hit player
        if (Vector3.Distance(player.transform.position, agent.transform.position) <= EnemyStats.EnemyAttackingRange)
        {
            EnemyHit(); 
        }

    }

    protected virtual void EnemyHit()
    {
        if (CanAttack()) 
        {
            if (playerHealthScript != null)
            {
                playerHealthScript.TakeDamage(EnemyStats.EnemyAttackDamage);
                StartAttackCoolDown(EnemyStats.EnemyAttackCooldown);
            }
        } 
    }

    //For taking damage
    public virtual void EnemyTakeDamage(int damage)
    {
        enemyStats.EnemyHP -= damage; 
        Die();
    }

    protected virtual void Die()
    {
        if (enemyStats.EnemyHP <= 0) 
            Destroy(this.gameObject);
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

    protected virtual bool CanAttack()
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
        yield return EnemyStats.CoolDownWait; 
        _isOnCooldown = false;
    }

}