using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForestRunner : EnemyBase
{
    [SerializeField] public NavMeshAgent agent;

    public Transform[] patrolPoints;
    public float MaxX, MinX;
    public float MaxZ, MinZ;

    private void Awake()
    {
        Initialise(); //If I add some logic in my override Init
    }

    private void Initialise()
    {
        base.Init(agent, TypeOfEnemy());
        //Can add more logic here
    }

    public override Enemies TypeOfEnemy()
    {
        return Enemies.ForestRunner;
    }

    /*public override void EnemyTakeDamage(int damage)
    {
        base.EnemyTakeDamage(damage);
    }*/

    protected override void StartAttackCoolDown(float cooldownDuration)
    {
        base.StartAttackCoolDown(cooldownDuration);
    }

    // Optimising
    protected override void Tick()
    {
        base.Patrol(patrolPoints, agent);
    }


    /*
    private void Update()
    {
        base.Patrol(patrolPoints, agent);
    }
    */
    public override Vector3 SpawnPosition()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(MaxX, MinX), 0, Random.Range(MaxZ, MinZ));
        return spawnPosition;
    }
}
