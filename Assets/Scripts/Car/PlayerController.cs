using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    private int desiredLane = 1; // 0: left, 1: middle, 2: right

    public float laneDistance = 2.5f; // Distance between lanes
    public float jumpForce = 10f; // Force applied during jump
    public float gravity = -9.81f; // Gravity applied when in air
    public float lateralSpeed = 10f; // Speed of lane-switching

    private float fixedZPosition; // Keep the car fixed on the z-axis

    void Start()
    {
        controller = GetComponent<CharacterController>();
        fixedZPosition = transform.position.z; // Store the initial z-position of the car
    }

    void Update()
    {
        // Maintain a fixed z-position
        direction.z = 0;

        // Handle jumping
        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                direction.y = jumpForce;
        }
        else
        {
            direction.y += gravity * Time.deltaTime;
        }

        // Handle lane switching
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            desiredLane = Mathf.Max(0, desiredLane - 1);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            desiredLane = Mathf.Min(2, desiredLane + 1);

        // Determine the target position for lateral movement
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * laneDistance;

        // Smoothly interpolate to the target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, transform.position.y, fixedZPosition), lateralSpeed * Time.deltaTime);

        // Apply only lateral and vertical movement
        transform.position = new Vector3(smoothedPosition.x, transform.position.y, fixedZPosition);
    }

    void FixedUpdate()
    {
        // Apply movement based on the calculated direction (only vertical for jumping)
        controller.Move(direction * Time.fixedDeltaTime);
    }
}
