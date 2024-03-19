using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : AnimalBase
{
    [SerializeField] private PreySO prey;

    /*
    [Header("Prey Variables")]
    [SerializeField] private float _detectionRange = 10f; 
    [SerializeField] private float _escapeMaxDistance = 30f;
    */
    private Predator currentPredator = null; 

    // For predator animal
    public void AlertPrey(Predator predator)
    {
        SetState(AnimalState.Chase); 
        currentPredator = predator;
        StartCoroutine(RunFromPredator());
    }

    public override void ReceiveDamage(int damage)
    {
        prey.PreyHealth -= damage;

        if (prey.PreyHealth <= 0)
        {
            gameObject.SetActive(false);

            Die();

            // Spawning item
            GameObject droppedItem = Instantiate(droppingItem, droppingItem.transform.position, droppingItem.transform.rotation);
            droppedItem.SetActive(true);

            ResetAnimalSettings();
            survivalSceneManager.RespawnAnimal(gameObject, prey.RespawnTimer);
            return; 
        }

        base.RunFromPlayer();
        ChangeAnimalSliderHealth(prey.PreyHealth);
    }

    protected override void ResetAnimalSettings()
    {
        prey.PreyHealth = prey.PreySpawningHealth;

        animalAgent.speed = walkSpeed;
        currentState = AnimalState.Idle;

        ChangeAnimalSliderHealth(prey.PreyHealth);
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
        // StopAllCoroutines(); 
        base.Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 
        Gizmos.DrawWireSphere(animalTransform.position, _detectionRange);
    }
}
