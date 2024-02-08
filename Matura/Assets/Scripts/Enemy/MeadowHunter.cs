using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeadowHunter : EnemyBase
{
    [SerializeField] public NavMeshAgent agent;

    public Transform[] patrolPoints;
    private int targetPoint = 0;
    public override void EnemyAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void StartAttackCoolDown(float cooldownDuration)
    {
        base.StartAttackCoolDown(cooldownDuration);
    }

    private void Update()
    {
        base.Patrol(patrolPoints);
        
        //agent.SetDestination(player.transform.position);
    }
}
