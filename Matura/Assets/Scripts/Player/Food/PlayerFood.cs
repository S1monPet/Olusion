using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFood : MonoBehaviour, IDataPersistance
{
    [Header("Player Food")]
    public int Food;
    [SerializeField] private float foodDecreaseTimer = 5.0f;
    public PlayerFoodBar playerFoodBar;

    public PlayerHealth playerHealth;
    private int _damageToTakeIfOutOfFood = 5;

    public void LoadData(GameData data)
    {
        Food = data.Food;
        playerFoodBar.SetPlayersFoodSlider(Food);
    }

    public void SaveData(ref GameData data)
    {
        data.Food = Food;
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
                yield return new WaitForSeconds(foodDecreaseTimer);

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
                while (playerHealth.Health > 0)
                {
                    yield return new WaitForSeconds(20);

                    if (Food > 0)
                    {
                        break;
                    }

                    playerHealth.TakeDamage(_damageToTakeIfOutOfFood, null);
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
