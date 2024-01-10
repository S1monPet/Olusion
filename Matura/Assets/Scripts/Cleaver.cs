using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaver : MonoBehaviour
{

    private Collider cleaverCollider;

    private void Awake()
    {
        cleaverCollider  = GetComponent<Collider>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSlicing();
        } else if (Input.GetMouseButtonUp(0))
        {
            StopSlicing(); 
        }
    }

    private void StartSlicing()
    {
        cleaverCollider.enabled = true;
    }

    private void StopSlicing()
    {
        cleaverCollider.enabled &= false;
    }
    
}
