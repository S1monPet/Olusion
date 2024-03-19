using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : AnimalBase
{
    [Header("Predator")]
    // [SerializeField] private float _detectionRange = 15f;
    // [SerializeField] private float _escapeMaxDistance = 10f;
    [SerializeField] private float _maxChaseTime = 10f;
    [SerializeField] private int _attackDamage = 10;
    [SerializeField] private float _attackCooldown = 2f;

    private Prey _currentChaseTarget;

    public override void ReceiveDamage(int damage)
    {
        base.RunFromPlayer(); 
        throw new System.NotImplementedException();
    }

    protected override void ResetAnimalSettings()
    {
        throw new System.NotImplementedException();
    }

    protected override void CheckChaseConditions()
    {
        if (_currentChaseTarget) // We are already chasing
            return; 
        Collider[] colliders = new Collider[10];
        int numColliders = Physics.OverlapSphereNonAlloc(animalTransform.position, _detectionRange, colliders);

        for (int i = 0; i < numColliders; i++)
        {
            if (colliders[i].gameObject.CompareTag("Animal")) // More optimised than checking component
            {
                Prey prey = colliders[i].GetComponent<Prey>();
                if (prey != null)
                {
                    StartChase(prey);
                    return; 
                }
            } 
        }
        _currentChaseTarget = null;
    }

    private void StartChase(Prey prey)
    {
        _currentChaseTarget = prey;
        SetState(AnimalState.Chase); 
    }

    protected override void HandleChaseState()
    {
        if (_currentChaseTarget != null)
        {
            _currentChaseTarget.AlertPrey(this);
            StartCoroutine(ChasePrey());
        }
        else
        {
            SetState(AnimalState.Idle);
        }
    }

    private IEnumerator ChasePrey()
    {
        float startTime = Time.time; 

        while(_currentChaseTarget != null && Vector3.Distance(animalTransform.position, _currentChaseTarget.transform.position) > animalAgent.stoppingDistance) // Checking if it hasn't been eaten
        {
            if (Time.time - startTime >= _maxChaseTime ||  _currentChaseTarget == null)
            {
                StopChase();
                yield break; 
            }
            SetState(AnimalState.Chase);
            animalAgent.SetDestination(_currentChaseTarget.transform.position);

            yield return null; 
        }
        if (_currentChaseTarget)
        {
            _currentChaseTarget.ReceiveDamage(_attackDamage);
        }

        yield return new WaitForSeconds(_attackCooldown);

        _currentChaseTarget = null;
        HandleChaseState();

        CheckChaseConditions(); 
    }

    private void StopChase()
    {
        animalAgent.ResetPath();
        _currentChaseTarget = null;
        SetState(AnimalState.Moving);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(animalTransform.position, _detectionRange);
    }
}
