using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneFactor = 0.88f; // Ideal lane distance for a road width of 0.3428473m
    private float laneDistance;
    private int desiredLane = 1; // Default to center lane
    private RoadManager roadManager;

    public float roadSpeed = 10f; // Speed at which roads move

    private float baseRoadWidth = 0.3428473f; // The reference width for ideal laneFactor

    void Start()
    {
        roadManager = FindObjectOfType<RoadManager>();
        if (roadManager != null && roadManager.roadPrefab != null)
        {
            // Get the current road width dynamically
            float currentRoadWidth = roadManager.roadPrefab.transform.localScale.x; // Or fetched from the image tracker
            float imageWidth = GetTrackedImageWidth(); // Dynamically fetch the image width (if applicable)

            // Calculate lane distance proportionally
            laneDistance = laneFactor * (imageWidth / baseRoadWidth);

            Debug.Log($"Dynamic lane distance calculated: {laneDistance} (Current road width: {imageWidth})");
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

        // Move roads
        roadManager.UpdateRoads(roadSpeed);

        // Adjust position based on desired lane
        float targetX = (desiredLane - 1) * laneDistance; // Center lane is 1
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.deltaTime);
    }

    private float GetTrackedImageWidth()
    {
        // Fetch the width of the tracked image dynamically
        ARCarPlacement arPlacement = FindObjectOfType<ARCarPlacement>();
        if (arPlacement != null && arPlacement.imageManager != null)
        {
            foreach (var trackedImage in arPlacement.imageManager.trackables)
            {
                return trackedImage.size.x; // Dynamically return the width of the first tracked image
            }
        }

        Debug.LogWarning("No tracked image width found. Using default value.");
        return baseRoadWidth; // Fallback to the base reference width
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
