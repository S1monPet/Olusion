using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    private Rigidbody fruitRigidbody;
    private GameObject currentFruit; 
    public float minSpawnDelay = 1f; 
    public float maxSpawnDelay = 1.8f;

    public float minAngle = -5f;
    public float maxAngle = 5f;

    public float minForce = 4f;
    public float maxForce = 5f;
    private float decelerationRate = 1f;

    private int numberOfFruits = 5; //User will choose
    private int fruitsSpawned = 1; 
    
    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    //For decelerating speed of fruit
    private void FixedUpdate()
    {
        if (fruitRigidbody?.velocity.magnitude > 0)
        {
            Vector3 decelerationForce = -fruitRigidbody.velocity.normalized * decelerationRate;
            fruitRigidbody.AddForce(decelerationForce, ForceMode.Acceleration);
        }
    }


    private void OnEnable()
    {
        StartCoroutine(Spawn()); 
    }


    private void OnDisable()
    {
        StopAllCoroutines(); 
    }

    //Clear objects of table
    private void ClearTable(GameObject fruit)
    {
        
    }


    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (fruitsSpawned <= numberOfFruits)
        {

            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            Vector3 position = new Vector3();
            position.x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
            position.y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
            position.z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);


            //Diagnoal spawning
            float spawningAngle = Random.Range(minAngle, maxAngle);
            Quaternion rotation = Quaternion.Euler(0f, 0f, spawningAngle);

            //Create fruit
            currentFruit = Instantiate(prefab, position, rotation);

            //Using tag here, because I am not going to search for the objects more times
            if (currentFruit.tag == "Watermelon") 
            {
                Vector3 newRotation = new Vector3(0f, 90f, spawningAngle);
                currentFruit.transform.GetChild(0).rotation = Quaternion.Euler(newRotation);
            }

            float force = Random.Range(minForce, maxForce);
            fruitRigidbody = currentFruit.GetComponent<Rigidbody>();
            fruitRigidbody.AddForce(currentFruit.transform.forward * force, ForceMode.Impulse);

            

            yield return new WaitUntil(() => currentFruit.tag == "Sliced");
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            fruitsSpawned++; 
            //Temporary solution 
            Destroy(currentFruit);
            currentFruit = null; 
            Cleaver cleaver = FindObjectOfType<Cleaver>();
            if (cleaver != null)
            {
                Destroy(cleaver.gameObject);
                cleaver = null;
            }
        }
    }
   
}
