using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class ARCarPlacement : MonoBehaviour
{
    // Prefab of the car to be placed in AR
    public GameObject carPrefab;

    // Prefab of the road to be placed in AR
    public GameObject roadPrefab;

    // ARTrackedImageManager responsible for detecting and tracking images in AR
    public ARTrackedImageManager imageManager;

    // ARSession reference used for resetting and managing AR functionality
    private ARSession arSession;

    // Offset applied to the car's vertical position after placement
    public float carYOffset = 0.5f;

    // Stores a reference to the currently spawned car
    private GameObject spawnedCar;

    // Ensures roads are only initialized once per session
    private bool roadsInitialized = false;

    // Keeps track of recognized AR images to avoid duplicate placements
    private HashSet<string> trackedImages = new HashSet<string>();

    // UI canvas that prompts the user to scan for AR images
    public GameObject scanCanvas;

    private void Awake()
    {
        Debug.Log("ARCarPlacement:Awake() - Ensuring single instance.");

        // Prevents multiple instances of ARCarPlacement from existing at the same time
        if (FindObjectsOfType<ARCarPlacement>().Length > 1)
        {
            Debug.LogWarning("Multiple ARCarPlacement instances found! Destroying duplicate...");
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        Debug.Log("ARCarPlacement:OnEnable()");

        // Start the AR initialization process when this component is enabled
        StartCoroutine(InitializeAR());
    }

    private IEnumerator InitializeAR()
    {
        Debug.Log("Initializing AR system...");

        // Find the ARSession instance in the scene
        arSession = FindObjectOfType<ARSession>();

        // If ARSession is not found, wait for it to be initialized
        if (arSession == null)
        {
            Debug.LogWarning("ARSession not found. Waiting for initialization...");
            float waitTime = 0f;
            while (arSession == null && waitTime < 5f)
            {
                arSession = FindObjectOfType<ARSession>();
                yield return new WaitForSeconds(0.5f);
                waitTime += 0.5f;
            }
        }

        // If ARSession is still not found, log an error and stop execution
        if (arSession == null)
        {
            Debug.LogError("ARSession still not found! AR may not work.");
            yield break;
        }

        Debug.Log("ARSession is now active.");

        // Wait before performing the reset to ensure everything is initialized
        yield return new WaitForSeconds(2f);

        Debug.Log("Resetting AR session before enabling tracking...");
        StartCoroutine(ResetARSessionCompletely());

        // Subscribe to AR image tracking events if the image manager is available
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
            Debug.Log("Subscribed to ARTrackedImageManager.trackedImagesChanged.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from AR image tracking events when this component is disabled
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            Debug.Log("Unsubscribed from ARTrackedImageManager.trackedImagesChanged.");
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Iterate through newly detected AR images and place objects accordingly
        foreach (var trackedImage in eventArgs.added)
        {
            PlaceObjects(trackedImage);
        }
    }

    private void PlaceObjects(ARTrackedImage trackedImage)
    {
        // Ensure that roads are placed only once per session
        if (roadsInitialized)
        {
            Debug.LogWarning("Placement blocked! roadsInitialized is true.");
            return;
        }

        // Get the position of the tracked image where the car will be placed
        Vector3 position = trackedImage.transform.position;

        // Set a fixed rotation so the car faces forward (ignoring AR image rotation)
        Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

        // Instantiate the car at the detected AR marker position with the specified rotation
        spawnedCar = Instantiate(carPrefab, position, fixedRotation);

        // Adjust the car's position to apply the Y offset
        spawnedCar.transform.position += new Vector3(0, carYOffset, 0);

        // Find the RoadManager in the scene and initialize roads if available
        RoadManager roadManager = FindObjectOfType<RoadManager>();
        if (roadManager != null)
        {
            // Scale the road based on the detected image size
            float roadScaleFactor = (trackedImage.size.x) * 1.5f;
            roadManager.InitializeRoads(position, Quaternion.identity, roadScaleFactor);
        }

        // Mark roads as initialized to prevent further placement
        roadsInitialized = true;

        // Hide the scanning UI after successful placement
        scanCanvas.SetActive(false);

        // Start the game once the AR car and road are placed
        GameManager.StartGame();
    }

    public IEnumerator ResetARSessionCompletely()
    {
        Debug.Log("Starting full AR session reset...");

        // Ensure that ARSession is assigned before attempting a reset
        if (arSession == null)
        {
            Debug.LogWarning("ARSession not found. Trying again...");
            arSession = FindObjectOfType<ARSession>();

            if (arSession == null)
            {
                Debug.LogError("ERROR: ARSession still not found! Cannot reset.");
                yield break;
            }
        }

        // Wait until the ARSession has fully started
        while (arSession.subsystem != null && arSession.subsystem.running == false)
        {
            Debug.LogWarning("Waiting for ARSession to start...");
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Resetting ARSession...");

        try
        {
            // Perform the AR session reset
            arSession.Reset();
        }
        catch (System.Exception e)
        {
            Debug.LogError("CRITICAL ERROR: Failed to reset ARSession! " + e.Message);
            yield break;
        }

        // Wait for a moment before restarting image tracking
        yield return new WaitForSeconds(1.5f);

        if (imageManager != null)
        {
            Debug.Log("Restarting ARTrackedImageManager...");
            imageManager.enabled = false;
            yield return new WaitForSeconds(1f);
            imageManager.enabled = true;
        }

        // Ensure the scan canvas remains hidden after the reset
        yield return new WaitForSeconds(1f);
        scanCanvas.SetActive(false);
        Debug.Log("ARSession fully reset.");
    }
}
