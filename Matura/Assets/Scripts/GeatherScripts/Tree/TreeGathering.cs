using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class TreeGathering : GatheringBase
{
    [SerializeField] private TreeSO tree;
    public TreeSO Tree => tree;

    protected override void InitialiseGatherable()
    {
        // Initialising variables 
        gatherableHealth = Tree.SpawningTreeHealth;

        itemDrop = Tree.itemDrop.ItemToDrop;
        currentAmountOfItems = Tree.itemDrop.MaxAmountOfItems;

        respawnTimer = Tree.RespawnTimer;
    }
}
