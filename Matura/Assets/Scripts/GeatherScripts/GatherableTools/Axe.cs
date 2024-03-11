using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] private int _axeDamage = 5;

    public LayerMask layerMask;
    public bool CanGather; 

    private RaycastHit _hit;
    private float _maxRaycastDistance = 11f;

    public string TreeTag;
    public string CactusTag; 

    public string GatheringTool;
    public float GatheringRate;
    public WaitForSeconds GatheringRateTimer;

    private TreeGathering _previousTreeGatheringScript;
    private TreeGathering _currentTreeGatheringScript;
    private CactusGathering _currentCactusGatheringScript;

    private void OnEnable()
    {
        GatheringRateTimer = new WaitForSeconds(GatheringRate);
    }

    private void Awake()
    {

    }

    private void Update()
    {
        DetectIfObjectIsGatherable();
        Debug.Log(_currentTreeGatheringScript != null);
    }

    private void DetectIfObjectIsGatherable()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {

                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * _maxRaycastDistance, Color.blue, 15);

                if (Physics.Raycast(ray, out _hit, _maxRaycastDistance, layerMask))
                {
                    Debug.Log(_hit.collider.tag); 
                    //For Tree's
                    if (_hit.collider.CompareTag(TreeTag) && CanGather)
                    {
                        Debug.Log("set");
                        _previousTreeGatheringScript = _currentTreeGatheringScript;
                        _currentTreeGatheringScript = _hit.collider.GetComponent<TreeGathering>();
                        return; 
                    } 
                    //For Cactuse's
                    else if (_hit.collider.CompareTag(CactusTag) && CanGather)
                    {
                        _currentCactusGatheringScript = _hit.collider.GetComponent<CactusGathering>();
                        return; 
                    }
                }

                if (_currentTreeGatheringScript != null && _currentCactusGatheringScript != null) 
                {
                    _currentTreeGatheringScript.StopGathering();
                    _currentTreeGatheringScript = null; //Reset the reference

                    _currentCactusGatheringScript.StopGathering();
                    _currentCactusGatheringScript = null; 
                }
            }
        }
    }

    public void OnGatherButtonPress()
    {
        Debug.Log((_previousTreeGatheringScript != null) + " mamamaamaamamm");
        if (_previousTreeGatheringScript != null)
        {
            Debug.Log("Gathering");
            _previousTreeGatheringScript.Gather(_axeDamage, transform.root.gameObject, GatheringTool, GatheringRateTimer);
            return;
        } 
        else if (_currentCactusGatheringScript != null)
        {
            _currentCactusGatheringScript.Gather(_axeDamage, transform.root.gameObject, GatheringTool, GatheringRateTimer);
            return;
        }
    }
}
