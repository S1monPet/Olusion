using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeadowHunter : EnemyBase
{
    [SerializeField] public NavMeshAgent agent;

    public Transform[] patrolPoints;

    private void Awake()
    {
        agent.speed = EnemyStats.EnemyMovingSpeed;
        //For currently setting the HP back to 100
        EnemyStats.EnemyHP = 100;
        EnemyStats.EnemyArmor = 10;

        //For changing slider's to right value
        base.ChangeEnemySliderHealth(EnemyStats.EnemyHP);
        base.ChangeEnemySliderArmor(EnemyStats.EnemyArmor);
    }

    public override Enemies TypeOfEnemy()
    {
        return Enemies.MeadowHunter; 
    }

    public override void EnemyTakeDamage(int damage)
    {
        base.EnemyTakeDamage(damage);
    }

    protected override void StartAttackCoolDown(float cooldownDuration)
    {
        base.StartAttackCoolDown(cooldownDuration);
    }

    private void Update()
    {
        base.Patrol(patrolPoints, agent);
    }
}
