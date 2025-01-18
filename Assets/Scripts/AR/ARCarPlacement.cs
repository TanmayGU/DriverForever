using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCarPlacement : MonoBehaviour
{
    public GameObject carPrefab; // The car to place
    public GameObject roadPrefab; // The road to place
    public ARTrackedImageManager imageManager; // Reference to the AR Tracked Image Manager
    public float carYOffset = 0.5f; // Y-offset for the car placement, adjustable in Inspector

    private GameObject spawnedCar;
    private GameObject spawnedRoad;
    private bool roadsInitialized = false; // Prevent redundant initialization

    // Keep track of already processed images
    private HashSet<string> trackedImages = new HashSet<string>();

    private void OnEnable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
            Debug.Log("ARTrackedImageManager.trackedImagesChanged event subscribed in ARCarPlacement.");
        }
    }

    private void OnDisable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            Debug.Log("ARTrackedImageManager.trackedImagesChanged event unsubscribed in ARCarPlacement.");
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log($"PlaceObjects called for added image: {trackedImage.referenceImage.name}");
            PlaceObjects(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            Debug.Log($"PlaceObjects called for updated image: {trackedImage.referenceImage.name}");
            PlaceObjects(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            Debug.Log($"RemoveObjects called for removed image: {trackedImage.referenceImage.name}");
            RemoveObjects(trackedImage);
        }
    }

    private void PlaceObjects(ARTrackedImage trackedImage)
    {
        if (roadsInitialized)
        {
            Debug.Log("Roads already initialized. Skipping PlaceObjects.");
            return;
        }

        // Skip if the image is already tracked
        if (trackedImages.Contains(trackedImage.referenceImage.name))
        {
            Debug.Log($"Tracked image '{trackedImage.referenceImage.name}' already processed.");
            return;
        }

        // Destroy previously spawned road and car if they exist
        if (spawnedRoad != null)
        {
            Debug.Log("Destroying previously spawned road.");
            Destroy(spawnedRoad);
            spawnedRoad = null;
        }

        if (spawnedCar != null)
        {
            Debug.Log("Destroying previously spawned car.");
            Destroy(spawnedCar);
            spawnedCar = null;
        }

        // Get position and rotation from tracked image
        Vector3 position = trackedImage.transform.position;
        Quaternion rotation = trackedImage.transform.rotation;

        // Calculate scale based on image size
        Vector2 imageSize = trackedImage.size;
        float roadScaleFactor = imageSize.x;
        float carScaleFactor = roadScaleFactor * 0.8f;

        // Spawn the road
        spawnedRoad = Instantiate(roadPrefab, position, rotation);
        spawnedRoad.transform.localScale = new Vector3(roadScaleFactor, 1, roadScaleFactor);
        spawnedRoad.transform.position += new Vector3(0, 0.01f, 0); // Adjust Y-position
        Debug.Log($"Road instantiated at position: {spawnedRoad.transform.position}");

        // Parent the road to RoadManager
        RoadManager roadManager = FindObjectOfType<RoadManager>();
        if (roadManager != null)
        {
            spawnedRoad.transform.SetParent(roadManager.transform);
            Debug.Log($"Road parented to: {roadManager.name}");
        }

        // Spawn the car
        spawnedCar = Instantiate(carPrefab, position, rotation);
        spawnedCar.transform.localScale = new Vector3(carScaleFactor, carScaleFactor, carScaleFactor);
        spawnedCar.transform.position += rotation * Vector3.forward * (imageSize.y * 0.5f);
        spawnedCar.transform.position += new Vector3(0, carYOffset, 0); // Adjust Y-position
        Debug.Log($"Car instantiated at position: {spawnedCar.transform.position}");

        // Initialize roads
        if (roadManager != null)
        {
            roadManager.InitializeRoads(position, rotation, roadScaleFactor);
            Debug.Log("Roads initialized by ARCarPlacement.");
        }

        // Add tracked image to the set
        trackedImages.Add(trackedImage.referenceImage.name);
        Debug.Log($"Tracked image '{trackedImage.referenceImage.name}' added to processed set.");

        // Mark roads as initialized
        roadsInitialized = true;
        GameManager.StartGame();
    }

    private void RemoveObjects(ARTrackedImage trackedImage)
    {
        if (trackedImages.Contains(trackedImage.referenceImage.name))
        {
            trackedImages.Remove(trackedImage.referenceImage.name);
            Debug.Log($"Tracked image '{trackedImage.referenceImage.name}' removed from processed set.");

            if (spawnedCar != null)
            {
                Debug.Log("Destroying spawned car.");
                Destroy(spawnedCar);
                spawnedCar = null;
            }

            if (spawnedRoad != null)
            {
                Debug.Log("Destroying spawned road.");
                Destroy(spawnedRoad);
                spawnedRoad = null;
            }
        }
    }
}
