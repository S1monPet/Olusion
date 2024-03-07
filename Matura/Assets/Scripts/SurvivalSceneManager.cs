using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalSceneManager : MonoBehaviour
{
    [Header("Timer")]
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



    private void Update()
    {
        //Will be for keeping trees, when they despawn to respawn, enemies over time, etc.

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

    public void RespawnGatherableItem(GameObject gatherableItem, WaitForSeconds timer)
    {
        StartCoroutine(GatherableItemRespawnTimer(gatherableItem, timer));
    }

    private IEnumerator GatherableItemRespawnTimer(GameObject gatherableItem, WaitForSeconds timer)
    {
        yield return timer; 

        gatherableItem.SetActive(true);
    }
}
