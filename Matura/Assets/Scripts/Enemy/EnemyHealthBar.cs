using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class EnemyHealthBar : MonoBehaviour
{
    public Slider enemyHealthSlider;
    public Gradient gradient;
    public Image fill;

    //Setting Player's Health On Slider in EnemyBase.cs
    public void SetEnemyHealthSlider(int enemyHealth)
    {
        enemyHealthSlider.value = enemyHealth;

        SetEnemyHealthSliderColor();
    }

    //For Health Bar Gradient
    private void SetEnemyHealthSliderColor()
    {
        fill.color = gradient.Evaluate(enemyHealthSlider.normalizedValue);
    }
}
