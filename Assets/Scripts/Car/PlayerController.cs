using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   
    private CharacterController controller;

    // Direction vector for movement, including forward, jump, and gravity
    private Vector3 direction;

    // Speed at which the player moves forward
    public float forwardSpeed;

    // Lane management variables
    private int desiredLane = 1; // Default lane (0 = left, 1 = middle, 2 = right)
    public float laneDistance = 2.5f; // Distance between two lanes

    // Jump mechanics
    public float jumpForce; // Strength of the jump
    public float Gravity = -20; // Gravity applied to the player when in the air

    // Squeeze mechanics
    private Vector3 originalScale; // Stores the player's original scale
    public float squeezeFactor = 0.5f; // Factor to reduce the height during a squeeze
    public float squeezeDuration = 0.8f; // Time the player stays squeezed
    private bool isSqueezing = false; // Tracks if the player is currently squeezing

    // Start is called before the first frame update
    void Start()
    {
        
        controller = GetComponent<CharacterController>();

        // Save the original scale of the player for later use
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Set the forward movement speed
        direction.z = forwardSpeed;

        // Check if the player is grounded before allowing a jump
        if (controller.isGrounded)
        {
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            // Apply gravity when the player is not grounded
            direction.y += Gravity * Time.deltaTime;
        }

        // Lane movement logic
        if (Input.GetKeyDown(KeyCode.RightArrow)) // Move to the right lane
        {
            desiredLane++;
            if (desiredLane == 3) // Prevent moving out of bounds (only 3 lanes)
                desiredLane = 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) // Move to the left lane
        {
            desiredLane--;
            if (desiredLane == -1) // Prevent moving out of bounds
                desiredLane = 0;
        }

        // Calculate the target position based on the current lane
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

        // Adjust target position based on the desired lane
        if (desiredLane == 0) // Left lane
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if (desiredLane == 2) // Right lane
        {
            targetPosition += Vector3.right * laneDistance;
        }

        // Move the player smoothly to the target lane
        transform.position = targetPosition;

        // Squeeze logic when the Down Arrow key is pressed
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isSqueezing)
        {
            // Start the squeeze coroutine
            StartCoroutine(Squeeze());
        }
    }

    
    private void FixedUpdate()
    {
        // Move the player based on the direction vector
        controller.Move(direction * Time.fixedDeltaTime);
    }

    
    private void Jump()
    {
        // Set the upward velocity for the jump
        direction.y = jumpForce;
    }

    // Coroutine to handle the squeeze action
    private IEnumerator Squeeze()
    {
        // Mark the player as squeezing
        isSqueezing = true;

        // Calculate the squeezed scale (reduce height, keep other dimensions the same)
        Vector3 squeezedScale = new Vector3(originalScale.x, originalScale.y * squeezeFactor, originalScale.z);

        // Apply the squeezed scale to the player
        transform.localScale = squeezedScale;

        // Wait for the specified duration
        yield return new WaitForSeconds(squeezeDuration);

        // Revert the player's scale back to the original
        transform.localScale = originalScale;

        // Mark the player as no longer squeezing
        isSqueezing = false;
    }
}
