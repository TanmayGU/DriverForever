using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField] private GameObject roadPrefab; // Tillgänglig via Unity Inspector
    private List<GameObject> roadSegments = new List<GameObject>();
    private float newZPosition = 0;

    public void InitializeRoads()
    {
        for (int i = 0; i < 5; i++)
        {
            var road = Instantiate(roadPrefab, new Vector3(0, 0, i * 30), Quaternion.identity);
            road.name = $"Road {i}";
            roadSegments.Add(road);
        }
    }

    public void RecycleRoad(GameObject road)
    {
        newZPosition += 30f;
        road.transform.position = new Vector3(0, 0, newZPosition);
        road.SetActive(true);
        Debug.Log($"Recycled road: {road.name} to position: {road.transform.position}");
    }

    void Update()
    {
        foreach (var road in roadSegments)
        {
            Debug.Log($"Road segment {road.name} is at position {road.transform.position}");
        }
    }
}
