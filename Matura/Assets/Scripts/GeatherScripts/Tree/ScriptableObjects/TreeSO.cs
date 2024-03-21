using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Gathering", menuName = "Gathering/TreeGathering", order = 1)]

public class TreeSO : ScriptableObject
{
    [field: SerializeField] public string Tag; //Name of item; 
    [field: SerializeField] public int TreeHealth;
    [field: SerializeField] public int SpawningTreeHealth;
    [field: SerializeField] public float RespawnTime;

    public List<ItemDrop> ItemDrops = new List<ItemDrop>();    
    public WaitForSeconds RespawnTimer { get; private set; }

    private void OnEnable()
    {
        RespawnTimer = new WaitForSeconds(RespawnTime);
    }

}

[System.Serializable]
public class ItemDrop
{
    public GameObject ItemToDrop;
    public int StartingAmountOfItems; // For reseting
    public int CurrentAmountOfItems; // Keeping track of current items
}
