using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 7f;
    public float speedIncrease = 1f;
    public float maxHorizontalRange = 4f;
    public float rotationSpeed = 200f; // Adjust rotation speed here
    public float leftRotationOffset = -30f; // Adjust left rotation offset here
    public float rightRotationOffset = 25f; // Adjust right rotation offset here
    public float rollSpeed = 200f; // Adjust roll speed here

    private float targetRotation = 90f;
    private float rollDegrees = 0f;

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        //currentPosition.z += speed * Time.deltaTime;
        transform.position = currentPosition;

        speed += speedIncrease * Time.deltaTime;

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float touchX = touch.position.x;

            if (touchX < Screen.width / 2)
            {
                targetRotation = 90f + leftRotationOffset;
                MovePlayer(-0.5f); // Adjust the left movement speed here
            }
            else
            {
                targetRotation = 90f + rightRotationOffset;
                MovePlayer(0.5f); // Adjust the right movement speed here
            }
        }
        else
        {
            targetRotation = 90f;
        }

        // Rotate the player smoothly towards the target rotation
        Quaternion targetQuaternion = Quaternion.Euler(0f, targetRotation, rollDegrees);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuaternion, rotationSpeed * Time.deltaTime);

        // Add roll effect while the player has speed
        if (speed > 0)
        {
            rollDegrees += rollSpeed * Time.deltaTime;
        }
    }

    private void MovePlayer(float direction)
    {
        float moveHorizontal = direction * speed * Time.deltaTime;
        Vector3 currentPositionClamped = transform.position;
        currentPositionClamped.x += moveHorizontal;
        currentPositionClamped.x = Mathf.Clamp(currentPositionClamped.x, -maxHorizontalRange, maxHorizontalRange);
        transform.position = currentPositionClamped;
    }

    
}
