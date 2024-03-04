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
    public string GatheringTool;
    public float GatheringRate;
    public WaitForSeconds GatheringRateTimer;

    private TreeGathering _currentGatheringScript;

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
                    if (_hit.collider.CompareTag(TreeTag) && CanGather)
                    {
                        _currentGatheringScript = _hit.collider.GetComponent<TreeGathering>();
                        if (_currentGatheringScript != null)
                        {
                            _currentGatheringScript.Gather(_axeDamage, transform.root.gameObject, GatheringTool, GatheringRateTimer);
                            return; 
                        }
                    } 
                }

                if (_currentGatheringScript != null) 
                {
                    _currentGatheringScript.StopGathering();
                    _currentGatheringScript = null; //Reset the reference
                }
            }
        }
    }
}
