using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Data", menuName = "MyScriptableOjbects/WeaponStats", order = 1)]

public class EnemySO : ScriptableObject
{
    //HP, ATTACK DAMAGE, MOVE SPEED, RANGE, ARMOR, ATTACK SPEED
    [field: Header("Enemy constants")]
    [field: SerializeField] public float EnemyHP { get; private set; }
    [field: SerializeField] public float EnemyAttackDamage;
    [SerializeField] private float EnemyAttackSpeed;
    [field: SerializeField] public float EnemyMovingSpeed;
    [field: SerializeField] public float EnemyRange;
    [field: SerializeField] public float EnemyArmor;

    public WaitForSeconds CoolDownWait { get; private set; } 

    private void OnEnable()
    {
        CoolDownWait = new WaitForSeconds(EnemyAttackSpeed);
    }
            
}