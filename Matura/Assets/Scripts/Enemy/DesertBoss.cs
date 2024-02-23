using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DesertBoss : EnemyBase
{
    [SerializeField] public NavMeshAgent agent;

    public Transform[] patrolPoints;

    private void Awake()
    {
        Initialise(); //If I add some logic in my override Init
    }

    private void Initialise()
    {
        base.Init(agent);
        //Can add more logic here
    }

    public override Enemies TypeOfEnemy()
    {
        return Enemies.DesertHunter;
    }

    /*public override void EnemyTakeDamage(int damage)
    {
        base.EnemyTakeDamage(damage);
    }*/

    protected override void StartAttackCoolDown(float cooldownDuration)
    {
        base.StartAttackCoolDown(cooldownDuration);
    }

    private void Update()
    {
        base.Patrol(patrolPoints, agent);
    }
}

