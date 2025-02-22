using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    // Array of road prefabs used for generating the road
    public GameObject[] roadPrefabs;

    // Number of road segments to be maintained in the scene at a time
    public int roadCount = 5;

    // Length of each road segment, adjusted by a scaling factor
    public float roadLength = 15.0f * 1.5f;

    // Speed at which the road moves towards the player
    public float roadSpeed = 10f;

    // Queue that stores active road segments for easy recycling
    private Queue<GameObject> roadSegments = new Queue<GameObject>();

    // Stores the scale factor for dynamically resizing road segments
    private float storedScaleFactor = 1.0f;

    // Public read-only access to the current road segments in the scene
    public IReadOnlyCollection<GameObject> RoadSegments => roadSegments;

    // Initializes the road by placing road segments in a straight line
    public void InitializeRoads(Vector3 startPosition, Quaternion rotation, float scaleFactor)
    {
        // Clear any previously existing road segments before initializing
        ClearRoads();

        // Store the scale factor for later use
        storedScaleFactor = scaleFactor;

        // Defines the direction in which roads will be placed (along the Z-axis)
        Vector3 roadDirection = Vector3.forward * roadLength;

        // Instantiate and position road segments
        for (int i = 0; i < roadCount; i++)
        {
            // Calculate the position of each road segment in the straight path
            Vector3 position = startPosition + (roadDirection * i);

            // Use a predefined first road prefab, then randomly assign prefabs for subsequent roads
            GameObject road = Instantiate(i == 0 ? roadPrefabs[0] : GetRandomRoadPrefab(), position, Quaternion.identity);

            // Apply the scale factor to adjust the road's size
            road.transform.localScale = new Vector3(scaleFactor, 1, scaleFactor);

            // Store the road segment in the queue
            roadSegments.Enqueue(road);
        }

        Debug.Log("Roads initialized in a straight line.");
    }

    // Moves all road segments backward to create an illusion of movement
    public void UpdateRoads(float speed)
    {
        foreach (GameObject road in roadSegments)
        {
            road.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
        }

        // Check if the first road segment has moved out of the visible area
        if (roadSegments.Count > 0)
        {
            GameObject firstRoad = roadSegments.Peek();
            if (firstRoad.transform.position.z < -roadLength)
            {
                RecycleRoad();
            }
        }
    }

    // Recycles the oldest road segment by repositioning it at the end of the queue
    private void RecycleRoad()
    {
        if (roadSegments.Count == 0) return;

        // Remove and destroy the oldest road segment
        GameObject oldestRoad = roadSegments.Dequeue();
        Destroy(oldestRoad);

        // Find the last road in the queue
        GameObject lastRoad = roadSegments.ToArray()[roadSegments.Count - 1];

        // Calculate the new position for the recycled road
        Vector3 newPosition = lastRoad.transform.position + new Vector3(0, 0, roadLength);

        // Instantiate a new road segment at the calculated position
        GameObject newRoad = Instantiate(GetRandomRoadPrefab(), newPosition, Quaternion.identity);

        // Apply the stored scale factor to maintain consistency
        newRoad.transform.localScale = new Vector3(storedScaleFactor, 1, storedScaleFactor);

        // Add the new road segment to the queue
        roadSegments.Enqueue(newRoad);
    }

    // Selects a random road prefab from the available options
    private GameObject GetRandomRoadPrefab()
    {
        // Ensure there are road prefabs assigned before attempting to pick one
        if (roadPrefabs.Length == 0)
        {
            Debug.LogError("No road prefabs assigned!");
            return null;
        }

        // Select a random road prefab, avoiding the first one which is reserved for initialization
        return roadPrefabs[Random.Range(1, roadPrefabs.Length)];
    }

    // Clears all currently existing road segments from the scene
    private void ClearRoads()
    {
        foreach (var road in roadSegments)
        {
            Destroy(road);
        }
        roadSegments.Clear();
    }

    // Retrieves the first road segment in the queue without removing it
    public GameObject GetFirstRoad()
    {
        if (roadSegments.Count > 0)
        {
            return roadSegments.Peek();
        }
        return null;
    }
}
