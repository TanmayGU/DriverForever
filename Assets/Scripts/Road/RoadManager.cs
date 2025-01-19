using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject[] roadPrefabs; // Array of road prefabs
    public int roadCount = 5;
    public float roadLength = 15.0f; // Match prefab Z scale
    public float roadSpeed = 10f;

    private Queue<GameObject> roadSegments = new Queue<GameObject>();

    // Add a read-only property to expose the road segments
    public IReadOnlyCollection<GameObject> RoadSegments => roadSegments;

    private float storedScaleFactor = 1.0f; // Stores the scale factor for consistent scaling

    public void InitializeRoads(Vector3 startPosition, Quaternion rotation, float scaleFactor)
    {
        ClearRoads();
        storedScaleFactor = scaleFactor; // Store the scale factor for consistent recycling

        for (int i = 0; i < roadCount; i++)
        {
            Vector3 position = startPosition + new Vector3(0, 0, i * roadLength);
            GameObject road = Instantiate(GetRandomRoadPrefab(), position, rotation);
            road.transform.localScale = new Vector3(scaleFactor, 1, scaleFactor);
            roadSegments.Enqueue(road);
        }

        Debug.Log("Roads initialized with random prefabs.");
    }

    public void UpdateRoads(float speed)
    {
        foreach (GameObject road in roadSegments)
        {
            road.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
        }

        if (roadSegments.Count > 0)
        {
            GameObject firstRoad = roadSegments.Peek();

            if (firstRoad.transform.position.z < -roadLength)
            {
                RecycleRoad();
            }
        }
    }

    private void RecycleRoad()
    {
        if (roadSegments.Count == 0)
        {
            Debug.LogWarning("No roads available to recycle!");
            return;
        }

        // Remove the oldest road
        GameObject oldestRoad = roadSegments.Dequeue();

        // Get the last road's position
        GameObject lastRoad = roadSegments.ToArray()[roadSegments.Count - 1];
        Vector3 newPosition = lastRoad.transform.position + new Vector3(0, 0, roadLength);

        // Reset the position
        oldestRoad.transform.position = new Vector3(
            Mathf.Round(newPosition.x * 100f) / 100f,
            Mathf.Round(newPosition.y * 100f) / 100f,
            Mathf.Round(newPosition.z * 100f) / 100f
        );

        // Reset scale to the stored scale factor
        oldestRoad.transform.localScale = new Vector3(storedScaleFactor, 1, storedScaleFactor);

        // Re-enqueue the recycled road
        roadSegments.Enqueue(oldestRoad);

        Debug.Log($"Recycled road to position: {oldestRoad.transform.position}");
    }

    private GameObject GetRandomRoadPrefab()
    {
        if (roadPrefabs.Length == 0)
        {
            Debug.LogError("No road prefabs assigned!");
            return null;
        }
        return roadPrefabs[Random.Range(0, roadPrefabs.Length)];
    }

    private void ClearRoads()
    {
        foreach (var road in roadSegments)
        {
            Destroy(road);
        }
        roadSegments.Clear();
    }
}
