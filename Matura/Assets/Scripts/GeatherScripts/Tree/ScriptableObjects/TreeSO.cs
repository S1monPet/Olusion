using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TreeGathering", menuName = "Gathering/TreeGathering", order = 1)]

public class TreeSO : ScriptableObject
{
    [field: SerializeField] public string Tag; //Name of item; 
    [field: SerializeField] public int TreeHP; 
    [field: SerializeField] public float GatheringRate;
    [field: SerializeField] public int TotalAmountOfLogs;

    public WaitForSeconds GatheringRateTimer { get; private set; }

    private void OnEnable()
    {
        GatheringRateTimer = new WaitForSeconds(GatheringRate);
    }
}
