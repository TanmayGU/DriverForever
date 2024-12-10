using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject[] roadPrefabs; // Assign your road prefabs
    private List<GameObject> activeRoads = new List<GameObject>();

    public Transform playerTransform; // Assign the car transform
    private float zSpawn = 0;
    public float roadLength = 30;
    public int numberOfRoads = 5;

    void Start()
    {
        for (int i = 0; i < numberOfRoads; i++)
        {
            if (i == 0)
                SpawnRoad(0); // First road
            else
                SpawnRoad(Random.Range(0, roadPrefabs.Length));
        }
    }

    void Update()
    {
        if (playerTransform.position.z - 35 > zSpawn - (numberOfRoads * roadLength))
        {
            SpawnRoad(Random.Range(0, roadPrefabs.Length));
            DeleteRoad();
        }
    }

    void SpawnRoad(int roadIndex)
    {
        GameObject road = Instantiate(roadPrefabs[roadIndex], Vector3.forward * zSpawn, Quaternion.identity);
        activeRoads.Add(road);
        zSpawn += roadLength;
    }

    void DeleteRoad()
    {
        Destroy(activeRoads[0]);
        activeRoads.RemoveAt(0);
    }
}
