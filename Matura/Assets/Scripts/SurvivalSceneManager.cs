using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI; 

public class SurvivalSceneManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface m_navMeshSurface;

    [Header("UI")]
    public PlayerMovement playerMovementScript; 
    public GameObject closeChestUI; 
    public GameObject inventoryUI; 

    [Header("Survival Timer")]
    private float _survivedTime; 
    public TextMeshProUGUI survivalTimeText;

    [Header("Enemy")]
    private int _animationToStartWith; //Will start with "Walking"

    //[Header("Object Pooling")]

    private void SurvivalTimer() //On death automatically stops counting
    {
        _survivedTime++;
    }

    public void UpdateSurvivedTimer() //Is from PlayerHealth script; 
    {
        survivalTimeText.text = _survivedTime.ToString() + "s";
    }
    

    private void Start()
    {
        InvokeRepeating(nameof(SurvivalTimer), 1f, 1f);
        Application.targetFrameRate = 300; //CAP

        _animationToStartWith = Animator.StringToHash("Walking");
    }


    //Is started when Enemy dies. 
    public void RespawnEnemy(WaitForSeconds timer, GameObject enemy, Animator animator)
    {
        StartCoroutine(EnemyRespawnTimer(timer, enemy, animator));
    }

    private IEnumerator EnemyRespawnTimer(WaitForSeconds timer, GameObject currentEnemy, Animator animator)
    {
        yield return timer;
        /*GameObject enemy = ObjectPool.SharedInstance.GetPooledObject();
        if (enemy != null)
        {
            enemy.transform.position = currentEnemy.transform.position;
            enemy.transform.rotation = currentEnemy.transform.rotation;
            enemy.SetActive(true);
        }*/
        animator.Play(_animationToStartWith); 
        currentEnemy.SetActive(true);
    }

    public void EnemyChestDespawn(GameObject chest, GameObject chestInstantiatedParent, float despawnTimer)
    {
        StartCoroutine(EnemyChestDespawnCoroutine(chest, chestInstantiatedParent, despawnTimer));
    }

    private IEnumerator EnemyChestDespawnCoroutine(GameObject chest, GameObject chestInstantiatedParent, float despawnTimer)
    {
        yield return new WaitForSeconds(despawnTimer);

        inventoryUI.SetActive(false);
        closeChestUI.SetActive(false);

        playerMovementScript.enabled = true; // Why?

        chestInstantiatedParent.SetActive(false); // Disabling chest UI
        chest.SetActive(false);
    }

    public void RespawnGatherableItem(GameObject gatherableItem, ItemLifecycleManager itemLifecycleManager, WaitForSeconds timer)
    {
        // m_navMeshSurface.UpdateNavMesh(m_navMeshSurface.navMeshData);
        StartCoroutine(GatherableItemRespawnTimer(gatherableItem, itemLifecycleManager, timer));
    }

    private IEnumerator GatherableItemRespawnTimer(GameObject gatherableItem, ItemLifecycleManager itemLifecycleManager, WaitForSeconds timer)
    {
        yield return timer;

        if (itemLifecycleManager != null) // More readable than ?
            itemLifecycleManager.SpawnItemsRandomly(); 
            
        gatherableItem.SetActive(true);
    }

    public void RespawnAnimal(GameObject animal, WaitForSeconds respawnTimer)
    {
        StartCoroutine(AnimalRespawnCoroutine(animal, respawnTimer));
    }

    private IEnumerator AnimalRespawnCoroutine(GameObject animal, WaitForSeconds respawnTimer)
    {
        yield return respawnTimer;
        animal.SetActive(true);
    }

}
