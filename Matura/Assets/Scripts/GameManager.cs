using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    [Header("Dying And Respawing")]
    private EnemyBase enemyBase; 

    [Header("FPS")]
    private float fps;
    public TextMeshProUGUI FPSText;

    public static ObjectPool SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool; 

    private void GetFPS()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPSText.text = fps.ToString();
    }

    private void Start()
    {
        InvokeRepeating("GetFPS", 1, 1);

        //Object pooling
        pooledObjects = new List<GameObject>();
        GameObject enemy; 
        for (int i = 0; i < amountToPool; i++)
        {
            enemy = Instantiate(objectToPool);
            enemy.SetActive(false);
            pooledObjects.Add(enemy);
        }
    }

    private void Update()
    {
        //Will be for keeping trees, when they despawn to respawn, enemies over time, etc.

    }
    //Is started when Enemy dies. 
    public void RespawnEnemy(WaitForSeconds timer, GameObject enemy)
    {
        StartCoroutine(EnemyRespawnTimer(timer));
        Instantiate(enemy);
    }

    private IEnumerator EnemyRespawnTimer(WaitForSeconds timer)
    {
        yield return timer; 
    }
}
