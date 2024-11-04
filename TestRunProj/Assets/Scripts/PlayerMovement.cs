using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 5f; // Speed of the automatic forward movement
    public float swipeSpeed = 10f; // Speed at which the player moves left/right
    public float laneWidth = 3f; // Distance between lanes (adjust based on your game)
    public float rotationSpeed = 5f;

    private float targetXOffset = 0f; // Offset for side movement
    private Quaternion targetRotation; // Stores the initial rotation of the player

    private Vector2 swipeStartPos; // Starting position of the swipe
    private float swipeThreshold = 50f; // Adjust this threshold as needed
    private bool isSwiping = false;
    private bool isMovingSideways = false;
    
    void Start()
    {
        targetRotation = transform.rotation;
    }

void Update()
    {
        // Move the player forward based on their current orientation
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;

        // Check for swipe start
        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPos = Input.mousePosition;
            isSwiping = true;
        }

        // Check for swipe end or significant movement
        if (isSwiping)
        {
            float swipeDeltaX = Input.mousePosition.x - swipeStartPos.x;
            if (Mathf.Abs(swipeDeltaX) > swipeThreshold)
            {
                isMovingSideways = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isSwiping = false;
                isMovingSideways = false;
            }
        }

        // Calculate target offset based on swipe input
        if (isSwiping)
        {
            float screenWidth = Screen.width;
            float touchPositionX = Input.mousePosition.x;

            // Calculate the swipe distance
            float swipeDeltaX = touchPositionX - swipeStartPos.x;

            // Normalize the swipe distance to a value between -1 and 1
            float normalizedSwipe = swipeDeltaX / screenWidth;

            // Set the target offset for left/right movement while swiping
            targetXOffset = Mathf.Clamp(normalizedSwipe * laneWidth, -laneWidth, laneWidth);
        }

        // Smoothly move the player to the target offset only if `isMovingSideways` is true
        if (isMovingSideways)
        {
            Vector3 lateralMovement = transform.right * targetXOffset;
            Vector3 targetPosition = transform.position + lateralMovement;
            transform.position = Vector3.Lerp(transform.position, targetPosition, swipeSpeed * Time.deltaTime);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Rotate the player by 90 degrees when pressing A or D
        if (Input.GetKeyDown(KeyCode.A))
        {
            RotatePlayer(-90f); // Rotate left
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            RotatePlayer(90f); // Rotate right
        }
    }

    private void RotatePlayer(float angle)
    {
        targetRotation *= Quaternion.Euler(0, angle, 0); // Update the target rotation smoothly
        targetXOffset = 0f; // Reset the target offset to 0 after rotation to prevent sudden jumps
    }
}
