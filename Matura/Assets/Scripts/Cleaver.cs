using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaver : MonoBehaviour
{
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

    private void OnDisable()
    {
        StopAllCoroutines(); 
    }
    

    private void StartSpinning()
    {
        transform.Rotate(300f * Time.deltaTime, 0f, 0f, Space.Self);
    }

    private void StopSpinning()
    {
        transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Fruit"))
        {
            StopSpinning(); 
        }
    }

}
