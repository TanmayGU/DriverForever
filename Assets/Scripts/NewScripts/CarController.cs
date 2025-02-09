using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;  // Speed of the car
    public float laneDistance = 2f;  // Distance between lanes
    private int currentLane = 0;  // Current lane index
    private Vector3 targetPosition;  // Target position for lane change

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // Move the car forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Smoothly move the car to the target lane
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);

        // Handle touch input for lane changing
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < Screen.width / 2)
                {
                    ChangeLane(-1);  // Move to the left lane
                }
                else if (touch.position.x > Screen.width / 2)
                {
                    ChangeLane(1);  // Move to the right lane
                }
            }
        }
    }

    void ChangeLane(int direction)
    {
        // Update the current lane index
        currentLane += direction;
        currentLane = Mathf.Clamp(currentLane, -1, 1);  // Ensure the lane index stays within bounds

        // Calculate the target position based on the lane index
        targetPosition = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the car collides with an obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Handle collision with obstacle
            Debug.Log("Collided with obstacle");
            Destroy(collision.gameObject);
        }
    }
}