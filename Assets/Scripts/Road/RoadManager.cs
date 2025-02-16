using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject[] roadPrefabs;
    public int roadCount = 5;
    public float roadLength = 15.0f * 1.5f;
    public float roadSpeed = 10f;

    private Queue<GameObject> roadSegments = new Queue<GameObject>();
    private float storedScaleFactor = 1.0f;

    public IReadOnlyCollection<GameObject> RoadSegments => roadSegments;

    public void InitializeRoads(Vector3 startPosition, Quaternion rotation, float scaleFactor)
    {
        ClearRoads();
        storedScaleFactor = scaleFactor;

        Vector3 roadDirection = Vector3.forward * roadLength; // Ensures correct forward placement

        for (int i = 0; i < roadCount; i++)
        {
            Vector3 position = startPosition + (roadDirection * i); // Always along Z-axis
            GameObject road = Instantiate(i == 0 ? roadPrefabs[0] : GetRandomRoadPrefab(), position, Quaternion.identity); // Ignore rotation
            road.transform.localScale = new Vector3(scaleFactor, 1, scaleFactor);
            roadSegments.Enqueue(road);
        }

        Debug.Log("Roads initialized in a straight line.");
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
        if (roadSegments.Count == 0) return;

        GameObject oldestRoad = roadSegments.Dequeue();
        Destroy(oldestRoad);

        GameObject lastRoad = roadSegments.ToArray()[roadSegments.Count - 1];
        Vector3 newPosition = lastRoad.transform.position + new Vector3(0, 0, roadLength);

        GameObject newRoad = Instantiate(GetRandomRoadPrefab(), newPosition, Quaternion.identity);
        newRoad.transform.localScale = new Vector3(storedScaleFactor, 1, storedScaleFactor);
        roadSegments.Enqueue(newRoad);
    }

    private GameObject GetRandomRoadPrefab()
    {
        if (roadPrefabs.Length == 0)
        {
            Debug.LogError("No road prefabs assigned!");
            return null;
        }
        return roadPrefabs[Random.Range(1, roadPrefabs.Length)];
    }

    private void ClearRoads()
    {
        foreach (var road in roadSegments)
        {
            Destroy(road);
        }
        roadSegments.Clear();
    }

    public GameObject GetFirstRoad()
    {
        if (roadSegments.Count > 0)
        {
            return roadSegments.Peek();
        }
        return null;
    }

}
