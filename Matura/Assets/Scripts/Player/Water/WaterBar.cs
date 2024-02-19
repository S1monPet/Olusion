using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterBar : MonoBehaviour
{
    public Slider playersAmountOfWater;
    //Setting Player's Health On Slider in EnemyBase.cs
    public void SetPlayersWaterSlider(int amountOfWater)
    {
        playersAmountOfWater.value = amountOfWater;
    }

}
