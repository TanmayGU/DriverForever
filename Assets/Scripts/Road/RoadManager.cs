using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject[] roadPrefabs;
    public float zSpawn = 0;
    public float roadLenght = 30;
    public int numberOfRoads = 5;
    private List<GameObject> activeRoads = new List<GameObject>();

    public Transform playerTransform;
    private void Start()
    {
        //SpawnRoad(0);
        //SpawnRoad(1);
        //SpawnRoad(4);
        for (int i = 0;i < numberOfRoads; i++)
        {
            if (i == 0)
                SpawnRoad(0);
            else
                SpawnRoad(Random.Range(0, roadPrefabs.Length));
        }
    }


    private void Update()
    {
        if (playerTransform.position.z -50 > zSpawn - (numberOfRoads * roadLenght))
        {
            SpawnRoad(Random.Range(0, roadPrefabs.Length));
            DeleteRoad();
        }
    }
    public void SpawnRoad(int roadIndex)
    {
        GameObject go = Instantiate(roadPrefabs[roadIndex],transform.forward * zSpawn,transform.rotation);
        activeRoads.Add(go);
        zSpawn += roadLenght;
    }

    private void DeleteRoad()
    {
        Destroy(activeRoads[0]);
        activeRoads.RemoveAt(0);
    }
}
