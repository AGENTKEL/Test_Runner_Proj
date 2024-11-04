using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isStarted;
    public bool isLevelEnded;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private CanvasHandler _canvasHandler;
    
    public float forwardSpeed = 5f;
    public float swipeSpeed = 10f; 
    public float laneWidth = 3f;
    public float rotationSpeed = 5f;

    private float targetXOffset = 0f;
    private Quaternion targetRotation;

    private Vector2 swipeStartPos; 
    private float swipeThreshold = 50f;
    private bool isSwiping = false;
    private bool isMovingSideways = false;
    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (isLevelEnded)
        {
            playerAnimator.SetBool("Poor", false);
            playerAnimator.SetBool("Casual", false);
            playerAnimator.SetBool("Rich", false);
            playerAnimator.SetBool("Idle", true);
            return;
        }

        if (!isStarted)
        {
            playerAnimator.SetBool("Idle", true);
            if (Input.GetMouseButtonDown(0)) // 0 is for left mouse button or primary touch
            {
                isStarted = true;
                playerAnimator.SetBool("Idle", false);
                playerAnimator.SetBool("Casual", true);
                _canvasHandler.tutorialUI.SetActive(false);
            }
            return;
        }
        
        // Move forward
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;
        

        // Start of swipe
        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPos = Input.mousePosition;
            isSwiping = true;
            targetXOffset = 0f; // Reset target offset when starting a swipe
        }

        // Handle swipe
        if (isSwiping && Input.GetMouseButton(0))
        {
            float screenWidth = Screen.width;
            float touchPositionX = Input.mousePosition.x;

            float swipeDeltaX = touchPositionX - swipeStartPos.x;

            if (Mathf.Abs(swipeDeltaX) > 0)
            {
                float normalizedSwipe = swipeDeltaX / screenWidth;
                targetXOffset = Mathf.Clamp(normalizedSwipe * laneWidth, -laneWidth, laneWidth);
            }
            else
            {
                targetXOffset = 0f;
            }
        }

        // End of swipe
        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
            targetXOffset = 0f; // Reset the target offset to 0 when the swipe ends
        }

        // Calculate the target position
        Vector3 lateralMovement = transform.right * targetXOffset;
        Vector3 targetPosition = transform.position + lateralMovement;
        

        // Smoothly move the player to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, swipeSpeed * Time.deltaTime);

        // Smoothly rotate the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void RotatePlayer(float angle)
    {
        targetRotation *= Quaternion.Euler(0, angle, 0);
        targetXOffset = 0f;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Left"))
        {
            RotatePlayer(-90f);
            // Check for a child called "Checkpoint"
            Transform checkpointTransform = other.transform.Find("Checkpoint");
            if (checkpointTransform != null)
            {
                // Get the Animator component of the Checkpoint object
                Animator checkpointAnimator = checkpointTransform.GetComponent<Animator>();
                if (checkpointAnimator != null)
                {
                    // Activate the "Open" trigger in the animator
                    checkpointAnimator.SetTrigger("Open");
                    Debug.Log("Opened checkpoint: " + checkpointTransform.name);
                }
                else
                {
                    Debug.LogWarning("No Animator found on Checkpoint: " + checkpointTransform.name);
                }
            }
            else
            {
                Debug.LogWarning("No Checkpoint child found in: " + other.name);
            }
            other.enabled = false;
        }
        
        if (other.CompareTag("Right"))
        {
            RotatePlayer(90f);
            // Check for a child called "Checkpoint"
            Transform checkpointTransform = other.transform.Find("Checkpoint");
            if (checkpointTransform != null)
            {
                // Get the Animator component of the Checkpoint object
                Animator checkpointAnimator = checkpointTransform.GetComponent<Animator>();
                if (checkpointAnimator != null)
                {
                    // Activate the "Open" trigger in the animator
                    checkpointAnimator.SetTrigger("Open");
                    Debug.Log("Opened checkpoint: " + checkpointTransform.name);
                }
                else
                {
                    Debug.LogWarning("No Animator found on Checkpoint: " + checkpointTransform.name);
                }
            }
            else
            {
                Debug.LogWarning("No Checkpoint child found in: " + other.name);
            }
            other.enabled = false;
        }
        
        
        
        
    }
}
