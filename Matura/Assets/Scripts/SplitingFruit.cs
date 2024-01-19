using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitingFruit : MonoBehaviour
{
    public GameObject fruit;
    public GameObject whole;
    public GameObject sliced;
    public GameObject cleaverPrefab;
    private bool slicingInProgress = false;
    private float cleaverSpeed = 30f;
    private float cleaverOffset = 1f; //Distance from fruit to cleaver when cut
    private float positionYOffset = 0.5f; //For setting knife cutting

    private void Slice()
    {
        if (slicingInProgress) //To prevent from making duplicates, because of Update().
            return;

        //Makes the object stop moving
        Rigidbody fruitRigidbody = fruit.GetComponent<Rigidbody>();
        fruitRigidbody.velocity = Vector3.zero;
        fruitRigidbody.isKinematic = true;

        //Changed tag to sliced, so we can start another object in FruitSpawner.cs
        fruit.tag = "Sliced";


        //Stops pieces that could fall down
        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.velocity = Vector3.zero;
            slice.angularVelocity = Vector3.zero;
            slice.isKinematic = true; 
            slice.detectCollisions = false; 
        }
    


        //Put it behind camera
        GameObject cleaver = Instantiate(cleaverPrefab, new Vector3(0f, 3f, -15f), Quaternion.Euler(-60f, 2f, 0f)); 

        //Set cleaver position and rotation
        if (cleaver != null)
        {
            /* 
            Rotating cleaver to match fruit
            float rotationIncrement = -5f; 
            cleaver.transform.rotation = whole.transform.rotation;
            cleaver.transform.Rotate(Vector3.up, rotationIncrement);
            */
            //Move cleaver to apple with slow animation
            StartCoroutine(CleaverSpawningAnimation(cleaver, fruit));
        }

        //Destroy(cleaver); Don't forget
        StartCoroutine(ResetSlicingFlag());
    }

    private IEnumerator ResetSlicingFlag()
    {
        yield return new WaitForSeconds(1f);
        slicingInProgress = true; 

    }
    //Chat unsure, have to check for best optimizations
    private IEnumerator CleaverSpawningAnimation(GameObject cleaver, GameObject fruit)
    {
        Vector3 cutIn = new Vector3(fruit.transform.position.x, fruit.transform.position.y - positionYOffset, fruit.transform.position.z - cleaverOffset);
        float arrivalThreshold = 0.01f; // For the lowest deviation

        while (Vector3.Distance(cleaver.transform.position, cutIn) > arrivalThreshold)
        {
            float step = cleaverSpeed * Time.deltaTime;
            cleaver.transform.position = Vector3.MoveTowards(cleaver.transform.position, cutIn, step);

            yield return null;
        }

        whole.SetActive(false);
        sliced.SetActive(true);


        cleaver.transform.position = cutIn; //To ensure that it's there

        //animation for tunring out
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxRaycastDistance = 10f; //Hits close to 5

            //Work in progress, works for all objects not only one. Chat made it maybe better solutions
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance) && (hit.collider.CompareTag("Fruit") || hit.collider.CompareTag("Watermelon"))) //If used multiple times better to store them
            {
                Slice();
            }
        }
    }
}
