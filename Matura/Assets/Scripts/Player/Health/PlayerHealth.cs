using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{ 
    [SerializeField]
    Logger logger;

    [Header("Player Health")]
    public static int Health;
    public HealthBar healthBar;

    [Header("Armor")]
    public List<GameObject> armorObjects = new List<GameObject>();

    private void Start()
    {
        Health = 100;
    }

    private int CheckForArmorReduction() //Get armor reduction
    {
        foreach (GameObject armor in armorObjects)
        {
            if (armor.activeSelf)
                return armor.GetComponent<Item>().damageReduction; 
        }
        return 0; 
    }

    public void TakeDamage(int damage)
    {
        int armorReduction = CheckForArmorReduction();
        if (armorReduction <= 0)
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
        else if (armorReduction > 0)
        {
            float reducedDamage = damage * (1 - armorReduction / 100f);
            Health -= Mathf.CeilToInt(reducedDamage);

            ChangePlayerSliderHealth();

            if (Health <= 0)
            {
                //Play animation of dying, game over
                logger.Log("Bravo", this);
                Destroy(gameObject);
            }
            logger.Log(Health.ToString(), this);
        }
    }

    public void AddHealth(int amountOfHealth)
    {
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
