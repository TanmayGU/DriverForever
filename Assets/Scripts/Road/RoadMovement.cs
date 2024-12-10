using System.Collections.Generic;
using UnityEngine;

public class RoadMovement : MonoBehaviour
{
    public GameObject roadPrefab; // Assign your road prefab here in the Inspector
    public int numberOfRoads = 5; // Number of road segments to maintain
    public float roadLength = 30f; // Length of each road segment
    public float roadSpeed = 10f; // Speed at which the road moves

    private List<GameObject> activeRoads = new List<GameObject>(); // List to store active road segments

    void Start()
    {
        // Spawn the initial set of roads
        for (int i = 0; i < numberOfRoads; i++)
        {
            SpawnRoad(i * roadLength);
        }
    }

    void Update()
    {
        // Move all active roads backward
        foreach (GameObject road in activeRoads)
        {
            road.transform.position -= Vector3.forward * roadSpeed * Time.deltaTime;
        }

        // Reuse the first road when it's out of view
        if (activeRoads[0].transform.position.z < -roadLength)
        {
            GameObject roadToRecycle = activeRoads[0];
            activeRoads.RemoveAt(0); // Remove it from the list

            // Reposition the road to the back of the queue
            roadToRecycle.transform.position = activeRoads[activeRoads.Count - 1].transform.position + Vector3.forward * roadLength;
            activeRoads.Add(roadToRecycle); // Add it to the back of the list
        }
    }

    void SpawnRoad(float zPosition)
    {
        // Instantiate a new road prefab and position it
        GameObject newRoad = Instantiate(roadPrefab, new Vector3(0, 0, zPosition), Quaternion.identity);
        activeRoads.Add(newRoad); // Add it to the list of active roads
    }
}
