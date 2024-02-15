using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
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

    [Header("Bars")]
    public HealthBar healthBar;
    public EnemyHealthBar enemyHealthBar; 
    public EnemyArmorBar enemyArmorBar;
    public NavMeshAgent currentNavMeshAgent; 

    [Header("Game Manager")]
    public GameManager gameManager; 


    //For patrolling
    private int targetPoint = 0; 
    private bool _isOnCooldown;

    protected void Init(NavMeshAgent agent) //Don't think this will ever be overriden
    {
        currentNavMeshAgent = agent; //This is all set on Awake
        agent.speed = EnemyStats.EnemyMovingSpeed;

        EnemyStats.EnemyHP = EnemyStats.StartingHP;
        EnemyStats.EnemyArmor = EnemyStats.StartingEnemyArmor;

        //For changing slider's to right value
        ChangeEnemySliderHealth(EnemyStats.EnemyHP);
        ChangeEnemySliderArmor(EnemyStats.EnemyArmor);
    }

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
                playerHealthScript.TakeDamage(EnemyStats.EnemyAttackDamage); //Change HP on player
                ChangePlayerSliderHealth(); //Change HP in HealthBar

                StartAttackCoolDown(EnemyStats.EnemyAttackCooldown);
            }
        } 
    }

    //Change Player Health Bar
    protected virtual void ChangePlayerSliderHealth()
    {
        healthBar.SetPlayerHealthSlider(EnemyStats.EnemyAttackDamage);
    }

    //For taking damage
    public virtual void EnemyTakeDamage(int damage)
    {
        TakeHealthDamage(damage);
        TakeArmorDamage(damage);
    }

    protected virtual void TakeHealthDamage(int damage)
    {
        //If there is no more enemy armor we take health damage
        if (enemyStats.EnemyArmor > 0) return; 

        enemyStats.EnemyHP -= damage;
        ChangeEnemySliderHealth(enemyStats.EnemyHP); 
    }

    //Changing enemy slider health
    protected virtual void ChangeEnemySliderHealth(int enemyHealth)
    {
        enemyHealthBar.SetEnemyHealthSlider(enemyHealth);
    }
    //Changing enemy slider Armor
    protected virtual void ChangeEnemySliderArmor(int enemyArmor)
    {
        enemyArmorBar.SetEnemyArmorSlider(enemyArmor);
    }

    protected virtual void TakeArmorDamage(int damage)
    {
        if (enemyStats.EnemyArmor > 0)
        {
            enemyStats.EnemyArmor -= damage;
            enemyArmorBar.SetEnemyArmorSlider(enemyStats.EnemyArmor);

            if (enemyStats.EnemyArmor <= 0)
            {
                enemyStats.EnemyHP += enemyStats.EnemyArmor; //+ Because enemyArmor is - Math++
                ChangeEnemySliderHealth(enemyStats.EnemyHP);

                enemyStats.EnemyArmor = 0; //Set Armor to -1 because there is noone left or 0
            }
        }
        Die();
    }

    protected virtual void Die()
    {
        if (enemyStats.EnemyHP <= 0)
        {
            gameObject.SetActive(false);

            Init(currentNavMeshAgent);
            gameManager.RespawnEnemy(EnemyStats.RespawnTimer, gameObject);
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