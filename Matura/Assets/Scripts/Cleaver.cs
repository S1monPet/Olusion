using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaver : MonoBehaviour
{
    public GameObject cleaver;
    //public Rigidbody fruitRigidbody; Might be useful
    public Rigidbody cleaverRigidbody;

    private float fruitYRotation;
    private GameObject currentFruit; 

    void Start()
    {
        StartCoroutine(Spin());
    }

    IEnumerator Spin()
    {
        while(true)
        {
            StartSpinning();
            yield return null;
        }
    }

    //Fruit hitting and going through
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sliced"))
        {
            currentFruit = collision.gameObject;
            //fruitAnimator = currentFruit.GetComponent<Animator>(); Was used for animation

            StopSpinning();
            StopAllCoroutines();
            //Calling function after defining currentFruit
            SetCleaverRotation();
            cleaverRigidbody.isKinematic = true; //For being able to go through fruit
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines(); 
    }

    //Rotation cleaver to the roation of fruit
    private void SetCleaverRotation()
    {   
        if (currentFruit != null)
        {
            fruitYRotation = currentFruit.transform.rotation.y;
            cleaver.transform.Rotate(new Vector3(70f, fruitYRotation, 10f));
        }
    }
    

    private void StartSpinning()
    {
        transform.Rotate(2000 * Time.deltaTime, 0f, 0f, Space.Self);
    }

    private void StopSpinning()
    {
        transform.rotation = Quaternion.identity;
    }


}
