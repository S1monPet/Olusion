using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaver : MonoBehaviour
{
    private void Update()
    {
        
    }

    private void StartSpinning()
    {
        transform.Rotate(300f * Time.deltaTime, 0f, 0f, Space.Self);
    }

    private void StopSpinning()
    {
        transform.rotation = Quaternion.identity;
    }

}
