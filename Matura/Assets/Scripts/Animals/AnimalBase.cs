using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI; 

public enum AnimalState
{
    Idle, Moving, Chase
}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AnimalBase : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected AnimalHealthbar animalHealthbar; 

    [Header("Wander")]
    [SerializeField] private float wanderDistance = 50f; // How far can animal move
    [SerializeField] protected float walkSpeed = 5f;
    [SerializeField] private float maxWalkTime = 6f; // Animal movement before taking a break

    [Header("Idle")]
    [SerializeField] private float idleTime = 5f; // Time to break

    [Header("Chase")]
    [SerializeField] private float runSpeed = 8f;

    [Header("Attributes")]
    [SerializeField] private int _maxRepathAmount = 5; 
    [SerializeField] private int _lookRotationSpeed = 1;
    [SerializeField] private Vector3 _animalSpawningPosition;

    [Header("Animal Variables")]
    [SerializeField] protected float _detectionRange = 10f;
    [SerializeField] protected float _escapeMaxDistance = 5f;

    protected NavMeshAgent animalAgent;
    protected Transform animalTransform; 
    protected AnimalState currentState = AnimalState.Idle;
    protected Animator animator;

    [Header("Respawn")]
    [SerializeField] protected SurvivalSceneManager survivalSceneManager;

    [Header("Enemy Drop")]
    public GameObject droppingItem;

    [Header("Player")]
    [SerializeField] protected GameObject player;
    protected Transform playerTransform; 

    private void Awake()
    {
        animalTransform = GetComponent<Transform>(); // External call otherwise
        animator = animalTransform.GetChild(0).GetComponent<Animator>(); // Our animator is on model
        animalAgent = GetComponent<NavMeshAgent>();

        _animalSpawningPosition = animalTransform.position;
        playerTransform = player.transform;
    }

    private void OnEnable()
    {
        InitialiseAnimal();
    }


    public virtual void InitialiseAnimal()
    {
        ResetAnimalSettings(); // Reseting
        UpdateState(); // Start lifecycle
    }

    protected virtual void UpdateState()
    {
        switch (currentState)
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

    /* Abstract void */
    protected abstract void ResetAnimalSettings();

    public abstract void ReceiveDamage(int damage);

    protected void RunFromPlayer()
    {
        SetState(AnimalState.Chase);
    }

    private IEnumerator RunAwayFromPlayer()
    {
        // Not sure about this, it works though
        while (true)
        {
            // Checking if agent has reached destination
            if (!animalAgent.pathPending && animalAgent.remainingDistance <= animalAgent.stoppingDistance) // || animalAgent.remainingDistance == 0
            {
                SetState(AnimalState.Idle);
                yield break;
            }

            // Debug.Log("Moving: " + animalAgent.remainingDistance.ToString() + " " + animalAgent.stoppingDistance.ToString() + " " + animalAgent.pathPending.ToString());
            yield return null;
        }
    }

    private void SetRunningDestinationFromPlayer()
    {
        if (animalAgent != null)
        {
            Vector3 runDirection = animalTransform.position - playerTransform.position;
            Vector3 escapeDestination = animalTransform.position + runDirection.normalized * (_escapeMaxDistance * 2);
            animalAgent.SetDestination(GetRandomNavMeshPosition(escapeDestination, _escapeMaxDistance));
            // SetAgentRotationToTarget(escapeDestination); 
        }
    }

    /*
    public void SetAgentRotationToTarget(Vector3 targetDirection) //For fast rotation
    {
        Vector3 direction = (targetDirection - animalTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        while (lookRotation != Quaternion.identity && Quaternion.Angle(animalTransform.rotation, lookRotation) > 0.1f)
            animalTransform.rotation = Quaternion.Slerp(animalTransform.rotation, lookRotation, Time.deltaTime * 1f);
    }
    */

    protected virtual void HandleChaseState()
    {
        StopAllCoroutines();

        SetRunningDestinationFromPlayer();
        StartCoroutine(RunAwayFromPlayer());
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

        Vector3 randomDestination = GetRandomNavMeshPosition(_animalSpawningPosition, wanderDistance); 
        animalAgent.SetDestination(randomDestination);
        // SetAgentRotationToTarget(randomDestination);

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

            CheckChaseConditions(); // Checking if it's being chased

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
        // Debug.Log(currentState);
        animator?.CrossFadeInFixedTime(newState.ToString(), 0.3f); // Makes a transition

        if (newState == AnimalState.Moving)
            animalAgent.speed = walkSpeed;

        if (newState == AnimalState.Chase)
            animalAgent.speed = runSpeed;

        UpdateState(); 
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
