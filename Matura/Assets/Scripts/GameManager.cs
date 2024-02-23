using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("FPS")]
    private float fps;
    public TextMeshProUGUI FPSText;

    [Header("Enemy")]
    private int _animationToStartWith; //Will start with "Walking"

    //[Header("Object Pooling")]

    private void GetFPS() //InvokeRepeating requires
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPSText.text = fps.ToString();
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetFPS), 1, 1);
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
}
