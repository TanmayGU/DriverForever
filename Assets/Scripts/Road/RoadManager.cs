using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject roadPrefab;
    public int roadCount = 5;
    public float roadLength = 15.0f; // Match prefab Z scale
    public float roadSpeed = 10f;

    private Queue<GameObject> roadSegments = new Queue<GameObject>();

    public void InitializeRoads(Vector3 startPosition, Quaternion rotation, float scaleFactor)
    {
        ClearRoads();

        for (int i = 0; i < roadCount; i++)
        {
            Vector3 position = startPosition + new Vector3(0, 0, i * roadLength);
            GameObject road = Instantiate(roadPrefab, position, rotation);
            road.transform.localScale = new Vector3(scaleFactor, 1, scaleFactor);
            roadSegments.Enqueue(road);
        }

        Debug.Log("Roads initialized.");
    }

    public void UpdateRoads(float speed)
    {
        foreach (GameObject road in roadSegments)
        {
            road.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
        }

        // Recycle the road segment when it moves far enough behind
        if (roadSegments.Count > 0)
        {
            GameObject firstRoad = roadSegments.Peek();

            // Check if the first road has moved far enough behind
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

        GameObject oldestRoad = roadSegments.Dequeue();
        GameObject lastRoad = roadSegments.ToArray()[roadSegments.Count - 1];
        Vector3 newPosition = lastRoad.transform.position + new Vector3(0, 0, roadLength);

        // Snap position to prevent any gaps
        oldestRoad.transform.position = new Vector3(
            Mathf.Round(newPosition.x * 100f) / 100f,
            Mathf.Round(newPosition.y * 100f) / 100f,
            Mathf.Round(newPosition.z * 100f) / 100f
        );

        roadSegments.Enqueue(oldestRoad);
        Debug.Log($"Recycled road to position: {oldestRoad.transform.position}");
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