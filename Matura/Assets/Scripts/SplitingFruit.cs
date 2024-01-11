using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitingFruit : MonoBehaviour
{
    public GameObject whole;
    public GameObject sliced;
    public GameObject cleaverPrefab;
    private bool slicingInProgress = false;

    private void Slice()
    {
        if (slicingInProgress) //To prevent from making duplicates, because of Update().
            return;

        whole.SetActive(false);
        sliced.SetActive(true);

        //Changed tag to sliced, so we can stop objects from despawning in FruitSpawner.cs
        sliced.tag = "Sliced";

        GameObject cleaver = Instantiate(cleaverPrefab);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.velocity = Vector3.zero;
            slice.angularVelocity = Vector3.zero;
            slice.useGravity = false;
            slice.freezeRotation = true;
            slice.detectCollisions = false;
        }
        if (cleaver != null)
        {
            Vector3 cleaverPosition = sliced.transform.position - sliced.transform.forward * 2; 
            cleaver.transform.position = cleaverPosition; 
        }
        //Destroy(cleaver); Don't forget
        StartCoroutine(ResetSlicingFlag());
    }

    private IEnumerator ResetSlicingFlag()
    {
        yield return new WaitForSeconds(1f);
        slicingInProgress = true; 

    }

    private void Update()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxRaycastDistance = 10f; //Hits close to 5

            //Work in progress, works for all objects not only one. Chat made it maybe better solutions
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance) && hit.collider.CompareTag("Fruit"))
            {
                Slice();
            }
        }
    }
}
