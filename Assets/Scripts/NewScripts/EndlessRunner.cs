using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRunner : MonoBehaviour
{
    public GameObject roadPrefab;  // Road segment prefab
    public float roadLength = 10f;  // Length of each road segment
    public int numSegments = 5;  // Number of road segments to instantiate
    private Queue<GameObject> roadSegments = new Queue<GameObject>();  // Queue to manage road segments

    void Start()
    {
        // Instantiate initial road segments
        for (int i = 0; i < numSegments; i++)
        {
            Vector3 position = new Vector3(0, 0, i * roadLength);
            GameObject segment = Instantiate(roadPrefab, position, Quaternion.identity);
            roadSegments.Enqueue(segment);
        }
    }

    void Update()
    {
        // Check if the first segment is behind the car
        if (roadSegments.Peek().transform.position.z + roadLength < transform.position.z)
        {
            // Move the first segment to the end of the queue
            GameObject segment = roadSegments.Dequeue();
            segment.transform.position += Vector3.forward * roadLength * numSegments;
            roadSegments.Enqueue(segment);
        }
    }
}