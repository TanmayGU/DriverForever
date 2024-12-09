using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    // Direction vector for movement
    private Vector3 direction;

    // Forward movement speed
    public float forwardSpeed = 5f;

    // Lane management
    private int desiredLane = 1; // Default to the middle lane (0 = left, 1 = middle, 2 = right)
    public float laneDistance = 2.5f; // Distance between lanes

    // Jump and Gravity
    public float jumpForce = 10f;
    public float gravity = -20f;

    // Squeeze (crouch)
    private Vector3 originalScale;
    public float squeezeFactor = 0.5f;
    public float squeezeDuration = 0.8f;
    private bool isSqueezing = false;

    // Reference to AR camera
    public Transform arCameraTransform;

    void Start()
    {
        // Initialize the CharacterController
        controller = GetComponent<CharacterController>();

        // Save the player's original scale for squeeze logic
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Set forward movement speed
        direction.z = forwardSpeed;

        // Jump logic
        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            // Apply gravity when the player is in the air
            direction.y += gravity * Time.deltaTime;
        }

        // Handle lane movement
        HandleLaneSwitching();

        // Squeeze logic (crouch)
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isSqueezing)
        {
            StartCoroutine(Squeeze());
        }

        // Calculate the target position based on the desired lane
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }

        // Smoothly move the player to the target lane position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
    }

    private void FixedUpdate()
    {
        // Move the player forward based on the calculated direction
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        // Apply upward force for the jump
        direction.y = jumpForce;
    }

    private void HandleLaneSwitching()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane == 3)
                desiredLane = 2; // Stay within bounds
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane == -1)
                desiredLane = 0; // Stay within bounds
        }
    }

    private IEnumerator Squeeze()
    {
        isSqueezing = true;

        // Reduce the player's height
        Vector3 squeezedScale = new Vector3(originalScale.x, originalScale.y * squeezeFactor, originalScale.z);
        transform.localScale = squeezedScale;

        // Wait for the squeeze duration
        yield return new WaitForSeconds(squeezeDuration);

        // Revert to the original scale
        transform.localScale = originalScale;
        isSqueezing = false;
    }
}
