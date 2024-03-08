using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnvironmentalObject
{
    Tree, 
    Ground
}

public class RandomItemSpawner : MonoBehaviour
{
    [Header("Spawning Object")]
    public EnvironmentalObject enviromentalObject;

    public int MaximumAmountOfItems, MinimumAmountOfItems;


    public void SpawnItemsRandomly()
    {
        switch (enviromentalObject)
        {
            case EnvironmentalObject.Tree:
                SpawnItemsInTriangle();
                break; 
            case EnvironmentalObject.Ground:
                SpawnItemsInCircle();
                break; 
        }
    }

    private void SpawnItemsInTriangle()
    {

    }

    private void SpawnItemsInCircle()
    {

    }
}
