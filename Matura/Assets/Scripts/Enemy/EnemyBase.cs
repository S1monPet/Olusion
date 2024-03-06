using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;


public enum Enemies { MeadowHunter, 
                      ForestRunner,
                      DarkForestHunter, DarkForestSoldiers, DarkForestKing, 
                      DesertHunter, DesertSoldiers, DesertKing, 
                      SnowHunter, SnowSoldiers, SnowGuards, SnowHeavyGuards, SnowKing }

public enum Mobs { Crab }

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemySO enemyStats;
    public EnemySO EnemyStats => enemyStats;

    [SerializeField] private GameObject player;
    public GameObject Player => player;
    public Sprite enemySprite;
    public Enemies currentEnemy; //Setting current enemy for getting out his name
    public PlayerHealth playerHealthScript; //Maybe some class will need it so it's protected

    [Header("Bars")]
    public EnemyHealthBar enemyHealthBar; 
    public EnemyArmorBar enemyArmorBar;
    protected NavMeshAgent currentNavMeshAgent; 
    protected float lookRotationSpeed = 20f;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Animations")]
    public Animator enemyAnimator;
    public Animator playerAnimator;
    public NavMeshAgent playerAgent;

    //For patrolling
    private int targetPoint = 0; 
    private bool _isOnCooldown;

    private void OnDisable()
    {
        _isOnCooldown = false; //Because courotines don't continue when you disable object
    }

    protected void Init(NavMeshAgent agent, Enemies enemyType) //Don't think this will ever be overriden
    {
        currentNavMeshAgent = agent; //This is all set on Awake
        agent.speed = EnemyStats.EnemyMovingSpeed;

        EnemyStats.EnemyHP = EnemyStats.StartingHP;
        EnemyStats.EnemyArmor = EnemyStats.StartingEnemyArmor;

        currentEnemy = enemyType; 

        //For changing slider's to right value
        ChangeEnemySliderHealth(EnemyStats.EnemyHP);
        ChangeEnemySliderArmor(EnemyStats.EnemyArmor);
    }
    public abstract Vector3 SpawnPosition();
    public abstract Enemies TypeOfEnemy();
    protected virtual void EnemyAttack(NavMeshAgent agent)
    {
        //Checking if enemy is close enough to hit player
        if (Vector3.Distance(player.transform.position, agent.transform.position) <= EnemyStats.EnemyAttackingRange)
        {
            EnemyHit();
            SetEnemyRotation(agent);

        }
        else if (Vector3.Distance(player.transform.position, agent.transform.position) <= EnemyStats.EnemyPatrolingRange)
        {
            agent.SetDestination(player.transform.position);
            agent.speed = EnemyStats.EnemyAttackSpeed;

            SetEnemyRotation(agent);
            //enemyAnimator.SetBool("IsPlayerClose", true); //Start running after
        }
        else
        {
            //if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walking")) //Check if is already walking
                //enemyAnimator.SetBool("IsPlayerClose", false);
        }

    }

    protected void SetEnemyRotation(NavMeshAgent agent)
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Small threshold to avoid constant micro-adjustments && check if rotation is deafault
        if (lookRotation != Quaternion.identity && Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    protected virtual void EnemyHit()
    {
        if (CanAttack()) 
        {
            if (playerHealthScript != null) 
            {
                enemyAnimator.Play("Attack");
                playerHealthScript.TakeDamage(EnemyStats.EnemyAttackDamage, gameObject); //Change HP on player

                ResetAttack(); 
            }
        } 
    }


    protected void ResetAttack()
    {
        enemyAnimator.SetBool("Attack", false);
        StartAttackCoolDown(EnemyStats.EnemyAttackCooldown);
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

            Init(currentNavMeshAgent, currentEnemy);
            gameManager.RespawnEnemy(EnemyStats.RespawnTimer, gameObject, enemyAnimator);
        }
    }

    //Setting base for patrolling
    protected virtual void Patrol(Transform[] patrolPoints, NavMeshAgent agent)
    {
        if (playerHealthScript.CheckIfPlayerIsAlive()) //Checks player's HP so we don't start another hit, and pursue
            EnemyAttack(agent);

        if(!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            targetPoint = (targetPoint + 1) % patrolPoints.Length; //For effective looping through array 4 % 4 = 0; 
            agent.SetDestination(patrolPoints[targetPoint].position);
            agent.speed = EnemyStats.EnemyMovingSpeed;

            StartCoroutine(SetEnemyRotationCoroutine(agent));
        }
    }

    protected IEnumerator SetEnemyRotationCoroutine(NavMeshAgent agent)
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Continue rotation 
        while (targetRotation != Quaternion.identity && Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookRotationSpeed);
            yield return null; //Wait for the next frame
        }

    }


    //public abstract void EnemyAttack();

    protected virtual bool CanAttack()
    {
        return !_isOnCooldown; 
    }

    //For inheritance in other classes to change cooldown duration
    protected virtual void StartAttackCoolDown(float cooldownDuration)
    {
        StartCoroutine(AttackCooldown(cooldownDuration)); 
    }
    
    protected IEnumerator AttackCooldown(float cooldownDuration)
    {
        _isOnCooldown = true;
        StopEnemyAgentAndAnimations(); 

        yield return EnemyStats.CoolDownWait;

        currentNavMeshAgent.ResetPath();

        ReleaseEnemyAgentAndAnimations(); 
        _isOnCooldown = false;

    }

    private void StopEnemyAgentAndAnimations()
    {
        currentNavMeshAgent.isStopped = true;
        currentNavMeshAgent.velocity = Vector3.zero;
        enemyAnimator.SetBool("CoolDown", true);

    }

    private void ReleaseEnemyAgentAndAnimations()
    {
        currentNavMeshAgent.isStopped = false;
        enemyAnimator.Play("Walking");
        enemyAnimator.SetBool("CoolDown", false);
    }

}