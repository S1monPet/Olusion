using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{ 
    [SerializeField]
    Logger logger;

    [Header("Player Health")]
    private static int Health = 100;

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            //Play animation of dying, game over
            logger.Log("Bravo", this);
            Destroy(gameObject);
        }
        logger.Log(Health.ToString(), this);
    }
}
