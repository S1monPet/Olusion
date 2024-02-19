using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFoodBar : MonoBehaviour
{
    public Slider playersAmountOfFood;
    //Setting Player's Health On Slider in EnemyBase.cs
    public void SetPlayersFoodSlider(int amountOfFood)
    {
        playersAmountOfFood.value = amountOfFood;
    }
}
