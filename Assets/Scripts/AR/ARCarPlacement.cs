using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCarPlacement : MonoBehaviour
{
    public GameObject carPrefab;
    public GameObject roadPrefab;
    public ARTrackedImageManager imageManager;
    public float carYOffset = 0.5f;

    private GameObject spawnedCar;
    private bool roadsInitialized = false;
    private HashSet<string> trackedImages = new HashSet<string>();

    public GameObject scanCanvas;

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
            PlaceObjects(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            RemoveObjects(trackedImage);
        }
    }

    private void PlaceObjects(ARTrackedImage trackedImage)
    {
        if (roadsInitialized) return;

        Vector3 position = trackedImage.transform.position;
        Quaternion rotation = trackedImage.transform.rotation;

        float roadScaleFactor = trackedImage.size.x;
        spawnedCar = Instantiate(carPrefab, position, rotation);
        //spawnedCar.transform.localScale = new Vector3(roadScaleFactor * 0.8f, roadScaleFactor * 0.8f, roadScaleFactor * 0.8f);
        spawnedCar.transform.position += new Vector3(0, carYOffset, 0);

        RoadManager roadManager = FindObjectOfType<RoadManager>();
        if (roadManager != null)
        {
            roadManager.InitializeRoads(position, rotation, roadScaleFactor);
        }

        roadsInitialized = true;
        scanCanvas.SetActive(false);
        GameManager.StartGame();
    }

    private void RemoveObjects(ARTrackedImage trackedImage)
    {
        if (trackedImages.Contains(trackedImage.referenceImage.name))
        {
            trackedImages.Remove(trackedImage.referenceImage.name);

            if (spawnedCar != null) Destroy(spawnedCar);
        }
    }
}
