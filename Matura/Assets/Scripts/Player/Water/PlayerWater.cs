using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWater : MonoBehaviour, IDataPersistance
{
    [Header("Player Water")]
    public int Water;
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
            while (Water > 0)
            {
                yield return new WaitForSeconds(30);

                if (Water <= 0)
                {
                    break;
                }

                Water--;
                Debug.Log("Water decreased by 1. Current water level: " + Water);
                waterBar.SetPlayersWaterSlider(Water);

            }
            if (Water <= 0)
            {
                while (playerHealth.Health > 0)
                {
                    yield return new WaitForSeconds(20);

                    if (Water > 0) 
                    {
                        break; 
                    }

                    playerHealth.TakeDamage(_damageToTakeIfOutOfWater, null);
                }
            }
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
