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
    }

    public override Enemies TypeOfEnemy()
    {
        return Enemies.MeadowHunter; 
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
