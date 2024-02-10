using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    public Transform billBoardCam; 

    //Late update because Update can cause some Jitter, careful on how many you are moving here. 
    private void LateUpdate()
    {
        transform.LookAt(transform.position + billBoardCam.forward);
    }
}
