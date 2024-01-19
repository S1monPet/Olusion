using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaver : MonoBehaviour
{
    public GameObject fruit;
    public GameObject cleaver;
    //public Rigidbody fruitRigidbody; Might be useful
    public Rigidbody cleaverRigidbody;
    public float rotationIncrement = 8f;

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
            StopSpinning();
            StopAllCoroutines();
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
        cleaver.transform.rotation = fruit.transform.rotation;
        cleaver.transform.Rotate(new Vector3(-60f, rotationIncrement, 4f));
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
