using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWater : MonoBehaviour, IDataPersistance
{
    [Header("Player Water")]
    public int Water;
    [SerializeField] private float waterDecreaseTimer = 10.0f; 
    [SerializeField] private float waterHealthDecreaseTimer = 20.0f; 
    public WaterBar waterBar;
   
    public PlayerHealth playerHealth;
    private int _damageToTakeIfOutOfWater = 10;

    public void LoadData(GameData data)
    {
        Water = data.Water;
        waterBar.SetPlayersWaterSlider(Water);
    }

    public void SaveData(ref GameData data)
    {
        data.Water = Water;
    }

    private void Start()
    {
        StartCoroutine(WaterDecreaseTimer());
    }

    private IEnumerator WaterDecreaseTimer()
    {
        while (true)
        {
            if (Water > 0)
            {
                yield return new WaitForSeconds(waterDecreaseTimer);

                Water--;
                Debug.Log("Water decreased by 1. Current water level: " + Water);
                waterBar.SetPlayersWaterSlider(Water);
            }
            else
            {
                // Start decreasing health if water is at or below 0
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
        while (playerHealth.Health > 0 && Water <= 0)
        {
            yield return new WaitForSeconds(waterHealthDecreaseTimer);

            // Stopping damage if water was replenished
            if (Water > 0)
            {
                Debug.Log("Water replenished, stopping health decrease.");

                StartCoroutine(WaterDecreaseTimer());
                yield break;
            }

            playerHealth.TakeDamage(_damageToTakeIfOutOfWater, null);

            yield return null;
        }
    }

    public void AddWater(int amountOfWater)
    {
        if (Water + amountOfWater > 100)
        {
            Water = 100;
            return;
        }

        Water += amountOfWater;
        waterBar.SetPlayersWaterSlider(Water);
    }

}
