using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    private static int Health = 100;

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            //Play animation of dying, game over
            Debug.Log("Bravo");
            Destroy(gameObject);
        }
        Debug.Log(Health.ToString());
    }
}
