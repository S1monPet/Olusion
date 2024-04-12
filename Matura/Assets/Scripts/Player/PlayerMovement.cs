using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour, IDataPersistance
{
    [Header("JoyStick")]
    [SerializeField] private FixedJoystick _fixedMovementJoystick; 
    [SerializeField] private FixedJoystick _fixedRotationJoystick;

    public GameObject _gameplayInteractions;

    // Event on Joysticks
    public bool MovementJoystickActive { get; set; }
    public bool RotationJoystickActive { get; set; }

    [Header("Player Position")]
    public Transform playerTransform;

    //public Camera camera;
    private LayerMask _combinedLayerMask;
    public LayerMask GroundLayerMask;
    public LayerMask EnemyLayerMask; 
    public LayerMask GatherableLayerMask;

    public GameObject inventory;
    public Inventory inventoryScript;

    private RaycastHit hit;
    public NavMeshAgent agent;
    public Animator animator;

    //Distance of player movement
    public static float MaxRaycastDistance = 20f; //Only one max
    public string GroundTag = "Ground";
    public string TreeTag = "Tree";
    public string CactusTag = "Cactus";
    public string EnemyTag = "Enemy";

    public float lookRotationSpeed = 20f;

    private Coroutine _rotationCoroutine = null;
    private Coroutine _targetRotationCoroutine = null; 

    public bool _canMove = true;
    public void LoadData(GameData data)
    {
        // On the first run, or if no save data exists, we skip setting the player's position to maintain the default start position.
        if (data.playerPosition == Vector3.zero) return;

        playerTransform.position = data.playerPosition;

        if (data.playerRotation != Quaternion.identity)
            playerTransform.rotation = data.playerRotation;

        agent.Warp(data.playerPosition); // Warping Agent to our position
    }

    public void SaveData(ref GameData data)
    {
        data.playerPosition = playerTransform.position;  //Not saving, because this is in Start
        data.playerRotation = playerTransform.rotation;
    }

    void Awake()
    {
        agent.updateRotation = false;
        // Setting all masks to check
        _combinedLayerMask = GroundLayerMask | EnemyLayerMask | GatherableLayerMask; 
    }

    private void Update()
    {
        if (Input.touchCount > 0 && !inventory.activeInHierarchy)
        {
            // Functions won't be reused so I am will be checking here for better performance
            if (MovementJoystickActive)
            {
                JoyStickMovement(); 
            }
            else if (RotationJoystickActive)
            {
                JoyStickRotation(); 
            }
        }
    }


    // Even on chest and button UI
    public void EnableInteractions()
    {
        if (!_gameplayInteractions.activeInHierarchy)
        {
            _gameplayInteractions.SetActive(true);
        }
    }

    // Even on chest and button UI
    public void DisableInteractions()
    {
        if (_gameplayInteractions.activeInHierarchy)
        {
            _gameplayInteractions.SetActive(false);
        }
    }

    public void JoyStickMovement()
    {
        Vector3 movementDirection = new Vector3(_fixedMovementJoystick.Horizontal, 0f, _fixedMovementJoystick.Vertical).normalized;

        if (movementDirection.magnitude >= 0.1f)
        {
            // Setting movement where Joy stick are pointing
            Vector3 destination = playerTransform.position + movementDirection;

            RotatePlayer(movementDirection);

            MovePlayer(destination);
        }
        // We stop
        else
        {
            StopMoving();
        }

        ManagePlayerAnimations(); 
    }

    public void JoyStickRotation()
    {
        Vector3 movementDirection = new Vector3(_fixedRotationJoystick.Horizontal, 0f, _fixedRotationJoystick.Vertical).normalized;
        RotatePlayer(movementDirection); 
    }

    private void RotatePlayer(Vector3 movementDirection)
    {
        if (movementDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            if (targetRotation != Quaternion.identity)
            {
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, lookRotationSpeed * Time.deltaTime);
            }
        }
    }

    private void ManagePlayerAnimations()
    {
        //Has path because we are resseting path in the other file if object is not clicked
        if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            // SetAgentRotation();
            animator.SetBool("isRunning", true); //Setting animation

            //If animation is not idle
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("ReceiveHit"))
                animator.Play("Running");

        }
        else if (!agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetBool("isRunning", false); //Setting animation off
        }
    }

    /*
    private void Movement()
    {
        //Checks if inventory is active
        if (!inventory.activeInHierarchy && _canMove)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    UnityEngine.Debug.DrawLine(ray.origin, ray.origin + ray.direction * MaxRaycastDistance, Color.green, 3);

                    if (Physics.Raycast(ray, out hit, MaxRaycastDistance, _combinedLayerMask)) 
                    {
                        // Checking Interactions
                        if (!hit.collider.CompareTag(GroundTag))
                        {
                            if (CanInteractWithTarget)
                            {
                                agent.destination = hit.point;
                                SetAgentRotation();

                                StopMoving();
                            }
                            else
                            {
                                MovePlayer(hit.point);
                            }
                            return; 
                        }
                        else 
                        {
                            MovePlayer(hit.point); 
                        }

                    }
                }
            } 

            //Has path because we are resseting path in the other file if object is not clicked
            if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                SetAgentRotation();
                animator.SetBool("isRunning", true); //Setting animation

                //If animation is not idle
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("ReceiveHit"))
                    animator.Play("Running");

            } 
            else if (!agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                animator.SetBool("isRunning", false); //Setting animation off
            }
        }
    }
    */

    // Button 
    private void MovePlayer(Vector3 destination)
    {
        // Disable any running coroutine
        StopOnGoingProcesses();

        // If grounds is hit and item was not pressed, set hit.point and go towards location
        if (agent.isStopped)
            agent.isStopped = false;

        agent.SetDestination(destination); // Returns bool
    }

    public void StopOnGoingProcesses()
    {
        inventoryScript.StopRestoration();
    }

    

    public void SetAgentRotation()
    {
        if (_rotationCoroutine != null)
        {
            StopCoroutine(_rotationCoroutine);
        }
        _rotationCoroutine = StartCoroutine(SetAgentRotationCoroutine());
    }

    private IEnumerator SetAgentRotationCoroutine() 
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        //Small threshold to avoid constant micro-adjustments && check if rotation is deafault
        while (lookRotation != Quaternion.identity && Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
            yield return null; 
        }
    }

    //Agent logic to stop moving and look towards item
    public void StopMoving() //For MAP UI
    {
        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        agent.ResetPath();
        animator.SetBool("isRunning", false);
    }

    public void StopMovingAndPlayAnimation(float timeToWaitToCancelAnimation)
    {
        _canMove = false; 
        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        agent.ResetPath();
        animator.SetBool("isRunning", false);

        WaitForAnimation(timeToWaitToCancelAnimation);
    }

    public void SetAgentRotationToTarget(GameObject target) //For fast rotation
    {
        if (_targetRotationCoroutine != null)
        {
            StopCoroutine(_targetRotationCoroutine);
        }
        StartCoroutine(SetAgentRotationToTargetCoroutine(target));
    }

    private IEnumerator SetAgentRotationToTargetCoroutine(GameObject target)
    {
        Vector3 direction = (target.transform.position - playerTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        while (lookRotation != Quaternion.identity && Quaternion.Angle(playerTransform.rotation, lookRotation) > 0.1f)
        {
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
            yield return null; 
        }
    }


    public void WaitForAnimation(float timeToWaitToCancelAnimation)
    {
        StartCoroutine(WaitForAnimationCoroutine(timeToWaitToCancelAnimation));
    }

    private IEnumerator WaitForAnimationCoroutine(float timeToWaitToCancelAnimation)
    {
        yield return new WaitForSeconds(timeToWaitToCancelAnimation);
        _canMove = true; 
    }
}

