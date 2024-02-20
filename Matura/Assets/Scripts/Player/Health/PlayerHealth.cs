using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{ 
    [SerializeField]
    Logger logger;

    [Header("Player Health")]
    public static int Health = 100;
    public HealthBar healthBar;


    public void TakeDamage(int damage)
    {
        Health -= damage;
        ChangePlayerSliderHealth();

        if (Health <= 0)
        {
            //Play animation of dying, game over
            logger.Log("Bravo", this);
            Destroy(gameObject);
        }
        logger.Log(Health.ToString(), this);
    }

    public void AddHealth(int amountOfHealth)
    {
        Debug.Log(amountOfHealth);
        if (Health + amountOfHealth >= 100)
        {
            Health = 100;
            ChangePlayerSliderHealth();
            return;
        }
        Health += amountOfHealth;
        ChangePlayerSliderHealth();
    }

    public void ChangePlayerSliderHealth()
    {
        healthBar.SetPlayerHealthSlider(Health);

    }
}
