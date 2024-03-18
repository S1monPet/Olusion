using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class AnimalHealthbar : MonoBehaviour
{
    public Slider animalHealthSlider;
    public Gradient gradient;
    public Image fill;
    public void SetAnimalHealthSlider(int animalHealth)
    {
        animalHealthSlider.value = animalHealth;
        SetAnimalHealthSliderColor(); 
    }

    private void SetAnimalHealthSliderColor()
    {
        fill.color = gradient.Evaluate(animalHealthSlider.normalizedValue);
    }
}
