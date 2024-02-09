using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Data", menuName = "MyScriptableOjbects/WeaponStats", order = 1)]

public class EnemySO : ScriptableObject
{
    //HP, ATTACK DAMAGE, MOVE SPEED, RANGE, ARMOR, ATTACK SPEED
    [field: Header("Enemy constants")]
    [field: SerializeField] public int EnemyHP { get; set; }
    [field: SerializeField] public int EnemyAttackDamage;
    [SerializeField] private float EnemyAttackCoolDown;
    [field: SerializeField] public float EnemyAttackCooldown;
    [field: SerializeField] public float EnemyAttackSpeed;
    [field: SerializeField] public float EnemyMovingSpeed;
    [field: SerializeField] public float EnemyPatrolingRange;
    [field: SerializeField] public float EnemyAttackingRange;
    [field: SerializeField] public int EnemyArmor;

    public WaitForSeconds CoolDownWait { get; private set; } 

    private void OnEnable()
    {
        CoolDownWait = new WaitForSeconds(EnemyAttackSpeed);
    }           
}