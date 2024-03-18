using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : AnimalBase
{
    [Header("Prey Variables")]
    [SerializeField] private float _detectionRange = 10f; 
    [SerializeField] private float _escapeMaxDistance = 80f;

    private Predator currentPredator = null; 

    public void AlertPrey(Predator predator)
    {
        SetState(AnimalState.Chase); 
        currentPredator = predator;
        _currentCoroutine = StartCoroutine(RunFromPredator());
    }


    private IEnumerator RunFromPredator()
    {
        // Wait until the predator is within detection range.
        while (currentPredator == null || Vector3.Distance(animalAgent.nextPosition, currentPredator.transform.position) > _detectionRange) // Is predator in range
        {
            yield return null; 
        }

        while (currentPredator != null && Vector3.Distance(animalAgent.nextPosition, currentPredator.transform.position) <= _detectionRange) 
        {
            RunAwayFromPredator();

            yield return null; 
        }

        // Predator out of range, run to our final location and go back to idle.
        if (!animalAgent.pathPending && animalAgent.remainingDistance > animalAgent.stoppingDistance)
        {
            yield return null; 
        }

        SetState(AnimalState.Idle);
    }

    private void RunAwayFromPredator()
    {
        if (animalAgent != null && animalAgent.isActiveAndEnabled)
        {
            if (!animalAgent.pathPending && animalAgent.remainingDistance < animalAgent.stoppingDistance)
            {
                Vector3 runDirection = animalTransform.position - currentPredator.transform.position;
                Vector3 escapeDestination = animalTransform.position + runDirection.normalized * (_escapeMaxDistance * 2);
                animalAgent.SetDestination(GetRandomNavMeshPosition(escapeDestination, _escapeMaxDistance)); 
            }
        }
    }

    protected override void Die()
    {
        StopAllCoroutines(); 
        base.Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 
        Gizmos.DrawWireSphere(animalTransform.position, _detectionRange);
    }
}
