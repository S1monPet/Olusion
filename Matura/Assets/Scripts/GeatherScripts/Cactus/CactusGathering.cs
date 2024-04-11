using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CactusGathering : GatheringBase
{
    [SerializeField] private CactusSO cactus;
    public CactusSO Cactus => cactus;

    protected override void InitialiseGatherable()
    {
        gatherableHealth = cactus.SpawningCactusHealth;

        itemDrop = cactus.itemDrop.ItemToDrop;
        currentAmountOfItems = cactus.itemDrop.MaxAmountOfItems;

        respawnTimer = cactus.RespawnTimer;
    }
}
