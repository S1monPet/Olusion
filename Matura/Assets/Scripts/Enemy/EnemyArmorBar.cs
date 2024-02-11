using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class EnemyArmorBar : MonoBehaviour
{
    public Slider enemyArmorSlider;
    public Gradient gradient;
    public Image fill;

    public void SetEnemyArmorSlider(int enemyArmor)
    {
        enemyArmorSlider.value = enemyArmor;

        SetEnemyArmorSlider();
    }

    private void SetEnemyArmorSlider()
    {
        fill.color = gradient.Evaluate(enemyArmorSlider.normalizedValue); //Subject to change
    }
}
