using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;

    private int desiredLane = 1; // 0: left, 1: middle, 2: right
    public float laneDistance = 2.5f; // The distance between two lanes

    public float jumpForce;
    public float Gravity = -20;

    private Vector3 originalScale; // Store the original scale
    public float squeezeFactor = 0.5f; // Factor by which the object is squeezed
    public float squeezeDuration = 0.8f; // How long the squeeze lasts
    private bool isSqueezing = false; // To check if already squeezing

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalScale = transform.localScale; // Save the original scale
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = forwardSpeed;

        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            direction.y += Gravity * Time.deltaTime;
        }

        // Gather the inputs for lane movement
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane == 3)
                desiredLane = 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane == -1)
                desiredLane = 0;
        }

        // Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }

        transform.position = targetPosition;

        // Handle squeezing
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isSqueezing)
        {
            StartCoroutine(Squeeze());
        }
    }

    private void FixedUpdate()
    {
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    private IEnumerator Squeeze()
    {
        isSqueezing = true;

        // Reduce height (Y-axis) while keeping X and Z proportionate
        Vector3 squeezedScale = new Vector3(originalScale.x, originalScale.y * squeezeFactor, originalScale.z);

        // Apply the squeeze
        transform.localScale = squeezedScale;

        // Wait for the squeeze duration
        yield return new WaitForSeconds(squeezeDuration);

        // Revert to the original scale
        transform.localScale = originalScale;

        isSqueezing = false;
    }
}
