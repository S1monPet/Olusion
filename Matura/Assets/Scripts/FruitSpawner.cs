using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    private Rigidbody fruitRigidbody;
    public float minSpawnDelay = 1f; 
    public float maxSpawnDelay = 1.8f;

    public float minAngle = -5f;
    public float maxAngle = 5f;

    public float minForce = 15f;
    public float maxForce = 22f;

    private int numberOfFruits = 5; //User will choose
    private int fruitsSpawned = 1; 
    
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
            GameObject fruit = Instantiate(prefab, position, rotation);

            //Using tag here, because I am not going to search for the objects more times
            if (fruit.tag == "Watermelon") 
            {
                Vector3 newRotation = new Vector3(0f, 90f, spawningAngle);
                fruit.transform.GetChild(0).rotation = Quaternion.Euler(newRotation);
            }

            float force = Random.Range(minForce, maxForce);
            fruitRigidbody = fruit.GetComponent<Rigidbody>();
            fruitRigidbody.AddForce(fruit.transform.up * force, ForceMode.Impulse);

            //Start routine for checking velocity to stop fruit
            yield return StartCoroutine(CheckFruitVelocity(fruitRigidbody));

            if (fruit.tag == "Sliced" && fruitsSpawned > 1)
            {

                fruitsSpawned++;
                yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
                //Temporary solution 
                Destroy(fruit);
                Cleaver cleaver = FindObjectOfType<Cleaver>();
                if(cleaver != null)
                    Destroy(cleaver.gameObject);
            }
            else
            {
                yield return new WaitUntil(() => fruit.tag == "Sliced");
                yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
                fruitsSpawned++; 
                //Temporary solution 
                Destroy(fruit);
                Cleaver cleaver = FindObjectOfType<Cleaver>();
                if (cleaver != null)
                    Destroy(cleaver.gameObject);
            }
        }
    }
    
  
    //For stopping fruit
    private IEnumerator CheckFruitVelocity(Rigidbody fruitRigidbody) 
    {
        //Delay checking of velocity for better performance, fruit doesn't stop for good 2 seconds
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            if (fruitRigidbody != null && fruitRigidbody.velocity.y < 0.1f)
            {
                fruitRigidbody.velocity = Vector3.zero;
                fruitRigidbody.angularVelocity = Vector3.zero;
                fruitRigidbody.isKinematic = true; 
                yield break;
            }
            //Little delay so we don't run loop the whole time
            yield return new WaitForSeconds(0.1f);
        }
    }


}
