using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacles;  // Array to hold different obstacle prefabs
    public float spawnInterval = 2f;  // Time interval between spawns
    public float roadWidth = 5f;  // Width of the road

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        // Check if the timer exceeds the spawn interval
        if (timer >= spawnInterval)
        {
            SpawnObstacle();  // Spawn an obstacle
            timer = 0f;  // Reset the timer
        }
    }

    void SpawnObstacle()
    {
        // Random position along the width of the road
        float spawnPositionX = Random.Range(-roadWidth / 2, roadWidth / 2);
        Vector3 spawnPosition = new Vector3(spawnPositionX, 0, transform.position.z);

        // Randomly select an obstacle prefab
        int obstacleIndex = Random.Range(0, obstacles.Length);
        Instantiate(obstacles[obstacleIndex], spawnPosition, Quaternion.identity);
    }
}