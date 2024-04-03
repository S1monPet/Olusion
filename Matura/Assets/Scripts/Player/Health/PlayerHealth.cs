using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDataPersistance
{ 
    [SerializeField]
    Logger logger;

    [Header("Player Health")]
    public int Health; //Had it as static before
    public HealthBar healthBar;

    [Header("Animations")]
    public Animator playerAnimator;

    [Header("UI")]
    public GameObject deathCanvas;
    public PlayerDeathUI playerDeathUIScript;
    private Sprite _enemySprite;
    private string _enemyName; 

    [Header("Armor")]
    public List<GameObject> armorObjects = new List<GameObject>();

    //Save and load
    public void LoadData(GameData data)
    {
        Health = data.Health;
    }

    public void SaveData(ref GameData data)
    {
        data.Health = Health;
    }


    private void Start()
    {
        ChangePlayerSliderHealth();
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

    public void TakeDamage(int damage, GameObject enemy)
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

                GameManager.Instance.SetCurrentPlayerState(GameManager.PlayerState.Dead); // For Save/Load 

                if (enemy != null)
                {
                    EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
                    _enemySprite = enemyBase.enemySprite;

                    _enemyName = enemyBase.currentEnemy.ToString(); 
                }
                ActivateDeathScreen();

                playerAnimator.Play("Death");
                DisableAllScripts(gameObject); 
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
                if (enemy != null)
                {
                    EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
                    _enemySprite = enemyBase.enemySprite;
                }

                ActivateDeathScreen();

                playerAnimator.Play("Death");
                DisableAllScripts(gameObject);
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

    private void DisableAllScripts(GameObject player)
    {
        foreach (var script in player.GetComponents<MonoBehaviour>())
        {
            script.enabled = false; 
        }
    }

    public bool CheckIfPlayerIsAlive()
    {
        if (Health <= 0) return false;

        return true; 
    }

    private void ActivateDeathScreen()
    {
        deathCanvas.SetActive(true);
        playerDeathUIScript.ChangeScreen(_enemySprite, _enemyName);
    }

}
