using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTree : GatheringBase
{
    private void Update()
    {
        DetectIfObjectIsGatherable();
        FastExitIfPlayerGathering();
    }
}
