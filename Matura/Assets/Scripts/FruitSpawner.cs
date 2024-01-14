using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public float minSpawnDelay = 1f; 
    public float maxSpawnDelay = 1.8f;

    public float minAngle = -10f;
    public float maxAngle = 10f;

    public float minForce = 15f;
    public float maxForce = 22f;

    public float maxLifetime = 3.7f;

    private int numberOfFruits = 5; //User will choose
    private int fruitsSpawned = 0; 
    
    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn()); 
    }

    private void OnDisable()
    {
        StopAllCoroutines(); 
    }

    
    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (fruitsSpawned != numberOfFruits)
        {
            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            Vector3 position = new Vector3();
            position.x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
            position.y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
            position.z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);


            //Diagnoal spawning
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
            

            GameObject fruit = Instantiate(prefab, position, rotation);
            
            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            //Stop objects that have been cut
            if (fruit.tag == "Sliced")
            {   
                Destroy(fruit, maxLifetime);
            }
            fruitsSpawned++;
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

        }
    }

    
}
