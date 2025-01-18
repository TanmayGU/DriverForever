// PlayerController.cs
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneFactor = 0.88f; // Percentage of road width to calculate lane distance
    private float laneDistance;
    private int desiredLane = 1; // Default to center lane
    private RoadManager roadManager;

    void Start()
    {
        roadManager = FindObjectOfType<RoadManager>();
        if (roadManager != null && roadManager.roadPrefab != null)
        {
            laneDistance = roadManager.roadPrefab.transform.localScale.x * laneFactor;
            Debug.Log($"Calculated lane distance: {laneDistance}");
        }
        else
        {
            Debug.LogWarning("RoadManager or roadPrefab not found! Using default lane distance.");
            laneDistance = 3f; // Default fallback
        }
    }

    void Update()
    {
        if (!GameManager.gameStarted) return;

        HandleTouchInput();

        float targetX = (desiredLane - 1) * laneDistance; // Center is lane 1
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;

            if (touchPosition.x < Screen.width / 2) MoveLeft();
            else MoveRight();
        }
    }

    public void MoveLeft()
    {
        if (desiredLane > 0) desiredLane--;
        Debug.Log($"Moved to lane {desiredLane}");
    }

    public void MoveRight()
    {
        if (desiredLane < 2) desiredLane++;
        Debug.Log($"Moved to lane {desiredLane}");
    }
}
