using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    // Array of road prefabs to choose from for spawning
    public GameObject[] roadPrefabs;

    // Position along the Z-axis where the next road will be spawned
    public float zSpawn = 0;

    public Vector3 startPosition = Vector3.zero;


    // Length of each road prefab
    public float roadLenght = 30;

    // Number of roads to keep active at any given time
    public int numberOfRoads = 5;

    // A list to keep track of currently active roads in the scene
    private List<GameObject> activeRoads = new List<GameObject>();

    // Reference to the player's transform to track their position
    public Transform playerTransform;

    private void Start()
    {
        // Initializing the scene by spawning the initial set of roads
        for (int i = 0; i < numberOfRoads; i++)
        {
            // Always spawn the first road from the 0th index (e.g., a starting road)
            if (i == 0)
                SpawnRoad(0);
            else
                // Spawn a random road from the available prefabs for variety
                SpawnRoad(Random.Range(0, roadPrefabs.Length));
        }
    }

    private void Update()
    {
        // Check if the player has moved far enough to spawn a new road
        // Here, we spawn a new road if the player's position is 50 units behind the last spawned road
        if (playerTransform.position.z - 50 > zSpawn - (numberOfRoads * roadLenght))
        {
            // Spawn a new random road
            SpawnRoad(Random.Range(0, roadPrefabs.Length));

            // Remove the oldest road to maintain the set number of active roads
            DeleteRoad();
        }
    }

    // Method to spawn a road prefab at the correct position
    public void SpawnRoad(int roadIndex)
    {
        Vector3 spawnPosition = startPosition + transform.forward * zSpawn;
        GameObject go = Instantiate(roadPrefabs[roadIndex], spawnPosition, transform.rotation);
        activeRoads.Add(go);
        zSpawn += roadLenght;
    }

    // Method to delete the oldest road to prevent memory overload
    private void DeleteRoad()
    {
        // Destroy the first road in the list (the oldest road)
        Destroy(activeRoads[0]);

        // Remove it from the activeRoads list
        activeRoads.RemoveAt(0);
    }
}
