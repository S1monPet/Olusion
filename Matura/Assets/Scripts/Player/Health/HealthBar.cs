using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider playerHealthSlider;
    public Gradient gradient;
    public Image fill;
    //Setting Player's Health On Slider in EnemyBase.cs
    public void SetPlayerHealthSlider(int playerHealth)
    {
        playerHealthSlider.value = playerHealth;

        SetPlayerHealthSliderColor(); 
    }

    //For Health Bar Gradient
    private void SetPlayerHealthSliderColor()
    {
        fill.color = gradient.Evaluate(playerHealthSlider.normalizedValue);
    }
}
