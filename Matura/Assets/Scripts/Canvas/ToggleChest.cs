using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ToggleChest : MonoBehaviour
{
    public Transform PlayerTransform;
    public Image chest;
    public List<GameObject> chestList;
    private GameObject _nearestChest;

    private float _distance;
    private float _playerRange = 3f;
    private bool _isChestNear = false; 

    private GameObject chestImage; 
    

    private void Awake()
    {
        chestImage = chest.gameObject; 
    }

    private void CheckIfChestIsNear()
    {
        _isChestNear = false; 

        foreach (GameObject chest in chestList)
        {
            _distance = Vector3.Distance(PlayerTransform.position, chest.transform.position);
            if (_distance < _playerRange)
            {
                _nearestChest = chest;
                chestImage.SetActive(true);
                _isChestNear = true; 
                //Debug.Log(_nearestChest);
                break; 
            } 
            
            if (!_isChestNear)
            {
                chestImage.SetActive(false);
                _nearestChest = null; 
            }
                
        }
        
    }

    private void Update() //For better performance
    {
        CheckIfChestIsNear();
    }
}
