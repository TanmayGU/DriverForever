// RoadManager.cs
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject roadPrefab;
    public int roadCount = 5;
    public float roadLength = 30f;

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

    public void RecycleRoad()
    {
        if (roadSegments.Count == 0)
        {
            Debug.LogWarning("No roads available to recycle!");
            return;
        }

        GameObject oldestRoad = roadSegments.Dequeue();
        Vector3 newPosition = oldestRoad.transform.position + new Vector3(0, 0, roadLength * roadCount);
        oldestRoad.transform.position = newPosition;
        roadSegments.Enqueue(oldestRoad);

        Debug.Log($"Recycled road to position: {newPosition}");
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
