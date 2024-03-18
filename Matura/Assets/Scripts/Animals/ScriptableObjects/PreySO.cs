using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Animals", menuName = "Animals/Prey", order = 1)]
public class PreySO : ScriptableObject
{
    [field: SerializeField] public int PreyHealth;
    [field: SerializeField] public int PreySpawningHealth;
    [field: SerializeField] public float RespawnTime;
    [field: SerializeField] public bool _attackPlayer = false;

    public List<ItemDrop> ItemDrops = new List<ItemDrop>();

    public WaitForSeconds RespawnTimer { get; private set; }

    private void OnEnable()
    {
        RespawnTimer = new WaitForSeconds(RespawnTime);
    }
}
