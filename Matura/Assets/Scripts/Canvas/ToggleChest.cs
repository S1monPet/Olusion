using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleChest : MonoBehaviour
{
    public Transform PlayerTransform;
    public Image openChestImage;
    public Image closeChestImage;
    public List<GameObject> chestList;
    private GameObject _nearestChest;

    private float _distance;
    private float _playerRange = 3f;
    private bool _isChestNear = false;

    private GameObject openChest;
    private GameObject closeChest;

    public Inventory inventoryScript;
    public GameObject inventory;

    private Chest chestScript;


    private void Awake()
    {
        openChest = openChestImage.gameObject;
        closeChest = closeChestImage.gameObject;
    }

    private void CheckIfChestIsNear()
    {
        _isChestNear = false;

        foreach (GameObject chest in chestList)
        {
            _distance = Vector3.Distance(PlayerTransform.position, chest.transform.position);
            if (_distance < _playerRange && chest.activeSelf) //Check if chest is still there and if distance is smaller than players range
            {
                // Setting Open and Close image gameObject 
                if (!inventory.activeInHierarchy)
                {
                    closeChest.SetActive(false);
                    openChest.SetActive(true);
                }
                else
                {
                    openChest.SetActive(false); 
                    closeChest.SetActive(true);
                }
                _nearestChest = chest;
                _isChestNear = true;

                //Debug.Log(_nearestChest);
                break;
            } 

            if (!_isChestNear)
            {
                // Disabling both UI's
                openChest.SetActive(false);
                closeChest.SetActive(false);

                chestScript = null;
                _nearestChest = null;
            }

        }

    }

    private void LateUpdate() //For better performance
    {
        CheckIfChestIsNear();
    }

    public void OpenChest() //Event
    {
        chestScript = _nearestChest.GetComponent<Chest>(); //Late but aight

        if (_nearestChest != null)
            inventoryScript.OpenChest(chestScript);
    }

    public void CloseChest() //Event
    {
        inventoryScript.ToggleInventory(false);
        chestScript = null; 
        _nearestChest = null;
        inventoryScript.playerMovementScript.enabled = true; 
    }
}
