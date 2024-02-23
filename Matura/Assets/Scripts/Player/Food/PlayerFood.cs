using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFood : MonoBehaviour
{
    [Header("Player Food")]
    public int Food;
    public PlayerFoodBar playerFoodBar;

    public PlayerHealth playerHealth;
    private int _damageToTakeIfOutOfFood = 5;

    private void Awake()
    {
        playerFoodBar.SetPlayersFoodSlider(Food);
    }

    private void Start()
    {
        StartCoroutine(FoodDecreaseTimer());
    }

    private IEnumerator FoodDecreaseTimer()
    {
        while (true)
        {
            while (Food > 0)
            {
                yield return new WaitForSeconds(30);

                if (Food < 0)
                {
                    break;
                }

                Food--;
                Debug.Log("Food decreased by 1. Current food level: " + Food);
                playerFoodBar.SetPlayersFoodSlider(Food);

            }
            if (Food <= 0)
            {
                while (PlayerHealth.Health > 0)
                {
                    yield return new WaitForSeconds(20);

                    if (Food > 0)
                    {
                        break;
                    }

                    playerHealth.TakeDamage(_damageToTakeIfOutOfFood);
                }
            }
        }
    }

    public void AddFood(int amountOfFood)
    {
        if (Food + amountOfFood > 100)
        {
            Food = 100;
            return;
        }

        Food += amountOfFood;
        playerFoodBar.SetPlayersFoodSlider(Food);
    }

}
