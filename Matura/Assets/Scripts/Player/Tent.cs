using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Tent : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    public Transform playerTransform;
    public Vector3 _activationPosition; 

    [Header("UI")]
    public GameObject inventoryCanvas;
    public GameObject minimap;
    public GameObject map;



    private async void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.gameObject != player) return;

        await Task.Run(() => { DisablePlayerControls(); }); //No need for cancelation token
    }



    public void ExitTent() //Button
    {
        EnablePlayerControls();
    }

    private void EnablePlayerControls()
    {
        //Enabling Canvas
        inventoryCanvas.SetActive(true);
        minimap.SetActive(true);
        map.SetActive(true);


        playerTransform.position = _activationPosition;
        player.SetActive(false);
    }

    
    private void DisablePlayerControls()
    {
        //Disabling Canvas
        inventoryCanvas.SetActive(false);
        minimap.SetActive(false);
        map.SetActive(false);

        player.SetActive(false);
    }


}
