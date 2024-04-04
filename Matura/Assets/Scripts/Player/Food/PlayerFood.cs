using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerFood : MonoBehaviour, IDataPersistance
{
    [Header("Player Food")]
    public int Food;
    [SerializeField] private float foodDecreaseTimer = 5.0f;
    [SerializeField] private float foodHealthDecreaseTimer = 30.0f;
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
            if (Food > 0)
            {
                yield return new WaitForSeconds(foodDecreaseTimer);

                Food--;
                Debug.Log("Food decreased by 1. Current food level: " + Food);
                playerFoodBar.SetPlayersFoodSlider(Food);

            }
            else 
            {
                // Start decreasing health if food is at or below 0
                StartDecreasingHealth();
                break; 
            }
        }
    }

    private void StartDecreasingHealth()
    {
        StartCoroutine(StartDecreasingHealthCoroutine());
    }

    private IEnumerator StartDecreasingHealthCoroutine()
    {
        while (playerHealth.Health > 0 && Food <= 0)
        {
            yield return new WaitForSeconds(foodHealthDecreaseTimer);

            // Stopping damage if water was replenished
            if (Food > 0)
            {
                Debug.Log("Food replenished, stopping health decrease.");

                StartCoroutine(FoodDecreaseTimer());
                yield break;
            }

            playerHealth.TakeDamage(_damageToTakeIfOutOfFood, null);

            yield return null; 
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
