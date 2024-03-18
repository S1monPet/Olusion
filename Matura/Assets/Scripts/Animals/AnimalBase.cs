using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI; 

public enum AnimalState
{
    Idle, Moving, Chase
}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AnimalBase : MonoBehaviour
{
    [SerializeField] protected PreySO Prey;

    [SerializeField] protected Animator animator;

    [Header("UI")]
    [SerializeField] protected AnimalHealthbar animalHealthbar; 

    [Header("Wander")]
    [SerializeField] private float wanderDistance = 50f; // How far can animal move
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float maxWalkTime = 6f; // Animal movement before taking a break

    [Header("Idle")]
    [SerializeField] private float idleTime = 5f; // Time to break

    [Header("Chase")]
    [SerializeField] private float runSpeed = 8f;

    [Header("Attributes")]
    [SerializeField] private int _maxRepathAmount = 5; 
    [SerializeField] private int _lookRotationSpeed = 1;

    protected NavMeshAgent animalAgent;
    protected Transform animalTransform; 
    protected AnimalState currentState = AnimalState.Idle;

    protected Coroutine _currentCoroutine;

    [Header("Respawn")]
    [SerializeField] protected SurvivalSceneManager survivalSceneManager;

    [Header("Enemy Drop")]
    public GameObject droppingItem;

    private void Start()
    {
        animalTransform = GetComponent<Transform>(); // External call otherwise
        animalAgent = GetComponent<NavMeshAgent>();

        InitialiseAnimal();
    }

    private void OnEnable()
    {
        InitialiseAnimal(); 
    }

    protected virtual void InitialiseAnimal()
    {
        Prey.PreyHealth = Prey.PreySpawningHealth; 

        animalAgent.speed = walkSpeed;
        currentState = AnimalState.Idle;

        UpdateState();
    }

    protected virtual void UpdateState()
    {
        switch(currentState)
        {
            case AnimalState.Idle:
                HandleIdleState();
                break; 
            case AnimalState.Moving:
                HandleMovingState();
                break;
            case AnimalState.Chase:
                HandleChaseState();
                break; 
        }
    }

    protected Vector3 GetRandomNavMeshPosition(Vector3 origin, float distance)
    {
        for (int i = 0; i != _maxRepathAmount; i++) // Trying to repath 
        {
            Vector3 randomDirection = Random.insideUnitSphere * distance; // Creating random point
            randomDirection += origin;

            NavMeshHit navMeshHit;

            if (NavMesh.SamplePosition(randomDirection, out navMeshHit, distance, NavMesh.AllAreas)) // Can we move towards this position
            {
                return navMeshHit.position;
            }
        }

        return origin; 
    }

    protected virtual void CheckChaseConditions()
    {

    }


    protected virtual void HandleChaseState()
    {
        StopAllCoroutines(); 
    }

    protected virtual void HandleIdleState()
    {
        StartCoroutine(WaitToMove()); 
    }

    // Animal wait timer
    private IEnumerator WaitToMove()
    {
        float waitTime = Random.Range(idleTime / 2, idleTime * 2); // Different moving
        yield return new WaitForSeconds(waitTime);

        Vector3 randomDestination = GetRandomNavMeshPosition(animalTransform.position, wanderDistance); 
        animalAgent.SetDestination(randomDestination);

        SetState(AnimalState.Moving); 
    }

    protected virtual void HandleMovingState()
    {
        StartCoroutine(WaitToReachDestination()); 
    }

    private IEnumerator WaitToReachDestination()
    {
        float startTime = Time.time; 

        while (animalAgent.pathPending || animalAgent.remainingDistance > animalAgent.stoppingDistance && animalAgent.isActiveAndEnabled)
        {
            if (Time.time - startTime >= maxWalkTime) // Prevent animals getting stuck trying to move to a position it can't reach 
            {
                animalAgent.ResetPath();
                SetState(AnimalState.Idle); 
                yield break; 
            }

            CheckChaseConditions();

            yield return null; 
        }
        // Destination reached
        SetState(AnimalState.Idle); 
    }

    protected void SetState(AnimalState newState)
    {
        if (currentState == newState)
            return; 
        currentState = newState;
        OnStateChanged(newState);
    }

    protected virtual void OnStateChanged(AnimalState newState)
    {
        if (newState == AnimalState.Moving)
            animalAgent.speed = walkSpeed;

        if (newState == AnimalState.Chase)
            animalAgent.speed = runSpeed;

        UpdateState(); 
    }

    protected void CheckForPlayer()
    {

    }

    public virtual void ReceiveDamage(int damage)
    {
        Prey.PreyHealth -= damage;
        ChangeAnimalSliderHealth(Prey.PreyHealth);

        if (Prey.PreyHealth <= 0)
        {
            gameObject.SetActive(false);

            Die();

            // Spawning item
            GameObject droppedItem = Instantiate(droppingItem, droppingItem.transform.position, droppingItem.transform.rotation);
            droppedItem.SetActive(true);

            InitialiseAnimal();
            survivalSceneManager.RespawnAnimal(gameObject, Prey.RespawnTimer);
        }
    }

    protected virtual void ChangeAnimalSliderHealth(int animalHealth)
    {
        animalHealthbar.SetAnimalHealthSlider(animalHealth);
    }

    protected virtual void Die()
    {
        StopAllCoroutines();
    }
}
